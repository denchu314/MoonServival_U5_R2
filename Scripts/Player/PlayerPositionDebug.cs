using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionDebug : MonoBehaviour
{
    // プレイヤーのTransform
    [SerializeField] private Transform playerTransform;

    // 画面に表示するUI Text (TextMeshProを使う場合は型をTextMeshProUGUIに)
    [SerializeField] private TextMeshProUGUI debugText;

    private void Update()
    {
        // プレイヤーが割り当てられていれば、そのPositionを取得しテキストに表示
        if (playerTransform != null && debugText != null)
        {
            Vector2 pos = playerTransform.position;
            debugText.text = $"Player Position: X={pos.x:F2}, Y={pos.y:F2}";
        }
    }
}
