using System.Collections.Generic;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static RoomManager Instance;

    // ======= フィールドや変数の定義 =======

    // タイルマップ情報（ここでは2次元配列で簡易的に扱う例）
    [SerializeField] public int width = 20;          // グリッドの横幅
    [SerializeField] public int height = 20;         // グリッドの高さ
    [SerializeField] public CustomTile.CustomTileType[,] tileTypeMap;     // 各セルのタイルタイプ(壁,空,パイプ etc)
    [SerializeField] private TextMeshProUGUI tileTypeMapText; // 認識しているタイルマップを表示するテキスト

    // 各セルがどのroomIDに属するかを格納 (屋外は -1 とする)
    private int[,] roomGrid;

    // 屋内空間(部屋)のリスト
    private Dictionary<int, RoomData> rooms = new Dictionary<int, RoomData>();

    // 各フレームの酸素消費などを計算するための設定
    [SerializeField] private float oxygenConsumptionRate = 5f; // 例: 1秒あたり5%減(大雑把に)
    [SerializeField] private float oxygenRefillRate = 10f;     // 例: 1秒あたり10%増など

    // パイプ接続管理
    // Room同士を繋いだ情報を保持(単純にListで扱う例)
    private List<(int, int)> roomConnections = new List<(int, int)>();

    private void Awake()
    {
        // シングルトンパターン
        if (Instance == null)
        {
            Instance = this;             // このオブジェクトをインスタンスに
            DontDestroyOnLoad(gameObject); // シーンが切り替わっても破棄しない
        }
        else
        {
            Destroy(gameObject); // 既に存在する場合は重複しないよう破棄
        }
    }

    // AwakeやStartで初期化
    private void Start()
    {
        // 例: tileTypeMap, roomGridを初期化
        tileTypeMap = new CustomTile.CustomTileType[width, height];
        roomGrid = new int[width, height];

        // サンプルとして全部Emptyで埋める (実際はゲーム開始時点のマップデータを設定)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileTypeMap[x, y] = CustomTile.CustomTileType.Empty;
            }
        }

        // 例: Flood Fillを走らせて部屋判定
        IdentifyRooms();
    }

    /// <summary>
    /// 毎フレーム酸素の増減などを更新
    /// </summary>
    private void Update()
    {
        // 例: 各部屋の酸素を消費 or 生成装置による供給
        // UpdateRooms(Time.deltaTime);

        // もしタイルが置かれた・壊されたなどのイベント時には、IdentifyRooms() を再実行するか検討
        // (省略)
    }

    /// <summary>
    /// Flood Fillを使って屋外/屋内を識別し、屋内ごとにRoomDataを作成する
    /// </summary>
    /// 

    public void IdentifyRooms()
    {
        // 1) roomGridを初期化 (全セルを -999 や -1 で埋める)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                roomGrid[x, y] = -999; // まだ未割り当てのフラグ
            }
        }
        // 既存のRoomDataもクリア
        rooms.Clear();

        // 2) マップ外から Flood Fillして屋外扱いにする
        //    例えば、マップ四辺を屋外起点にして空やパイプを探索 -> roomIDを -1 とする
        MarkOutside();

        // 3) 残りの -999 のセルでかつ Wall でない(=Empty等)セルを見つけたら、新しいRoomを作る
        int currentRoomID = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // まだ未割り当て && 壁でも屋外でもない
                if (roomGrid[x, y] == -999 && tileTypeMap[x, y] != CustomTile.CustomTileType.Wall)
                {
                    // 新しい部屋を見つけた
                    RoomData newRoom = new RoomData(currentRoomID);

                    // Flood Fillでこの部屋のタイルを集める
                    List<Vector2Int> roomTiles = FloodFillRoom(x, y, currentRoomID);
                    newRoom.tilesInRoom.AddRange(roomTiles);

                    // 部屋を登録
                    rooms.Add(currentRoomID, newRoom);
                    currentRoomID++;
                }
            }
        }
        Debug.Log("部屋の数は:" + rooms.Count + "個");
        // 4) パイプを確認して、部屋同士が繋がっているか判定(ここでは別メソッド)
        HandlePipes();
    }

    /// <summary>
    /// マップ外周から入ってこれる"空"セルを Flood Fillして屋外(-1)とする
    /// </summary>
    private void MarkOutside()
    {
        // 四辺を走査し、EmptyやPipeのセルを見つけたらFlood Fillで roomGrid = -1 を付与
        // 上辺と下辺
        for (int x = 0; x < width; x++)
        {
            FloodFillOutside(x, 0);
            FloodFillOutside(x, height - 1);
        }
        // 左辺と右辺
        for (int y = 0; y < height; y++)
        {
            FloodFillOutside(0, y);
            FloodFillOutside(width - 1, y);
        }
    }

    /// <summary>
    /// 指定セルがWallでなく、まだ未割り当てならFlood Fillで屋外(-1)とする
    /// </summary>
    private void FloodFillOutside(int startX, int startY)
    {
        // 範囲外チェック
        if (!InBounds(startX, startY)) return;
        // 既に割り当て済みor壁なら無視
        if (tileTypeMap[startX, startY] == CustomTile.CustomTileType.Wall || roomGrid[startX, startY] != -999) return;

        // BFSやDFSで連続領域を探索
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        roomGrid[startX, startY] = -1; // 屋外

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            // 4方向(or8方向)をチェック
            foreach (var dir in FourDirections())
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;
                if (InBounds(nx, ny) && tileTypeMap[nx, ny] != CustomTile.CustomTileType.Wall && roomGrid[nx, ny] == -999)
                {
                    roomGrid[nx, ny] = -1; // 屋外
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
    }

    /// <summary>
    /// 指定セルからFlood Fillして1つの部屋を特定し、roomIDを割り当てる
    /// </summary>
    private List<Vector2Int> FloodFillRoom(int startX, int startY, int roomID)
    {
        List<Vector2Int> collected = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        roomGrid[startX, startY] = roomID;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            collected.Add(current);

            // 4方向
            foreach (var dir in FourDirections())
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;
                if (InBounds(nx, ny)
                    && roomGrid[nx, ny] == -999
                    && tileTypeMap[nx, ny] != CustomTile.CustomTileType.Wall)
                {
                    roomGrid[nx, ny] = roomID;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
        return collected;
    }

    /// <summary>
    /// パイプで部屋同士が繋がっているかを調べ、roomConnectionsに登録
    /// </summary>
    private void HandlePipes()
    {
        roomConnections.Clear();

        // 全マスを見て、タイルがPipeの場合、周囲4方向の部屋IDを取得して繋ぐ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tileTypeMap[x, y] == CustomTile.CustomTileType.Pipe)
                {
                    // Pipeの上下左右を見て、異なるRoomIDを検出したら接続関係を登録
                    int pipeRoomID = roomGrid[x, y]; // pipe自体が属する部屋 (屋外=-1かもしれない)

                    HashSet<int> neighborRooms = new HashSet<int>();

                    foreach (var dir in FourDirections())
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;
                        if (InBounds(nx, ny))
                        {
                            int neighborID = roomGrid[nx, ny];
                            // 壁や-999以外なら部屋IDとして有効
                            if (neighborID >= 0) // -1は屋外なので今回は無視
                            {
                                neighborRooms.Add(neighborID);
                            }
                        }
                    }

                    // neighborRoomsに複数の異なるRoomIDがあれば、それらを互いに接続
                    // (単純にペアとして登録しておく例)
                    int[] roomArray = new int[neighborRooms.Count];
                    neighborRooms.CopyTo(roomArray);
                    for (int i = 0; i < roomArray.Length; i++)
                    {
                        for (int j = i + 1; j < roomArray.Length; j++)
                        {
                            var pair = (roomArray[i], roomArray[j]);
                            // 重複を避けるため、(小さいID, 大きいID)にそろえて登録
                            if (pair.Item1 > pair.Item2)
                            {
                                pair = (pair.Item2, pair.Item1);
                            }
                            if (!roomConnections.Contains(pair))
                            {
                                roomConnections.Add(pair);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 各フレーム呼ばれ、部屋内の酸素消費・生成装置の酸素補給・部屋間の酸素共有などを計算する
    /// </summary>
    public void UpdateRooms(float deltaTime)
    {
        // ==== 1) 酸素消費・酸素生成 ====
        foreach (var kv in rooms)
        {
            RoomData r = kv.Value;
            // 例: 常に酸素ConsumptionRate %/sec 減少
            r.oxygenLevel -= oxygenConsumptionRate * deltaTime;
            // 例: 何か酸素生成装置があれば加算(ここでは一律に仮定)
            r.oxygenLevel += oxygenRefillRate * deltaTime;

            // clamp
            r.oxygenLevel = Mathf.Clamp(r.oxygenLevel, 0f, 100f);
        }

        // ==== 2) 部屋間の接続による酸素共有 ====
        // ここでは単純に2部屋を同じ酸素量にそろえる(等分)例
        // 実際には部屋の容積や片方向伝達などを考慮する場合あり
        foreach (var pair in roomConnections)
        {
            int roomA = pair.Item1;
            int roomB = pair.Item2;
            if (!rooms.ContainsKey(roomA) || !rooms.ContainsKey(roomB)) continue;

            float avg = (rooms[roomA].oxygenLevel + rooms[roomB].oxygenLevel) / 2f;
            rooms[roomA].oxygenLevel = avg;
            rooms[roomB].oxygenLevel = avg;
        }
    }

    // ==== ユーティリティ関数 ==== //

    /// <summary>
    /// グリッド範囲内かどうか判定
    /// </summary>
    private bool InBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    /// <summary>
    /// 四方向(上下左右)のベクトル一覧
    /// </summary>
    private List<Vector2Int> FourDirections()
    {
        return new List<Vector2Int>
        {
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
            new Vector2Int(1,0),
            new Vector2Int(-1,0)
        };
    }
    public void UpdateTileTypeMapStringUI()
    {
        if (tileTypeMapText == null)
        {
            Debug.LogWarning("tileTypeMapText is not assigned.");
            return;
        }
        // 文字列を組み立てるためのバッファ
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // 上から下へ表示する場合
        //   for (int y = height - 1; y >= 0; y--)
        // 下から上に表示したい場合
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // tileTypeMap[x, y] の TileType を文字列化
                CustomTile.CustomTileType type = tileTypeMap[x, y];
                sb.Append(type.ToString());
                sb.Append(" ");
            }
            // 行末で改行
            sb.AppendLine();
        }

        // 組み立てた文字列をTextにセット
        tileTypeMapText.text = sb.ToString();
    }


}



