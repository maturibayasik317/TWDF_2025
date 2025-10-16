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
        public string name; // –¼‘O
        public int attackPower; // UŒ‚—Í
        public float attackInterval; // UŒ‚ŠÔŠu
        public float attackRange; // UŒ‚”ÍˆÍ
        public GameObject unitPrefab;//ƒ†ƒjƒbƒgê—pPrefab
    }
}
