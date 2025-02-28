using UnityEngine.Tilemaps;
using UnityEngine;
using TMPro;
using UnityEditor.EditorTools;
using static UnityEditor.PlayerSettings;

public class TilemapController : MonoBehaviour
{
    // シングルトンインスタンス
    public static TilemapController Instance;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool isBuildingMode; // 建設モードかどうか
    [SerializeField] private TextMeshProUGUI modeText; // モードを表示するテキスト

    private Tile currentTile; // 今選択中のタイル(建材)

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

    void Update()
    {
        // 左クリック判定
        //if (Input.GetMouseButtonDown(0))
        //{
        //    // 建設モードで、かつ建材が選択されていればタイルを置く
        //    if (isBuildingMode && currentTile != null)
        //    {


                
        //    }
        //}
    }

    // 例えばUIから呼ばれる: 建設モードオン/オフ切り替え
    public void SetBuildingMode()
    {
        isBuildingMode = true;
        modeText.text = "Building";
    }
    public void SetNormalMode()
    {
        isBuildingMode = false;
        modeText.text = "Normal";
    }

    public bool GetBuildingMode()
    {
        return isBuildingMode;
    }

    // インベントリから選んだタイル(またはスプライト)を設定
    public void PlaceTile(Tile tile)
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mousePos);
        //Debug.Log(cellPos.x + " " + cellPos.y + " " + cellPos.z);
        currentTile = tile;
        tilemap.SetTile(cellPos, currentTile);
    }


    public void CheckTile(Vector3Int cellPos)
    {
        // カスタムタイルを取得
        CustomTile tile = tilemap.GetTile<CustomTile>(cellPos);
        if (tile != null)
        {
            // tileType で判定
            if (tile.tileType == CustomTile.CustomTileType.Wall)
            {
                Debug.Log("This is a wall tile!");
            }
            else if (tile.tileType == CustomTile.CustomTileType.Pipe)
            {
                Debug.Log("This is a pipe tile!");
            }
            else
            {
                Debug.Log("Empty or other tile");
            }
        }
        else
        {
            Debug.Log("No tile found at this position");
        }
    }

    public CustomTile.CustomTileType GetTileType(Vector3Int cellPos)
    {
        CustomTile tile = tilemap.GetTile<CustomTile>(cellPos);
        if (tile == null)
        {
            tile.tileType = CustomTile.CustomTileType.Empty;
        }
        return tile.tileType;
    }
    public CustomTile.CustomTileType GetTileType(int x, int y)
    {
        Vector3Int cellPos = new Vector3Int(x,y);

        CustomTile tile = tilemap.GetTile<CustomTile>(cellPos);

        if (tile != null)
        {
            return tile.tileType;
        }
        else
        {
            return CustomTile.CustomTileType.Empty;
        }
    }

    public void UpdateTileTypeMap()
    {
        for (int x = 0; x < RoomManager.Instance.width; x++ ) {
            for (int y = 0; y < RoomManager.Instance.height; y++)
            {
                RoomManager.Instance.tileTypeMap[x,y] = GetTileType(x, y);
            }
        }
    }

    public Vector3Int GetClickPosition()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = TilemapController.Instance.tilemap.WorldToCell(mousePos);
        //Debug.Log(cellPos.x + " " + cellPos.y + " " + cellPos.z);
        return cellPos;
    }
}
