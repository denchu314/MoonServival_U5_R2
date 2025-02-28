using UnityEngine;

/// <summary>
/// カメラがプレイヤーを追従するシンプルなスクリプト。
/// </summary>
public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;  // 追従するターゲット（プレイヤー）
    [SerializeField] private float followSpeed = 5f; // カメラ移動スピード
    [SerializeField] private Vector3 offset;    // ターゲットとの相対位置

    private void LateUpdate()
    {
        // ターゲットが設定されていなければ何もしない
        if (target == null) return;

        // 現在のカメラ位置
        Vector3 currentPos = transform.position;
        // ターゲットの位置 + オフセット
        Vector3 targetPos = target.position + offset;
        // スムーズに移動 (リニア補間)
        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, followSpeed * Time.deltaTime);

        // カメラ位置を更新
        transform.position = newPos;
    }
}

