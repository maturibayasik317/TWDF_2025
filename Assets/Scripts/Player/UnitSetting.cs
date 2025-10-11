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
        public string name; // ���O
        public int attackPower; // �U����
        public float attackInterval; // �U���Ԋu
        public float attackRange; // �U���͈�
        public Sprite UnitHeadSprite; // �C�g�̉摜
    }
}
