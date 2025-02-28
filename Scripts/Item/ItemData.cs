using UnityEngine.Tilemaps;
using UnityEngine;

/// <summary>
/// 純粋にアイテムのデータのみを持つクラス。
/// シーン上のオブジェクト（GameObject）に依存しない。
/// </summary>
[System.Serializable] // Unity でシリアライズ(インスペクター表示など)可能にする
public class ItemData
{
    public string itemName;        // アイテム名
    public Sprite itemIcon;        // インベントリ表示用のアイコン
    public GameObject itemPrefab;  // 設置時に生成するPrefab

    // 数量、耐久度などが必要ならここに追加
    public float itemQuantity;          // アイテムの量
    public float itemDurability;      // アイテムの耐久性
    public bool  isOxygenItem;      // 酸素系アイテムか？
    public bool  isBuildingItem;    // 建築可能アイテムか？
    public Tile  itemTile; // 建築可能アイテムの場合のタイルの姿を設定


    // コンストラクタ（必要に応じて使う）
    public ItemData(string name, Sprite icon, GameObject prefab, float quantity, float durability, bool isoxygenitem, bool isbuildingitem, Tile itemtile)
    {
        itemName = name;
        itemIcon = icon;
        itemPrefab = prefab;
        itemQuantity = quantity;
        itemDurability = durability;
        isOxygenItem = isoxygenitem;
        isBuildingItem = isbuildingitem;
        itemTile = itemtile;

    }

    //public isOxgenItem()
    //{
    //    return this.isOxygenItem;
    //}

    // 引数なしコンストラクタ（シリアライズ用など）
    public ItemData() { }
}
