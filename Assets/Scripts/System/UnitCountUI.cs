using UnityEngine;
using TMPro;

public class UnitCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "Units: ";

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("UnitCountUI_TMP: TextMeshProUGUI がアサインされていません。Inspectorで割り当ててください。");
            enabled = false;
            return;
        }

        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.OnPlacedCountChanged += OnCountChanged;
            OnCountChanged(PlayerUnit.Instance.CurrentPlacedCount, PlayerUnit.Instance.MaxUnits);
        }
        else
        {
            text.text = $"{prefix}0/0";
        }
    }

    private void OnCountChanged(int current, int max)
    {
        text.text = $"{prefix}{current}/{max}";
    }

    private void OnDestroy()
    {
        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.OnPlacedCountChanged -= OnCountChanged;
        }
    }
}
