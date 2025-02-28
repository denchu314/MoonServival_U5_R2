using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// プレイヤーの入力を受け取り、アイテムを拾う・使う・設置する制御を行う。
/// </summary>
public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;        // シーン内のカメラ
    [SerializeField] private OxygenManager oxygenManager; // 酸素を管理するクラス（酸素回復を呼び出すため）
    [SerializeField] private float itemConsumptionRate;
    //[SerializeField] private TilemapController tilemap;

    // Updateはフレームごとに呼ばれる
    private void Update()
    {
        // 右クリックを押した瞬間
        if (Input.GetMouseButtonDown(1))
        {
            // フィールド上のアイテムを拾う処理
            CheckPickupItem();
        }

        // Eキーを押した瞬間
        if (Input.GetKeyDown(KeyCode.E))
        {
            // インベントリのアイテムを使用する処理（酸素回復など）
            UseItemFromInventory();
        }

        // Qキーを押した瞬間
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (TilemapController.Instance.GetBuildingMode())
            {
                TilemapController.Instance.SetNormalMode();
                //Debug.Log("ノーマルモードにセットされました。");
            }
            else
            {
                TilemapController.Instance.SetBuildingMode();
                //Debug.Log("建築モードにセットされました。");
            }
        }

        // 左クリックを押した瞬間
        if (Input.GetMouseButtonDown(0))
        {
            ItemData selectedItem = InventoryManager2D.Instance.GetSelectedItemData();

            // アイテムが空でなければ
            if (selectedItem != null)
            {
                // 現在、建築モードならば
                if (TilemapController.Instance.GetBuildingMode())
                {
                    // selectedItem が建材タイプならば
                    if (selectedItem.isBuildingItem)
                    {
                        TilemapController.Instance.PlaceTile(selectedItem.itemTile);
                        
                        //Vector3Int cellPos = TilemapController.Instance.GetClickPosition();
                        //Debug.Log(cellPos.x + " " + cellPos.y + " " + cellPos.z);
                        //認識しているタイルマップの更新→tilemapの更新

                        //TilemapController.Instance.CheckTile(cellPos);

                        // TileTypeMapの更新
                        TilemapController.Instance.UpdateTileTypeMap();

                        //部屋判定
                        RoomManager.Instance.IdentifyRooms();
                        RoomManager.Instance.UpdateTileTypeMapStringUI();
                        
                    }
                    //selectedItem が建材タイプでないならば
                    else
                    {
                        Debug.Log("このアイテムは建設に使えません。");
                    }
                }
                // 現在、通常モードならば
                else
                {
                    // アイテムをフィールドに置く処理
                    PlaceItem();
                }
            }
            else
            {
                Debug.Log("アイテムがありません。");
            }
        }
    }

    /// <summary>
    /// フィールド上のアイテムがあれば拾う（右クリック）処理
    /// </summary>
    private void CheckPickupItem()
    {
        // マウスカーソル位置をワールド座標に変換
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // そこにコライダー2DがあるかRaycast2Dで確認 (点に向かって発射)
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
        if (hit.collider != null)
        {
            // 当たったオブジェクトに ItemPickup2D があるか
            ItemPickup2D itemPickup = hit.collider.GetComponent<ItemPickup2D>();
            if (itemPickup != null)
            {
                // インベントリに追加 (成功true/失敗false)
                bool success = InventoryManager2D.Instance.AddItem(itemPickup);
                if (success)
                {
                    // 拾えたらフィールド上のGameObjectを消す
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("インベントリがいっぱいで拾えません。");
                }
            }
        }
    }


    /// <summary>
    /// インベントリからアイテムを使用する(総合窓口)。
    /// ここでアイテムの種類を判定し、各種アイテムの処理を振り分ける。
    /// </summary>
    private void UseItemFromInventory()
    {
        // 選択中のアイテムを取得
        ItemData selectedItemData = InventoryManager2D.Instance.GetSelectedItemData();

        // アイテムが無い or 空の場合は何もしない
        if (selectedItemData != null)
        {
            // ここで、「アイテム名」「フラグ」「ID」などで種類を判定する。
            // 今回は例として、itemNameが "Oxygen Tank" なら酸素用継続処理を呼ぶ。
            if (selectedItemData.itemName == "Oxygen Tank")
            {
                // 酸素アイテムの使用処理
                UseOxygenItemContinuous(selectedItemData, itemConsumptionRate);
            }
            else
            {
                // 将来的に他のアイテムも増える場合は、ここで else if ... 
                // あるいは default 処理などで拡張しやすい形にしておく
                Debug.Log("このアイテムはまだ使用処理が定義されていません: " + selectedItemData.itemName);
            }
        }
        else
        {
            Debug.Log("使用できるアイテムが選択されていません。");
        }

    }

    /// <summary>
    /// Eキーを押している間、選択中アイテムが酸素なら少しずつ消費して酸素回復
    /// </summary>

    /// <summary>
    /// 酸素アイテムを継続消費しながら、酸素を回復する処理
    /// </summary>
    private void UseOxygenItemContinuous(ItemData selectedItemData, float itemConsumptionRate)
    {
        // 酸素アイテムかどうかをさらにダブルチェックする例(フラグなど)
        // if (!selectedItemData.isOxygenItem) return; 

        // 残量があるなら消費して回復
        if (selectedItemData.itemQuantity > 0f)
        {
            // 1秒あたり itemConsumptionRate %を消費
            float consume = itemConsumptionRate * Time.deltaTime;
            // 実際に消費可能な量 (アイテム残量が足りない場合はそちらを優先)
            float actualConsume = Mathf.Min(consume, selectedItemData.itemQuantity);

            // アイテム残量を減らす
            selectedItemData.itemQuantity -= actualConsume;

            // プレイヤーの酸素を actualConsume % 回復 (RecoverOxygen(引数は割合))
            // もし OxygenManager.RecoverOxygen() が 0~1の割合を要求するなら /100f する
            oxygenManager.RecoverOxygen(actualConsume / 100f);

            // インベントリのUIを更新
            //InventoryManager2D.Instance.UpdateInventoryUI();
            InventoryManager2D.Instance.UpdateUI(); // すべてインベントリの枠が表示される時がある。その時はここを消す。

            // もし残量が0以下になったら削除する等の挙動
            if (selectedItemData.itemQuantity <= 0f)
            {
                // 使い切りならスロットから取り除く
                // InventoryManager2D.Instance.RemoveSelectedItem();
                // Debug.Log("酸素ボンベを使い切りました。");
            }
        }
        else
        {
            // 0% なら回復できない
            Debug.Log("この酸素ボンベは空です！");
        }
    }

    /// <summary>
    /// 左クリックでフィールド上にアイテムを設置（従来の処理）
    /// </summary>
    private void PlaceItem()
    {
        // 選択中のアイテムデータを取得
        ItemData selectedItemData = InventoryManager2D.Instance.GetSelectedItemData();
        if (selectedItemData != null)
        {
            // マウス座標をワールド座標に変換
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // itemPrefab をInstantiateしてフィールドに配置
            Instantiate(selectedItemData.itemPrefab, mousePos, Quaternion.identity);

            // インベントリから削除(1個消費)
            InventoryManager2D.Instance.RemoveSelectedItem();
        }
    }


}

