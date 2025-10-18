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
        public int attackPower; // 攻撃力
        public float attackInterval; // 攻撃間隔
        public float attackRange; // 攻撃範囲
        public GameObject unitPrefab;//ユニット専用Prefab
        public int blockCount = 1;//ブロック可能数
    }
}
