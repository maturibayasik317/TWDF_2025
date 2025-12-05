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

        [Header("AI / 出現設定")]
        public float weight = 1f;   // 出現確率（総当たり方式）
        public EnemyType type;      // 敵タイプ
        public EnemyType enemyType = EnemyType.Normal;
    }

    public enum EnemyType
    {
        Normal,
        Fast,
        Tank,
        Air,
        Boss
    }

    public int GetRandomEnemyIdByType(EnemyType targetType)
    {
        List<int> candidateIndexes = new List<int>();

        // 指定タイプの敵をリスト化
        for (int i = 0; i < enemyDataList.Count; i++)
        {
            if (enemyDataList[i].enemyType == targetType)
            {
                candidateIndexes.Add(i);
            }
        }

        // 一体も無い場合 → 全タイプからランダム
        if (candidateIndexes.Count == 0)
        {
            return UnityEngine.Random.Range(0, enemyDataList.Count);
        }

        // 指定タイプの敵からランダム
        return candidateIndexes[UnityEngine.Random.Range(0, candidateIndexes.Count)];
    }

}
