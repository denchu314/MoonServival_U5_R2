using UnityEngine;
using UnityEngine.UI;  // Slider や Text を使うため
using TMPro;          // TextMeshPro を使う場合に必要

/// <summary>
/// プレイヤーの酸素量を管理し、UIに反映するクラス。
/// 時間とともに酸素が減少するイメージ。
/// </summary>
public class OxygenManager : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 100f;    // 酸素の最大値
    [SerializeField] private float currentOxygen = 100f; // 現在の酸素量
    [SerializeField] private float oxygenDepletionRate = 5f; // 1秒あたりの酸素減少量

    [Header("UI References")]
    [SerializeField] private Slider oxygenSlider;  // Sliderコンポーネントをドラッグして割り当て
    [SerializeField] private TextMeshProUGUI oxygenText; // TextMeshProの文字を割り当て
    // もし Text を使うなら: [SerializeField] private Text oxygenText;

    private void Start()
    {
        // スライダーの最大値を maxOxygen に設定
        oxygenSlider.maxValue = maxOxygen;
        // 開始時のスライダーを満タンに
        oxygenSlider.value = currentOxygen;

        // テキストも更新
        UpdateOxygenUI();
    }

    private void Update()
    {
        // 1秒あたり oxygenDepletionRate だけ酸素を減らす
        currentOxygen -= oxygenDepletionRate * Time.deltaTime;

        // 0より下にはならないようにClamp
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // スライダーの値を更新
        oxygenSlider.value = currentOxygen;

        // テキストも更新
        UpdateOxygenUI();

        // 酸素が0になったらゲームオーバー等の処理を入れてもよい
        // if (currentOxygen <= 0f) { Debug.Log("Oxygen Depleted!"); }
    }

    // ▼▼▼【ここを新規追加】▼▼▼
    /// <summary>
    /// 酸素をパーセント回復する。(0.5 => 50%)
    /// </summary>
    public void RecoverOxygen(float recoverPercent)
    {
        // recoverPercentが0.5の場合、maxOxygen * 0.5 = 50%分を回復
        float recoverAmount = maxOxygen * recoverPercent;

        // 回復分を足し、上限を超えないようClamp
        currentOxygen = Mathf.Clamp(currentOxygen + recoverAmount, 0f, maxOxygen);

        // UIを更新
        oxygenSlider.value = currentOxygen;
        UpdateOxygenUI();
    }
    // ▲▲▲【ここまで追加】▲▲▲

    /// <summary>
    /// 酸素UI(テキストやスライダー)を更新する関数
    /// </summary>
    private void UpdateOxygenUI()
    {
        // スライダーには既に currentOxygen を反映済み
        // テキストにはパーセンテージを表示(小数点以下切り捨てなど)
        float percentage = (currentOxygen / maxOxygen) * 100f;
        oxygenText.text = Mathf.FloorToInt(percentage).ToString() + "%";
    }
}
