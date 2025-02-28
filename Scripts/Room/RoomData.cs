using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������(����)��\���f�[�^�N���X�B
/// Flood Fill�ɂ�茟�o���ꂽ�^�C���Q��ێ����A�_�f�ʂȂǂ��Ǘ�����B
/// </summary>
public class RoomData
{
    public int roomID;                        // ��ӂ̕���ID (0,1,2�c)
    public float oxygenLevel = 100f;          // ���̕����̎_�f�� (0~100% �ȂǑz��)
    public List<Vector2Int> tilesInRoom;      // �ǂ̃^�C�����W�����̕����ɑ����邩

    // �R���X�g���N�^
    public RoomData(int id)
    {
        roomID = id;
        tilesInRoom = new List<Vector2Int>();
    }
}
