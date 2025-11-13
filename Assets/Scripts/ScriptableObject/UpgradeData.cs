using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;        // 表示名
    [TextArea] public string description; // 強化の説明
    public Sprite icon;               // UI用アイコン

    public string targetUnitName;     // 対象ユニット（UnitData.nameと一致）

    public int attackBonus;
    public int hpBonus;
    public int blockBonus;
}
