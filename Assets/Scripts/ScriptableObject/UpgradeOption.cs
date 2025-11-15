using UnityEngine;

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
    public void ApplyUpgrade(UnitSetting unitSetting)
    {
        foreach (var unit in unitSetting.UnitDataList)
        {
            if (unit.id == targetUnitId)
            {
                unit.attackPower += addAttack;
                unit.maxHp += addMaxHp;
                unit.blockCount += addBlock;

                Debug.Log($"[Upgrade] {unit.name} 強化完了 → ATK:{unit.attackPower}  HP:{unit.maxHp}  BLOCK:{unit.blockCount}");
            }
        }
    }
}
