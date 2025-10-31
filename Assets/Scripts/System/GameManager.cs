using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int targetFrameRate = 60; // フレームレートの目標値

    [Header("Spawner設定")]
    [SerializeField] private EnemySpawner enemySpawner;
    [Header("敵生成管理")]
    public bool isGameStarted = false;
    public bool isSpawning = false; // 敵を生成するかどうかを制御するフラグ
    public int spawnInterval; // 敵を生成する間隔（単位はフレーム）
    public int spawnedEnemyCount; // これまでに生成された敵の数
    public int maxSpawnCount; // 敵の最大生成数

    // 現在シーン上に存在する（生存している）敵の数
    private int aliveEnemyCount = 0;

    [Header("UI")]
    [SerializeField] private GameObject startButtonObject; // GameStart ボタンの GameObject を Inspector で割当て
    [SerializeField] private GameObject stageClearObject; // StageClear 表示用オブジェクト（Inspectorに割当て、初期は非表示）
    [SerializeField] private GameObject gameOverObject; // ゲームオーバーUI追加
    [SerializeField] private TextMeshProUGUI countdownText;

    private bool isGameOver = false; // ゲームオーバー判定フラグ
    void Awake()
    {
        // シングルトン初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FixFrameRate(); // フレームレートを固定

        // StageClearとGameOverとクールダウンテキストを非表示
        if (stageClearObject != null)
            stageClearObject.SetActive(false);
        if (gameOverObject != null)
            gameOverObject.SetActive(false);
        if (countdownText != null)
            countdownText.gameObject.SetActive(false); 
    }

    void Start()
    {
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
        if (isGameStarted) return;
        isGameStarted = true;
        isSpawning = false;
        startButtonObject.SetActive(false); // ← ボタンを非表示
        StartCoroutine(enemySpawner.ManageSpawning());

        spawnedEnemyCount = 0;
        aliveEnemyCount = 0;

        // ユニット配置を許可（PlayerUnit が存在する場合）
        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.SetAllowPlacement(true);
        }

        StartCoroutine(StartCountdown());
    }

    //ユニットの再配置までのクールダウン
    private IEnumerator StartCountdown()
    {
        // カウントダウン開始（3→2→1→Start!!）
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = "Start!!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        BeginBattle();
    }

    private void BeginBattle()
    {
        Debug.Log("GameManager: Battle Started!");
        isSpawning = true;
        spawnedEnemyCount = 0;
        aliveEnemyCount = 0;

        if (enemySpawner != null)
        {
            StartCoroutine(enemySpawner.ManageSpawning());
        }

        // ユニット配置を許可
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
        CheckSpawnLimit();
    }

    // 敵が死亡したときに呼ぶ（EnemyController.DestroyEnemy から呼ぶ）
    public void NotifyEnemyDestroyed(EnemyController enemy)
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
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
        }
    }

    // ステージクリア判定（スポーン終了かつ生存敵0）
    public void CheckStageClear()
    {
       // ゲームオーバー中は処理しない
        if (isGameOver) return;

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

    public void GameOver()
    {
        if (isGameOver) return; // 二重呼び出し防止
        isGameOver = true;

        Debug.Log("GameManager: Game Over!");
        isSpawning = false; // 敵のスポーン停止

        /* なぜか使えない？
        // 全ての敵を止めたい場合（任意）
        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.StopAllCoroutines(); // 敵の行動停止（EnemyControllerがCoroutineを使っている場合）
        }*/

        // GameOver UI表示
        if (gameOverObject != null)
        {
            gameOverObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("gameOverObject が Inspector に設定されていません。");
        }
    }


    // デバッグ用のゲッター（外部確認用）
    public int GetAliveEnemyCount() => aliveEnemyCount;
    public int GetSpawnedEnemyCount() => spawnedEnemyCount;
}
