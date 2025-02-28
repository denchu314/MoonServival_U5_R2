using UnityEngine.Tilemaps;
using UnityEngine;

/// <summary>
/// フィールド上に置かれた2Dアイテムのスクリプト。
/// 名前・アイコン・Prefabへの参照を持つ。
/// </summary>
public class ItemPickup2D : MonoBehaviour
{
    [SerializeField] private string itemName;       // アイテム名
    [SerializeField] private Sprite itemIcon;       // インベントリ表示用アイコン
    [SerializeField] private GameObject itemPrefab; // 地面に置くときに生成するPrefab
    [SerializeField] private float itemQuantity;          // アイテムの量
    [SerializeField] private float itemDurability;      // アイテムの耐久性
    [SerializeField] private bool isOxygenItem;      // 酸素系アイテムか？
    [SerializeField] private bool isBuildingItem;      // 建築可能アイテムか？
    [SerializeField] private Tile itemTile; // 建築可能アイテムの場合のタイルの姿を設定


    // プロパティ（読み取り専用ゲッター）
    public string ItemName   => itemName;
    public Sprite ItemIcon   => itemIcon;
    public GameObject ItemPrefab => itemPrefab;

    public float ItemQuantity => itemQuantity;          // アイテムの量
    public float ItemDurability => itemDurability;      // アイテムの耐久性
    public bool IsOxygenItem => isOxygenItem; // 酸素系アイテムか？
    public bool IsBuildingItem => isBuildingItem; //建築可能アイテムか？
    public Tile ItemTile => itemTile;  // 建築可能アイテムの場合のタイルの姿
    // 注意: このスクリプトは 2D Collider (例: BoxCollider2D) を必ず付けること
    //       物理演算が必要なら Rigidbody2D も検討
}
