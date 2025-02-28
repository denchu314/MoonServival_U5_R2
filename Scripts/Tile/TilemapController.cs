using UnityEngine.Tilemaps;
using UnityEngine;
using TMPro;
using UnityEditor.EditorTools;
using static UnityEditor.PlayerSettings;

public class TilemapController : MonoBehaviour
{
    // �V���O���g���C���X�^���X
    public static TilemapController Instance;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool isBuildingMode; // ���݃��[�h���ǂ���
    [SerializeField] private TextMeshProUGUI modeText; // ���[�h��\������e�L�X�g

    private Tile currentTile; // ���I�𒆂̃^�C��(����)

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

    void Update()
    {
        // ���N���b�N����
        //if (Input.GetMouseButtonDown(0))
        //{
        //    // ���݃��[�h�ŁA�����ނ��I������Ă���΃^�C����u��
        //    if (isBuildingMode && currentTile != null)
        //    {


                
        //    }
        //}
    }

    // �Ⴆ��UI����Ă΂��: ���݃��[�h�I��/�I�t�؂�ւ�
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

    // �C���x���g������I�񂾃^�C��(�܂��̓X�v���C�g)��ݒ�
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
        // �J�X�^���^�C�����擾
        CustomTile tile = tilemap.GetTile<CustomTile>(cellPos);
        if (tile != null)
        {
            // tileType �Ŕ���
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
