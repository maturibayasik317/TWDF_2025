using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathData : MonoBehaviour
{
    public Transform positionStart; // スタート地点
    public Transform[] pathArray;   // 敵の移動経路の配列

    [Serializable]
    public class SpawnEntry
    {
        public EnemySetting.EnemyType enemyType;
        public int count = 1;   // このタイプを何体出すか
    }

    public List<SpawnEntry> spawnList = new List<SpawnEntry>();
    [Header("このルートが選ばれる確率の重みx/x合計")]
    public float spawnWeight = 1f;//例（上・中・下がそれぞれ１，３，１，の場合3/5で中から出てくる）
    public bool isBossOnly;//アタッチしているルートからボスが出るかどうか


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
