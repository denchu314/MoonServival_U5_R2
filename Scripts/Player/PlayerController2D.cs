using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

/// <summary>
/// 2D�v���C���[�� WASD �ňړ����邽�߂̊ȒP�ȃX�N���v�g
/// Transform �𒼐ړ����������B
/// </summary>
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // �ړ����x�i�C���X�y�N�^�ŕύX�j
    [SerializeField] private Tilemap tilemap;  // �^�C���}�b�v��Inspector�Őݒ�

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
        //transform.position += move;
        Vector3 nextPos = transform.position + move;
        Vector3Int nextPosInt = new Vector3Int((int)nextPos.x, (int)nextPos.y, (int)nextPos.z);
        CustomTile tile = tilemap.GetTile<CustomTile>(nextPosInt);
        // �^�C�������݂��A�����s�\�Ȃ�ړ�
        if (tile != null && tile.isWalkable)
        {
            transform.position = nextPos;
        }
        else
        {
            Debug.Log("���̃^�C���ɂ͈ړ��ł��܂���: " + (tile != null ? tile.tileType.ToString() : "�Ȃ�"));
        }
    }
}
