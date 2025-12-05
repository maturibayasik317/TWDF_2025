using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathData : MonoBehaviour
{
    public Transform positionStart; // スタート地点
    public Transform[] pathArray;   // 敵の移動経路の配列
    public EnemySetting.EnemyType spawnEnemyType;
    public float spawnWeight = 1f;
    public bool isBossOnly;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
