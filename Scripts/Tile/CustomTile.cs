using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 壁/パイプなどの区分情報を持つカスタムTile
/// </summary>
[CreateAssetMenu(fileName = "NewCustomTile", menuName = "Tiles/Custom Tile")]
public class CustomTile : Tile
{
    // タイルの種類を示す列挙型(例)
    public enum CustomTileType
    {
        Empty,
        Wall,
        Pipe,
        Other
    }

    // インスペクターで設定可能
    public CustomTileType tileType;

    // このタイルの上を歩けるか？
    public bool isWalkable = true;
}
