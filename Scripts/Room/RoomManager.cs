using System.Collections.Generic;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static RoomManager Instance;

    // ======= �t�B�[���h��ϐ��̒�` =======

    // �^�C���}�b�v���i�����ł�2�����z��ŊȈՓI�Ɉ�����j
    [SerializeField] public int width = 20;          // �O���b�h�̉���
    [SerializeField] public int height = 20;         // �O���b�h�̍���
    [SerializeField] public CustomTile.CustomTileType[,] tileTypeMap;     // �e�Z���̃^�C���^�C�v(��,��,�p�C�v etc)
    [SerializeField] private TextMeshProUGUI tileTypeMapText; // �F�����Ă���^�C���}�b�v��\������e�L�X�g

    // �e�Z�����ǂ�roomID�ɑ����邩���i�[ (���O�� -1 �Ƃ���)
    private int[,] roomGrid;

    // �������(����)�̃��X�g
    private Dictionary<int, RoomData> rooms = new Dictionary<int, RoomData>();

    // �e�t���[���̎_�f����Ȃǂ��v�Z���邽�߂̐ݒ�
    [SerializeField] private float oxygenConsumptionRate = 5f; // ��: 1�b������5%��(��G�c��)
    [SerializeField] private float oxygenRefillRate = 10f;     // ��: 1�b������10%���Ȃ�

    // �p�C�v�ڑ��Ǘ�
    // Room���m���q��������ێ�(�P����List�ň�����)
    private List<(int, int)> roomConnections = new List<(int, int)>();

    private void Awake()
    {
        // �V���O���g���p�^�[��
        if (Instance == null)
        {
            Instance = this;             // ���̃I�u�W�F�N�g���C���X�^���X��
            DontDestroyOnLoad(gameObject); // �V�[�����؂�ւ���Ă��j�����Ȃ�
        }
        else
        {
            Destroy(gameObject); // ���ɑ��݂���ꍇ�͏d�����Ȃ��悤�j��
        }
    }

    // Awake��Start�ŏ�����
    private void Start()
    {
        // ��: tileTypeMap, roomGrid��������
        tileTypeMap = new CustomTile.CustomTileType[width, height];
        roomGrid = new int[width, height];

        // �T���v���Ƃ��đS��Empty�Ŗ��߂� (���ۂ̓Q�[���J�n���_�̃}�b�v�f�[�^��ݒ�)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileTypeMap[x, y] = CustomTile.CustomTileType.Empty;
            }
        }

        // ��: Flood Fill�𑖂点�ĕ�������
        IdentifyRooms();
    }

    /// <summary>
    /// ���t���[���_�f�̑����Ȃǂ��X�V
    /// </summary>
    private void Update()
    {
        // ��: �e�����̎_�f������ or �������u�ɂ�鋟��
        // UpdateRooms(Time.deltaTime);

        // �����^�C�����u���ꂽ�E�󂳂ꂽ�Ȃǂ̃C�x���g���ɂ́AIdentifyRooms() ���Ď��s���邩����
        // (�ȗ�)
    }

    /// <summary>
    /// Flood Fill���g���ĉ��O/���������ʂ��A�������Ƃ�RoomData���쐬����
    /// </summary>
    /// 

    public void IdentifyRooms()
    {
        // 1) roomGrid�������� (�S�Z���� -999 �� -1 �Ŗ��߂�)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                roomGrid[x, y] = -999; // �܂������蓖�Ẵt���O
            }
        }
        // ������RoomData���N���A
        rooms.Clear();

        // 2) �}�b�v�O���� Flood Fill���ĉ��O�����ɂ���
        //    �Ⴆ�΁A�}�b�v�l�ӂ����O�N�_�ɂ��ċ��p�C�v��T�� -> roomID�� -1 �Ƃ���
        MarkOutside();

        // 3) �c��� -999 �̃Z���ł��� Wall �łȂ�(=Empty��)�Z������������A�V����Room�����
        int currentRoomID = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // �܂������蓖�� && �ǂł����O�ł��Ȃ�
                if (roomGrid[x, y] == -999 && tileTypeMap[x, y] != CustomTile.CustomTileType.Wall)
                {
                    // �V����������������
                    RoomData newRoom = new RoomData(currentRoomID);

                    // Flood Fill�ł��̕����̃^�C�����W�߂�
                    List<Vector2Int> roomTiles = FloodFillRoom(x, y, currentRoomID);
                    newRoom.tilesInRoom.AddRange(roomTiles);

                    // ������o�^
                    rooms.Add(currentRoomID, newRoom);
                    currentRoomID++;
                }
            }
        }
        Debug.Log("�����̐���:" + rooms.Count + "��");
        // 4) �p�C�v���m�F���āA�������m���q�����Ă��邩����(�����ł͕ʃ��\�b�h)
        HandlePipes();
    }

    /// <summary>
    /// �}�b�v�O����������Ă����"��"�Z���� Flood Fill���ĉ��O(-1)�Ƃ���
    /// </summary>
    private void MarkOutside()
    {
        // �l�ӂ𑖍����AEmpty��Pipe�̃Z������������Flood Fill�� roomGrid = -1 ��t�^
        // ��ӂƉ���
        for (int x = 0; x < width; x++)
        {
            FloodFillOutside(x, 0);
            FloodFillOutside(x, height - 1);
        }
        // ���ӂƉE��
        for (int y = 0; y < height; y++)
        {
            FloodFillOutside(0, y);
            FloodFillOutside(width - 1, y);
        }
    }

    /// <summary>
    /// �w��Z����Wall�łȂ��A�܂������蓖�ĂȂ�Flood Fill�ŉ��O(-1)�Ƃ���
    /// </summary>
    private void FloodFillOutside(int startX, int startY)
    {
        // �͈͊O�`�F�b�N
        if (!InBounds(startX, startY)) return;
        // ���Ɋ��蓖�čς�or�ǂȂ疳��
        if (tileTypeMap[startX, startY] == CustomTile.CustomTileType.Wall || roomGrid[startX, startY] != -999) return;

        // BFS��DFS�ŘA���̈��T��
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        roomGrid[startX, startY] = -1; // ���O

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            // 4����(or8����)���`�F�b�N
            foreach (var dir in FourDirections())
            {
                int nx = current.x + dir.x;
                int ny = current.y + dir.y;
                if (InBounds(nx, ny) && tileTypeMap[nx, ny] != CustomTile.CustomTileType.Wall && roomGrid[nx, ny] == -999)
                {
                    roomGrid[nx, ny] = -1; // ���O
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
    }

    /// <summary>
    /// �w��Z������Flood Fill����1�̕�������肵�AroomID�����蓖�Ă�
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

            // 4����
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
    /// �p�C�v�ŕ������m���q�����Ă��邩�𒲂ׁAroomConnections�ɓo�^
    /// </summary>
    private void HandlePipes()
    {
        roomConnections.Clear();

        // �S�}�X�����āA�^�C����Pipe�̏ꍇ�A����4�����̕���ID���擾���Čq��
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tileTypeMap[x, y] == CustomTile.CustomTileType.Pipe)
                {
                    // Pipe�̏㉺���E�����āA�قȂ�RoomID�����o������ڑ��֌W��o�^
                    int pipeRoomID = roomGrid[x, y]; // pipe���̂������镔�� (���O=-1��������Ȃ�)

                    HashSet<int> neighborRooms = new HashSet<int>();

                    foreach (var dir in FourDirections())
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;
                        if (InBounds(nx, ny))
                        {
                            int neighborID = roomGrid[nx, ny];
                            // �ǂ�-999�ȊO�Ȃ畔��ID�Ƃ��ėL��
                            if (neighborID >= 0) // -1�͉��O�Ȃ̂ō���͖���
                            {
                                neighborRooms.Add(neighborID);
                            }
                        }
                    }

                    // neighborRooms�ɕ����̈قȂ�RoomID������΁A�������݂��ɐڑ�
                    // (�P���Ƀy�A�Ƃ��ēo�^���Ă�����)
                    int[] roomArray = new int[neighborRooms.Count];
                    neighborRooms.CopyTo(roomArray);
                    for (int i = 0; i < roomArray.Length; i++)
                    {
                        for (int j = i + 1; j < roomArray.Length; j++)
                        {
                            var pair = (roomArray[i], roomArray[j]);
                            // �d��������邽�߁A(������ID, �傫��ID)�ɂ��낦�ēo�^
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
    /// �e�t���[���Ă΂�A�������̎_�f����E�������u�̎_�f�⋋�E�����Ԃ̎_�f���L�Ȃǂ��v�Z����
    /// </summary>
    public void UpdateRooms(float deltaTime)
    {
        // ==== 1) �_�f����E�_�f���� ====
        foreach (var kv in rooms)
        {
            RoomData r = kv.Value;
            // ��: ��Ɏ_�fConsumptionRate %/sec ����
            r.oxygenLevel -= oxygenConsumptionRate * deltaTime;
            // ��: �����_�f�������u������Ή��Z(�����ł͈ꗥ�ɉ���)
            r.oxygenLevel += oxygenRefillRate * deltaTime;

            // clamp
            r.oxygenLevel = Mathf.Clamp(r.oxygenLevel, 0f, 100f);
        }

        // ==== 2) �����Ԃ̐ڑ��ɂ��_�f���L ====
        // �����ł͒P����2�����𓯂��_�f�ʂɂ��낦��(����)��
        // ���ۂɂ͕����̗e�ς�Е����`�B�Ȃǂ��l������ꍇ����
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

    // ==== ���[�e�B���e�B�֐� ==== //

    /// <summary>
    /// �O���b�h�͈͓����ǂ�������
    /// </summary>
    private bool InBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    /// <summary>
    /// �l����(�㉺���E)�̃x�N�g���ꗗ
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
        // �������g�ݗ��Ă邽�߂̃o�b�t�@
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // �ォ�牺�֕\������ꍇ
        //   for (int y = height - 1; y >= 0; y--)
        // �������ɕ\���������ꍇ
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // tileTypeMap[x, y] �� TileType �𕶎���
                CustomTile.CustomTileType type = tileTypeMap[x, y];
                sb.Append(type.ToString());
                sb.Append(" ");
            }
            // �s���ŉ��s
            sb.AppendLine();
        }

        // �g�ݗ��Ă��������Text�ɃZ�b�g
        tileTypeMapText.text = sb.ToString();
    }


}



