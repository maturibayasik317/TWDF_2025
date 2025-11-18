using UnityEngine;
using static UnitSetting;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(menuName = "Game/UpgradeOption")]
public class UpgradeOption : ScriptableObject
{
    public string upgradeId;      // 強化の識別ID
    public string targetUnitId;   // 強化対象ユニットのID
    public string description;    // UIに表示する説明文

    public int addAttack = 0;
    public int addMaxHp = 0;
    public int addBlock = 0;

    // 強化処理（永続）
    // 強化処理（永続）
    public void ApplyUpgrade(UnitSetting setting)
    {
        foreach (var data in setting.UnitDataList)
        {
            if (data.id == targetUnitId)
            {
                if (data.upgradeCount >= UnitData.maxUpgradeCount)
                {
                    Debug.Log($"[{data.name}] は強化上限の {UnitData.maxUpgradeCount} に達しています。");
                    return;
                }

                // 強化処理
                if (addAttack != 0)
                    data.attackPower += addAttack;

                if (addMaxHp != 0)
                    data.maxHp += addMaxHp;

                if (addBlock != 0)
                    data.blockCount += addBlock;

                // 強化回数加算
                data.upgradeCount++;

                Debug.Log($"[{data.name}] 強化実行 → 現在 {data.upgradeCount} / {UnitData.maxUpgradeCount}");
            }
        }
    }
}
