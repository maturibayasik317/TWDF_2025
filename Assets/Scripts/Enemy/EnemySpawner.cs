using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController enemyPrefab; // 敵のプレハブ
    [SerializeField] private PathData[] pathDataArray;    // 移動経路情報の配列
    [SerializeField] private GameManager gameManager;

    private Coroutine spawnCoroutine;
    // 「残りの敵」を管理する辞書
    private Dictionary<PathData, List<EnemySetting.EnemyData>> spawnPool
        = new Dictionary<PathData, List<EnemySetting.EnemyData>>();

    void Start()
    {
        BuildSpawnPool();
    }

    // PathData.spawnList → 実際の敵データリストへ変換
    private void BuildSpawnPool()
    {
        spawnPool.Clear();

        foreach (var path in pathDataArray)
        {
            List<EnemySetting.EnemyData> list = new List<EnemySetting.EnemyData>();

            foreach (var entry in path.spawnList)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    int id = DBManager.instance.enemySetting
                        .GetRandomEnemyIdByType(entry.enemyType);

                    list.Add(DBManager.instance.enemySetting.enemyDataList[id]);
                }
            }

            spawnPool[path] = list;
        }
    }

    // ====== 生成管理 ======
    public IEnumerator ManageSpawning()
    {
        while (!gameManager.isGameStarted)
            yield return null;

        int timer = 0;

        while (gameManager.isSpawning)
        {
            timer++;

            if (timer > gameManager.spawnInterval)
            {
                timer = 0;
                Spawn();
                gameManager.AddEnemyToList();
                gameManager.CheckSpawnLimit();
            }

            yield return null;
        }
    }

    // 敵 1 体スポーン
    public void Spawn()
    {
        // まだ出せる Path を絞る
        var availablePaths = spawnPool
            .Where(p => p.Value.Count > 0)
            .Select(p => p.Key)
            .ToList();

        if (availablePaths.Count == 0)
        {
            Debug.Log("全ての敵を出し切りました");
            gameManager.isSpawning = false;
            return;
        }

        // 重み付きランダムで Path を選ぶ
        PathData selectedPath = GetWeightedRandomPath(availablePaths.ToArray());

        // その Path の敵リストからランダム1体
        List<EnemySetting.EnemyData> enemyList = spawnPool[selectedPath];
        int index = UnityEngine.Random.Range(0, enemyList.Count);

        EnemySetting.EnemyData data = enemyList[index];
        enemyList.RemoveAt(index);   // ← 出したので減らす

        // 生成処理
        Vector3 pos = selectedPath.positionStart.position;
        EnemyController c = Instantiate(enemyPrefab, pos, Quaternion.identity);
        c.InitializeEnemy(selectedPath, gameManager, data);
    }


    // PathData 用の重み付きランダム
    private PathData GetWeightedRandomPath(PathData[] paths)
    {
        float total = 0f;
        foreach (var p in paths) total += Mathf.Max(0, p.spawnWeight);

        float r = UnityEngine.Random.Range(0, total);

        foreach (var p in paths)
        {
            float w = Mathf.Max(0f, p.spawnWeight);
            if (r < w) return p;
            r -= w;
        }

        return paths[paths.Length - 1];
    }

    // ====== 不要：敵の重み付きランダム（使っていないためコメントアウト） ======
    /*
    private EnemySetting.EnemyData GetRandomEnemyByWeight()
    {
        var enemySetting = DBManager.instance.enemySetting;
        if (enemySetting == null || enemySetting.enemyDataList.Count == 0)
            return null;

        float total = 0f;
        foreach (var e in enemySetting.enemyDataList)
            total += Mathf.Max(0f, e.weight);

        if (total <= 0f)
            return enemySetting.enemyDataList[Random.Range(0, enemySetting.enemyDataList.Count)];

        float r = Random.Range(0f, total);
        foreach (var e in enemySetting.enemyDataList)
        {
            float w = Mathf.Max(0f, e.weight);
            if (r < w) return e;
            r -= w;
        }
        return enemySetting.enemyDataList[0];
    }
    */
}
