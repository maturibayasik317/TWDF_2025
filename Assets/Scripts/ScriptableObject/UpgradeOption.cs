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
    public string upgradeId;
    public string targetUnitId;
    public string description;

    public UpgradeType type;
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
