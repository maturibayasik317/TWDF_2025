using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("アップグレード設定")]
    [SerializeField] private UpgradeDatabase upgradeDatabase;
    [SerializeField] private GameObject upgradePopupUI;
    [SerializeField] private Transform upgradeOptionParent;
    [SerializeField] private GameObject upgradeButtonPrefab;

    [Header("NEXTボタン")]
    [SerializeField] private GameObject nextButtonObject;

    private List<UpgradeData> currentOptions = new List<UpgradeData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (upgradePopupUI != null)
            upgradePopupUI.SetActive(false);
    }

    /// <summary>
    /// ステージクリア時に呼ぶ
    /// </summary>
    public void ShowUpgradePopup()
    {
        if (upgradeDatabase == null || upgradeDatabase.allUpgrades.Count == 0)
        {
            Debug.LogWarning("UpgradeDatabase が設定されていません。");
            return;
        }

        // ポップアップを表示
        upgradePopupUI.SetActive(true);

        // NEXTボタンは非表示（選択後に表示）
        if (nextButtonObject != null)
            nextButtonObject.SetActive(false);

        // 既存ボタン削除
        foreach (Transform child in upgradeOptionParent)
            Destroy(child.gameObject);

        // ランダムに3つ抽出
        currentOptions = upgradeDatabase.allUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        foreach (var option in currentOptions)
        {
            var buttonObj = Instantiate(upgradeButtonPrefab, upgradeOptionParent);
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"{option.upgradeName}\n<size=80%>{option.description}</size>";

            var btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnUpgradeSelected(option));
        }
    }

    private void OnUpgradeSelected(UpgradeData data)
    {
        Debug.Log($"Upgrade Selected: {data.upgradeName}");

        // 対応するユニットに反映
        ApplyUpgradeToUnit(data);

        // UIを閉じてNEXTボタンを有効化
        upgradePopupUI.SetActive(false);
        if (nextButtonObject != null)
            nextButtonObject.SetActive(true);
    }

    private void ApplyUpgradeToUnit(UpgradeData data)
    {
        var unitList = DBManager.instance.unitSetting.UnitDataList;
        foreach (var unit in unitList)
        {
            if (unit.name == data.targetUnitName)
            {
                unit.attackPower += data.attackBonus;
                // UnitBlockにHPを持たせているため、UnitDataにmaxHpを追加するならそちらも反映可
                unit.blockCount += data.blockBonus;

                Debug.Log($"強化適用: {unit.name} 攻撃+{data.attackBonus} HP+{data.hpBonus} Block+{data.blockBonus}");
                return;
            }
        }

        Debug.LogWarning($"対象ユニット {data.targetUnitName} が見つかりませんでした。");
    }
}
