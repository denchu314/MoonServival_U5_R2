using UnityEngine;

/// <summary>
/// 2D�v���C���[�� WASD �ňړ����邽�߂̊ȒP�ȃX�N���v�g
/// Transform �𒼐ړ����������B
/// </summary>
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // �ړ����x�i�C���X�y�N�^�ŕύX�j

    private void Update()
    {
        // WASD or �����L�[�̓��͂��擾 (��InputSystem��Horizontal/Vertical���g�p)
        float moveX = Input.GetAxis("Horizontal"); // A,D�L�[�܂��́�,��
        float moveY = Input.GetAxis("Vertical");   // W,S�L�[�܂��́�,��

        // ���̓x�N�g�����쐬
        Vector3 moveDir = new Vector3(moveX, moveY, 0f);

        // 1�t���[��������̈ړ��� (Time.deltaTime �Ńt���[���ˑ���␳)
        Vector3 move = moveDir * moveSpeed * Time.deltaTime;

        // Transform �𒼐ړ�����
        transform.position += move;
    }
}
