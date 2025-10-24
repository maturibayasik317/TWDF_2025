using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int targetFrameRate = 60; // フレームレートの目標値


    [SerializeField] private EnemySpawner enemySpawner;
    public bool isSpawning; // 敵を生成するかどうかを制御するフラグ
    public int spawnInterval; // 敵を生成する間隔（単位はフレーム）
    public int spawnedEnemyCount; // これまでに生成された敵の数
    public int maxSpawnCount; // 敵の最大生成数

    // 現在シーン上に存在する（生存している）敵の数
    private int aliveEnemyCount = 0;

    [Header("UI")]
    [SerializeField] private GameObject startButtonObject; // GameStart ボタンの GameObject を Inspector で割当て
    [SerializeField] private GameObject stageClearObject; // StageClear 表示用オブジェクト（Inspectorに割当て、初期は非表示）
    
    void Awake()
    {
        // シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("複数の GameManager が存在します。古いインスタンスを破棄します。");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FixFrameRate(); // フレームレートを固定

        // StageClear非表示（
        if (stageClearObject != null)
        {
            stageClearObject.SetActive(false);
        }
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

    // ゲームを開始する（UIのGameStartボタンから呼ぶ）
    public void StartGame()
    {
        if (isSpawning)
        {
            Debug.Log("既にゲーム開始済みです");
            return;
        }

        Debug.Log("GameManager: StartGame called");
        if (startButtonObject != null)
        {
            startButtonObject.SetActive(false);
        }

        // 敵の生成を許可
        isSpawning = true;
        spawnedEnemyCount = 0;
        aliveEnemyCount = 0;

        if (enemySpawner != null)
        {
            StartCoroutine(enemySpawner.ManageSpawning());
        }
        else
        {
            Debug.LogWarning("EnemySpawner がセットされていません。Inspectorで enemySpawner を割り当ててください。");
        }

        // ユニット配置を許可（PlayerUnit が存在する場合）
        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.SetAllowPlacement(true);
        }
    }


    // 敵がスポーンしたときに呼ぶ（SpawnerまたはEnemyControllerの初期化から呼ぶ）
    public void RegisterSpawnedEnemy(EnemyController enemy)
    {
        spawnedEnemyCount++;
        aliveEnemyCount++;
        Debug.Log($"GameManager: RegisterSpawnedEnemy -> spawnedTotal={spawnedEnemyCount}, alive={aliveEnemyCount}");
        CheckSpawnLimit();
    }

    // 敵が死亡したときに呼ぶ（EnemyController.DestroyEnemy から呼ぶ）
    public void NotifyEnemyDestroyed(EnemyController enemy)
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        Debug.Log($"GameManager: NotifyEnemyDestroyed -> alive={aliveEnemyCount}");
        CheckStageClear();
    }

    // 敵の情報をListに追加
    public void AddEnemyToList()
    {
        spawnedEnemyCount++; // 生成した敵の数を増やす
    }

    // 敵の生成が上限に達したかを確認
    public void CheckSpawnLimit()
    {
        if (spawnedEnemyCount >= maxSpawnCount)
        {
            isSpawning = false;
            Debug.Log("GameManager: spawn limit reached, stopping spawn");
        }
    }

    // ステージクリア判定（スポーン終了かつ生存敵0）
    public void CheckStageClear()
    {
        // spawnedEnemyCount >= maxSpawnCount を見ることでスポーンが全て終わっているか確認する
        if (!isSpawning && spawnedEnemyCount >= maxSpawnCount && aliveEnemyCount <= 0)
        {
            ShowStageClear();
        }
    }

    // ステージクリア表示
    private void ShowStageClear()
    {
        Debug.Log("GameManager: Stage Clear!");
        if (stageClearObject != null)
        {
            stageClearObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("stageClearObject が Inspector に設定されていません。");
        }
    }

    // デバッグ用のゲッター（外部確認用）
    public int GetAliveEnemyCount() => aliveEnemyCount;
    public int GetSpawnedEnemyCount() => spawnedEnemyCount;
}
