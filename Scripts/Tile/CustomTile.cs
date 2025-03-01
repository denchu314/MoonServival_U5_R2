using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ��/�p�C�v�Ȃǂ̋敪�������J�X�^��Tile
/// </summary>
[CreateAssetMenu(fileName = "NewCustomTile", menuName = "Tiles/Custom Tile")]
public class CustomTile : Tile
{
    // �^�C���̎�ނ������񋓌^(��)
    public enum CustomTileType
    {
        Empty,
        Wall,
        Pipe,
        Other
    }

    // �C���X�y�N�^�[�Őݒ�\
    public CustomTileType tileType;

    // ���̃^�C���̏������邩�H
    public bool isWalkable = true;
}
