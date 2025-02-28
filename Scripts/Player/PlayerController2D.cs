using UnityEngine;

/// <summary>
/// 2Dプレイヤーが WASD で移動するための簡単なスクリプト
/// Transform を直接動かす方式。
/// </summary>
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 移動速度（インスペクタで変更可）

    private void Update()
    {
        // WASD or 方向キーの入力を取得 (旧InputSystemのHorizontal/Verticalを使用)
        float moveX = Input.GetAxis("Horizontal"); // A,Dキーまたは←,→
        float moveY = Input.GetAxis("Vertical");   // W,Sキーまたは↑,↓

        // 入力ベクトルを作成
        Vector3 moveDir = new Vector3(moveX, moveY, 0f);

        // 1フレームあたりの移動量 (Time.deltaTime でフレーム依存を補正)
        Vector3 move = moveDir * moveSpeed * Time.deltaTime;

        // Transform を直接動かす
        transform.position += move;
    }
}
