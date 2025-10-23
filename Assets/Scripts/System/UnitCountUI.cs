using UnityEngine;
using TMPro;

public class UnitCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "Units: ";
    private bool subscribed = false;

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("UnitCountUI_TMP: TextMeshProUGUI が割り当てられていません。Inspectorで割り当ててください。");
            enabled = false;
            return;
        }

        TrySubscribe();
    }

    void Update()
    {
        // PlayerUnit がまだ無い/未登録なら毎フレームチェックして一度だけ購読する
        if (!subscribed)
        {
            TrySubscribe();
        }
    }

    private void TrySubscribe()
    {
        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.OnPlacedCountChanged += OnCountChanged;
            // 初回表示（現在値を即反映）
            OnCountChanged(PlayerUnit.Instance.CurrentPlacedCount, PlayerUnit.Instance.MaxUnits);
            subscribed = true;
            Debug.Log("UnitCountUI_TMP: PlayerUnit に購読しました。");
        }
    }

    private void OnCountChanged(int current, int max)
    {
        // 安全化：nullチェック
        if (text == null) return;
        text.text = $"{prefix}{current}/{max}";
        Debug.Log($"UnitCountUI_TMP: 表示更新 {current}/{max}");
    }

    private void OnDestroy()
    {
        if (subscribed && PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.OnPlacedCountChanged -= OnCountChanged;
        }
    }
}
