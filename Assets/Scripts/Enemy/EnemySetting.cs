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
        public string name; // 名前
        public int maxHp;   // 最大HP
        public float speed; // 移動速度
        public AnimatorOverrideController overrideController; // 敵の移動アニメーション
    }
}
