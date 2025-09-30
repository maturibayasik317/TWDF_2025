using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab; // 敵のプレハブ
    [SerializeField]
    private PathData pathData; // 移動経路情報
    [SerializeField]
    private GameManager gameManager;


    // 敵の生成管理
    public IEnumerator ManageSpawning()
    {
        int timer = 0; // タイマーの初期化
        while (gameManager.isSpawning) // 敵を生成可能ならば
        {
            timer++; // タイマー加算
            if (timer > gameManager.spawnInterval) // タイマーが敵生成間隔を超えたら
            {
                timer = 0; // タイマーリセット
                Spawn(); // 敵生成
                gameManager.AddEnemyToList(); // 敵の情報をListに追加
                gameManager.CheckSpawnLimit();  // 最大生成数を超えたら敵の生成停止
            }
            yield return null;
        }
    }

    // 敵の生成
    public void Spawn()
    {
        // スタート地点プレハブから敵を生成
        EnemyController enemyController = Instantiate(enemyPrefab, pathData.positionStart.position, Quaternion.identity);

    }
}
