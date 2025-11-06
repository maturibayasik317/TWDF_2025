using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// シンプルな強化選択UIコントローラ。
/// Inspector:
///  - panelRoot: 強化パネルのルート GameObject（StageFlowManager が表示/非表示）
///  - optionButtons: 選択肢ボタン群（3つ用意するのが良い）
///  - optionTexts: 各ボタンに表示する説明テキスト（TMP）
/// 選択後は RogueProgress に反映し、StageFlowManager.ContinueToNextStage を呼ぶ。
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TextMeshProUGUI[] optionTexts;

    // 簡易な選択肢データ
    private struct UpgradeOption { public string label; public System.Action apply; }

    private List<UpgradeOption> options = new List<UpgradeOption>();

    private void Start()
    {
        // 初期は非表示（StageFlowManager が制御）
        if (panelRoot != null) panelRoot.SetActive(false);

        // ボタンの割当
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(idx));
        }
    }

    /// <summary>
    /// StageFlowManager から呼ばれて強化パネルを表示する前に選択肢を作る。
    /// </summary>
    public void ShowUpgradeChoices()
    {
        // 表示
        if (panelRoot != null) panelRoot.SetActive(true);

        // サンプル選択肢（ランダムか固定を好みで）
        options.Clear();
        options.Add(new UpgradeOption { label = "+1 配置枠", apply = () => RogueProgress.Instance.AddMaxUnits(1) });
        options.Add(new UpgradeOption { label = "+2 ユニットHP", apply = () => RogueProgress.Instance.AddUnitHp(2) });
        options.Add(new UpgradeOption { label = "+1 攻撃力", apply = () => RogueProgress.Instance.AddUnitAttack(1) });

        // テキスト表示（optionTextsの数に合わせて）
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (i < options.Count)
            {
                optionTexts[i].text = options[i].label;
                optionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnOptionSelected(int index)
    {
        if (index < 0 || index >= options.Count) return;
        // 適用
        options[index].apply?.Invoke();

        // Hide panel
        if (panelRoot != null) panelRoot.SetActive(false);

        // Continue to next stage
        if (StageFlowManager.Instance != null)
        {
            StageFlowManager.Instance.ContinueToNextStage();
        }
    }
}