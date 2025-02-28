using UnityEngine;
using UnityEngine.UI;  // Slider �� Text ���g������
using TMPro;          // TextMeshPro ���g���ꍇ�ɕK�v

/// <summary>
/// �v���C���[�̎_�f�ʂ��Ǘ����AUI�ɔ��f����N���X�B
/// ���ԂƂƂ��Ɏ_�f����������C���[�W�B
/// </summary>
public class OxygenManager : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 100f;    // �_�f�̍ő�l
    [SerializeField] private float currentOxygen = 100f; // ���݂̎_�f��
    [SerializeField] private float oxygenDepletionRate = 5f; // 1�b������̎_�f������

    [Header("UI References")]
    [SerializeField] private Slider oxygenSlider;  // Slider�R���|�[�l���g���h���b�O���Ċ��蓖��
    [SerializeField] private TextMeshProUGUI oxygenText; // TextMeshPro�̕��������蓖��
    // ���� Text ���g���Ȃ�: [SerializeField] private Text oxygenText;

    private void Start()
    {
        // �X���C�_�[�̍ő�l�� maxOxygen �ɐݒ�
        oxygenSlider.maxValue = maxOxygen;
        // �J�n���̃X���C�_�[�𖞃^����
        oxygenSlider.value = currentOxygen;

        // �e�L�X�g���X�V
        UpdateOxygenUI();
    }

    private void Update()
    {
        // 1�b������ oxygenDepletionRate �����_�f�����炷
        currentOxygen -= oxygenDepletionRate * Time.deltaTime;

        // 0��艺�ɂ͂Ȃ�Ȃ��悤��Clamp
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // �X���C�_�[�̒l���X�V
        oxygenSlider.value = currentOxygen;

        // �e�L�X�g���X�V
        UpdateOxygenUI();

        // �_�f��0�ɂȂ�����Q�[���I�[�o�[���̏��������Ă��悢
        // if (currentOxygen <= 0f) { Debug.Log("Oxygen Depleted!"); }
    }

    // �������y������V�K�ǉ��z������
    /// <summary>
    /// �_�f���p�[�Z���g�񕜂���B(0.5 => 50%)
    /// </summary>
    public void RecoverOxygen(float recoverPercent)
    {
        // recoverPercent��0.5�̏ꍇ�AmaxOxygen * 0.5 = 50%������
        float recoverAmount = maxOxygen * recoverPercent;

        // �񕜕��𑫂��A����𒴂��Ȃ��悤Clamp
        currentOxygen = Mathf.Clamp(currentOxygen + recoverAmount, 0f, maxOxygen);

        // UI���X�V
        oxygenSlider.value = currentOxygen;
        UpdateOxygenUI();
    }
    // �������y�����܂Œǉ��z������

    /// <summary>
    /// �_�fUI(�e�L�X�g��X���C�_�[)���X�V����֐�
    /// </summary>
    private void UpdateOxygenUI()
    {
        // �X���C�_�[�ɂ͊��� currentOxygen �𔽉f�ς�
        // �e�L�X�g�ɂ̓p�[�Z���e�[�W��\��(�����_�ȉ��؂�̂ĂȂ�)
        float percentage = (currentOxygen / maxOxygen) * 100f;
        oxygenText.text = Mathf.FloorToInt(percentage).ToString() + "%";
    }
}
