using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnitSetting;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public int upgradeCount;

    [Header("Upgrade UI")]
    [SerializeField] private GameObject upgradeBar;
    [SerializeField] private Button button01;
    [SerializeField] private Button button02;
    [SerializeField] private Button button03;

    [SerializeField] private TextMeshProUGUI text01;
    [SerializeField] private TextMeshProUGUI text02;
    [SerializeField] private TextMeshProUGUI text03;

    [Header("設定")]
    [SerializeField] private UnitSetting unitSetting; // ← 既存のデータ
    [SerializeField] private List<UpgradeOption> upgradeOptions; // ScriptableObject を登録

    private UpgradeOption[] currentChosen = new UpgradeOption[3];
    private bool upgraded = false;

    void Awake()
    {
        Instance = this;
        upgradeBar.SetActive(false);
    }

    void Start()
    {
        unitSetting.InitializeRuntimeData();
    }


    // GameManager から呼ばれる
    public void ShowUpgradePopup()
    {
        upgraded = false;

        upgradeBar.SetActive(true);

        PickRandomUpgrades();
        ApplyUI();
        LockNextButton();
    }

    private void PickRandomUpgrades()
    {
        // 強化可能な Option のみ抽出
        List<UpgradeOption> validList = new List<UpgradeOption>();

        foreach (var option in upgradeOptions)
        {
            var unit = unitSetting.UnitDataList.Find(u => u.id == option.targetUnitId);

            if (unit != null && unit.upgradeCount < UnitData.maxUpgradeCount)
                validList.Add(option);
        }

        if (validList.Count == 0)
        {
            Debug.Log("強化可能なユニットが存在しません。UpgradeBar を表示しません。");
            upgradeBar.SetActive(false);
            UnlockNextButton();
            return;
        }

        // 3つランダム選択
        for (int i = 0; i < 3; i++)
        {
            int r = Random.Range(0, validList.Count);
            currentChosen[i] = validList[r];
            validList.RemoveAt(r);

            if (validList.Count == 0) break;
        }
    }


    private void ApplyUI()
    {
        text01.text = currentChosen[0].description;
        text02.text = currentChosen[1].description;
        text03.text = currentChosen[2].description;

        button01.onClick.RemoveAllListeners();
        button02.onClick.RemoveAllListeners();
        button03.onClick.RemoveAllListeners();

        button01.onClick.AddListener(() => SelectUpgrade(0));
        button02.onClick.AddListener(() => SelectUpgrade(1));
        button03.onClick.AddListener(() => SelectUpgrade(2));
    }

    private void SelectUpgrade(int index)
    {
        if (upgraded) return; // 二重防止
        upgraded = true;

        currentChosen[index].ApplyUpgrade(unitSetting);

        upgradeBar.SetActive(false);
        UnlockNextButton();
    }

    private void LockNextButton()
    {
        if (GameManager.Instance.nextButtonObject != null)
            GameManager.Instance.nextButtonObject.SetActive(false);
    }

    private void UnlockNextButton()
    {
        if (GameManager.Instance.nextButtonObject != null)
            GameManager.Instance.nextButtonObject.SetActive(true);
    }
}
