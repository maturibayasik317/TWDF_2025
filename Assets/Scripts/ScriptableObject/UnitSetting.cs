using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "UnitSetting", menuName = "ScriptableObject/Unit Setting")]
public class UnitSetting : ScriptableObject
{
    public List<UnitData> UnitDataList = new List<UnitData>();

    [Serializable]
    public class UnitData
    {
        public string id;   // ID
        public string name; // 名前
        public int maxHp = 5;//HP
        public int attackPower; // 攻撃力
        public float attackInterval; // 攻撃間隔
        public float attackRange; // 攻撃範囲
        public GameObject unitPrefab;//ユニット専用Prefab
        public int blockCount = 1;//ブロック可能数
        [Header("配置タイプ設定")]
        public bool canPlaceWay = true;      // 地上に設置可能
        public bool canPlaceHighWay = false; // 高台に設置可能
        [Header("強化管理")]
        public int upgradeCount = 0;
        public const int maxUpgradeCount = 3;
    }
}
