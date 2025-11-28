using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitButtonCooldown : MonoBehaviour
{
    [Header("ユニットID（UnitData.id と一致）")]
    public string unitId;

    [Header("UI参照")]
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image buttonImage;

    private Color originalColor;
    private bool initialized = false;

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        originalColor = buttonImage.color;

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        // PlayerUnit の辞書からクールタイムを取得
        if (!PlayerUnit.Instance.unitCooldownEndTime.TryGetValue(unitId, out float endTime))
            return;

        float remaining = endTime - Time.time;

        if (remaining > 0f)
        {
            // クール中 → テキスト表示
            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.CeilToInt(remaining).ToString();
            }

            // ボタン暗くする
            buttonImage.color = new Color(originalColor.r * 0.5f,
                                          originalColor.g * 0.5f,
                                          originalColor.b * 0.5f,
                                          originalColor.a);
        }
        else
        {
            // クール完了 → 非表示
            if (cooldownText != null)
                cooldownText.gameObject.SetActive(false);

            // 元の色に戻す
            buttonImage.color = originalColor;
        }
    }
}
