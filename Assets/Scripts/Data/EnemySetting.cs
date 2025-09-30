using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

[CreateAssetMenu(fileName = "EnemySetting", menuName = "ScriptableObject/Enemy Setting")]
public class EnemySetting : ScriptableObject
{
    public List<EnemyData> enemyDataList = new List<EnemyData>();

    [Serializable]
    public class EnemyData
    {
        public string id;   // ID
        public string name; // ���O
        public int maxHp;   // �ő�HP
        public float speed; // �ړ����x
    }
}
