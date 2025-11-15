using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Upgrade UI")]
    [SerializeField] private GameObject upgradeBar;
    [SerializeField] private Button button01;
    [SerializeField] private Button button02;
    [SerializeField] private Button button03;

    [SerializeField] private TextMeshProUGUI text01;
    [SerializeField] private TextMeshProUGUI text02;
    [SerializeField] private TextMeshProUGUI text03;

    [Header("ê›íË")]
    [SerializeField] private UnitSetting unitSetting; // Å© ä˘ë∂ÇÃÉfÅ[É^
    [SerializeField] private List<UpgradeOption> upgradeOptions; // ScriptableObject Çìoò^

    private UpgradeOption[] currentChosen = new UpgradeOption[3];
    private bool upgraded = false;

    void Awake()
    {
        Instance = this;
        upgradeBar.SetActive(false);
    }

    // GameManager Ç©ÇÁåƒÇŒÇÍÇÈ
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
        List<UpgradeOption> list = new List<UpgradeOption>(upgradeOptions);
        for (int i = 0; i < 3; i++)
        {
            int r = Random.Range(0, list.Count);
            currentChosen[i] = list[r];
            list.RemoveAt(r);
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
        if (upgraded) return; // ìÒèdñhé~
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
