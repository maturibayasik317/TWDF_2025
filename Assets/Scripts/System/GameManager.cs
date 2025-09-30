using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = 60; // フレームレートの目標値


    [SerializeField] private EnemySpawner enemySpawner;
    public bool isSpawning; // 敵を生成するかどうかを制御するフラグ
    public int spawnInterval; // 敵を生成する間隔（単位はフレーム）
    public int spawnedEnemyCount; // これまでに生成された敵の数
    public int maxSpawnCount; // 敵の最大生成数
    void Awake()
    {
        FixFrameRate(); // フレームレートを固定
    }

    void Start()
    {
        isSpawning = true; // 敵を生成可能にする
        StartCoroutine(enemySpawner.ManageSpawning());
    }

    // フレームレートを固定
    private void FixFrameRate()
    {
        QualitySettings.vSyncCount = 0; // V-Sync（垂直同期）を無効化
        Application.targetFrameRate = targetFrameRate;
    }

    // 敵の情報をListに追加
    public void AddEnemyToList()
    {
        spawnedEnemyCount++; // 生成した敵の数を増やす
    }

    // 敵の生成が上限に達したかを確認
    public void CheckSpawnLimit()
    {
        if (spawnedEnemyCount >= maxSpawnCount) // 敵の最大生成数を超えたら
        {
            isSpawning = false; // 敵を生成不可にする
        }
    }
}
