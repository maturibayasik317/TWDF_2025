using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Game/Upgrade Database")]
public class UpgradeDatabase : ScriptableObject
{
    public List<UpgradeData> allUpgrades = new List<UpgradeData>();
}
