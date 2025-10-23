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
            Debug.LogError("UnitCountUI_TMP: TextMeshProUGUI �����蓖�Ă��Ă��܂���BInspector�Ŋ��蓖�ĂĂ��������B");
            enabled = false;
            return;
        }

        TrySubscribe();
    }

    void Update()
    {
        // PlayerUnit ���܂�����/���o�^�Ȃ疈�t���[���`�F�b�N���Ĉ�x�����w�ǂ���
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
            // ����\���i���ݒl�𑦔��f�j
            OnCountChanged(PlayerUnit.Instance.CurrentPlacedCount, PlayerUnit.Instance.MaxUnits);
            subscribed = true;
            Debug.Log("UnitCountUI_TMP: PlayerUnit �ɍw�ǂ��܂����B");
        }
    }

    private void OnCountChanged(int current, int max)
    {
        // ���S���Fnull�`�F�b�N
        if (text == null) return;
        text.text = $"{prefix}{current}/{max}";
        Debug.Log($"UnitCountUI_TMP: �\���X�V {current}/{max}");
    }

    private void OnDestroy()
    {
        if (subscribed && PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.OnPlacedCountChanged -= OnCountChanged;
        }
    }
}
