using UnityEngine;

/// <summary>
/// �J�������v���C���[��Ǐ]����V���v���ȃX�N���v�g�B
/// </summary>
public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;  // �Ǐ]����^�[�Q�b�g�i�v���C���[�j
    [SerializeField] private float followSpeed = 5f; // �J�����ړ��X�s�[�h
    [SerializeField] private Vector3 offset;    // �^�[�Q�b�g�Ƃ̑��Έʒu

    private void LateUpdate()
    {
        // �^�[�Q�b�g���ݒ肳��Ă��Ȃ���Ή������Ȃ�
        if (target == null) return;

        // ���݂̃J�����ʒu
        Vector3 currentPos = transform.position;
        // �^�[�Q�b�g�̈ʒu + �I�t�Z�b�g
        Vector3 targetPos = target.position + offset;
        // �X���[�Y�Ɉړ� (���j�A���)
        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, followSpeed * Time.deltaTime);

        // �J�����ʒu���X�V
        transform.position = newPos;
    }
}

