using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyController enemyPrefab; // 敵のプレハブ
    [SerializeField]
    private PathData[] pathDataArray; // 移動経路情報の配列
    [SerializeField]
    private GameManager gameManager;


    // 敵の生成管理
    public IEnumerator ManageSpawning()
    {
        while (!gameManager.isGameStarted)
        {
            yield return null;
        }

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
        // ランダムな経路を選択
        PathData selectedPath = pathDataArray[Random.Range(0, pathDataArray.Length)];

        // スタート地点プレハブから敵を生成
        EnemyController enemyController = Instantiate(enemyPrefab, selectedPath.positionStart.position, Quaternion.identity);

        // ランダムな敵を選択
        int enemyId = Random.Range(0, DBManager.instance.enemySetting.enemyDataList.Count);
        // 経路情報を初期化

        // 敵データの初期化
        enemyController.InitializeEnemy(selectedPath, gameManager, DBManager.instance.enemySetting.enemyDataList[enemyId]);

    }
}
