using UnityEngine;
using static UnitSetting;

public enum UpgradeType
{
    AttackUp,
    HpUp,
    BlockUp
}

[CreateAssetMenu(menuName = "Game/UpgradeOption")]
public class UpgradeOption : ScriptableObject
{
    public string upgradeId;      // 強化の識別ID
    public string targetUnitId;   // 強化対象のユニット
    public string description;    // UI説明文

    public UpgradeType type;// 強化タイプ
    public int value = 1;  // 上昇量

    // ランタイムのみ強化
    public void ApplyUpgrade(UnitSetting setting)
    {
        UnitData data = setting.UnitDataList.Find(u => u.id == targetUnitId);
        if (data == null) return;

        if (data.upgradeCount >= UnitData.maxUpgradeCount)
        {
            Debug.Log($"[{data.name}] は強化上限に達しています。");
            return;
        }

        // 強化（ランタイム値を変更）
        switch (type)
        {
            case UpgradeType.AttackUp:
                data.runtimeAttack += value;
                break;

            case UpgradeType.HpUp:
                data.runtimeMaxHp += value;
                break;

            case UpgradeType.BlockUp:
                data.runtimeBlock += value;
                break;
        }

        data.upgradeCount++;

        Debug.Log($"[{data.name}] を強化 → Attack:{data.runtimeAttack}  HP:{data.runtimeMaxHp} Block:{data.runtimeBlock}");
    }
}
