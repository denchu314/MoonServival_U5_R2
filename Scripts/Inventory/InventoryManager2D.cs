using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インベントリ全体を管理するクラス（シングルトン）。
/// スロット配列を保持し、アイテム追加や削除の処理を行う。
/// UIの更新もここで行う。
/// </summary>
public class InventoryManager2D : MonoBehaviour
{
    // シングルトンインスタンス
    public static InventoryManager2D Instance;

    [SerializeField] private int slotCount = 10;           // スロット数（初期値10）
    [SerializeField] private InventorySlot[] slots;        // スロット配列
    [SerializeField] private Image[] slotImages;           // 各スロットに対応するUI Image（アイコン用）
    [SerializeField] private Color selectedColor = Color.yellow; // 選択中スロットの枠色
    [SerializeField] private Color normalColor = Color.white;    // 非選択スロットの枠色
    [SerializeField] private Slider[] slotQuantityBar; // 量表示バー
    [SerializeField] private TextMeshProUGUI inventoryNumText; // インベントリの選択しているスロットの番号を表示するテキスト


    private int selectedIndex = 0; // 現在選択中のスロットインデックス

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

        // slots 配列をスロット数ぶん用意
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = new InventorySlot(); // 各スロットを初期化
        }
    }

    private void Start()
    {
        UpdateUI(); // 起動時にUIを更新
    }

    private void Update()
    {
        // マウスホイールでスロット選択を切り替える処理
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(scroll);
        //var scroll = Input.mouseScrollDelta.y * Time.deltaTime * 10;
        var scroll = Input.mouseScrollDelta.y;
        //Debug.Log(scroll);
        
        // ホイールを上方向に回した
        if (scroll > 0.5)
        {
            selectedIndex--;               // インデックスを減らす
            if (selectedIndex < 0)         // 先頭より手前になったら最後に回す
                selectedIndex = slotCount - 1;
            
            //Debug.Log("slot:"+selectedIndex);
            inventoryNumText.text = selectedIndex.ToString(); // 選択しているインベントリの番号を表示
            UpdateUI();                    // UIを更新して選択枠を反映
            //scroll = 0;
        }
        // ホイールを下方向に回した
        else if (scroll < -0.5)
        {
            selectedIndex++;               // インデックスを増やす
            if (selectedIndex >= slotCount)// 最後より先に行ったら先頭に回す
                selectedIndex = 0;
            
            //Debug.Log("slot:"+selectedIndex);
            inventoryNumText.text = selectedIndex.ToString(); // 選択しているインベントリの番号を表示
            UpdateUI();
            //scroll = 0;
        }

    }

    /// <summary>
    /// アイテムをインベントリに追加する処理。
    /// </summary>
    /// <param name="newItemPickup">フィールド上にある ItemPickup2D の参照</param>
    /// <returns>追加できたらtrue、空きがなかったらfalse</returns>
    public bool AddItem(ItemPickup2D newItemPickup)
    {
        // 空きスロットを探す
        for (int i = 0; i < slotCount; i++)
        {
            // このスロットが空なら
            if (slots[i].itemData == null)
            {
                // 新たに ItemData を作って必要情報をコピーする
                ItemData copiedData = new ItemData();
                copiedData.itemName   = newItemPickup.ItemName;   // 名前
                copiedData.itemIcon   = newItemPickup.ItemIcon;   // アイコン
                copiedData.itemPrefab = newItemPickup.ItemPrefab; // 設置用のPrefab
                copiedData.itemQuantity = newItemPickup.ItemQuantity; // 量
                copiedData.itemDurability = newItemPickup.ItemDurability; // 耐久性
                copiedData.isOxygenItem = newItemPickup.IsOxygenItem; // 酸素系アイテムかどうか？
                copiedData.isBuildingItem = newItemPickup.IsBuildingItem; // 建築可能アイテムかどうか？
                copiedData.itemTile = newItemPickup.ItemTile; // 建築可能アイテムの場合のタイルの姿を設定

                // スロットに登録
                slots[i].itemData = copiedData;

                // UI更新
                UpdateUI();

                // 追加成功なのでtrueを返す
                return true;
            }
        }

        // 全スロット埋まっていて空きがない場合
        Debug.Log("インベントリがいっぱいです");
        return false;
    }

    /// <summary>
    /// 現在選択中のスロットのアイテムを取得する。
    /// </summary>
    /// <returns>選択中スロットの ItemData</returns>
    public ItemData GetSelectedItemData()
    {
        return slots[selectedIndex].itemData;
    }

    /// <summary>
    /// 現在選択中のスロットからアイテムを削除する。
    /// </summary>
    public void RemoveSelectedItem()
    {
        slots[selectedIndex].itemData = null; // データを削除(=空)
        UpdateUI();                           // UIを更新
    }

    /// <summary>
    /// UIスロットのアイコンや枠色を更新する。
    /// </summary>
    public void UpdateUI()
    {
        // スロット数だけループ
        for (int i = 0; i < slotCount; i++)
        {
            // slotImages[i] に対応するスロットのアイテムデータを反映する
            if (slots[i].itemData != null)
            {
                // アイコンを設定
                slotImages[i].sprite = slots[i].itemData.itemIcon;
                slotImages[i].color = Color.white; // アイコンを表示可能に
            }
            else
            {
                // アイテムデータが無いならアイコンを消す
                slotImages[i].sprite = null;
                slotImages[i].color = new Color(1, 1, 1, 0); // 完全透明
            }

            // 親オブジェクトの枠（Image）があれば色を変える
            Image parentFrame = slotImages[i].transform.parent.GetComponent<Image>();
            if (parentFrame != null)
            {
                // 選択中なら黄色、それ以外は白
                parentFrame.color = (i == selectedIndex) ? selectedColor : normalColor;
            }

            // ▼ 残量バーの更新 ▼
            if (slotQuantityBar != null && slotQuantityBar.Length > i && slotQuantityBar[i] != null)
            {
                if (slots[i].itemData != null && slots[i].itemData.isOxygenItem)
                {
                    // isOxygenItem == true ならバーを表示し、残量を反映
                    slotQuantityBar[i].gameObject.SetActive(true);
                    slotQuantityBar[i].maxValue = 100f;
                    slotQuantityBar[i].value = slots[i].itemData.itemQuantity;
                }
                else
                {
                    // それ以外のアイテムなら非表示
                    slotQuantityBar[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
