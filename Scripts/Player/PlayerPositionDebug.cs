using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionDebug : MonoBehaviour
{
    // �v���C���[��Transform
    [SerializeField] private Transform playerTransform;

    // ��ʂɕ\������UI Text (TextMeshPro���g���ꍇ�͌^��TextMeshProUGUI��)
    [SerializeField] private TextMeshProUGUI debugText;

    private void Update()
    {
        // �v���C���[�����蓖�Ă��Ă���΁A����Position���擾���e�L�X�g�ɕ\��
        if (playerTransform != null && debugText != null)
        {
            Vector2 pos = playerTransform.position;
            debugText.text = $"Player Position: X={pos.x:F2}, Y={pos.y:F2}";
        }
    }
}
