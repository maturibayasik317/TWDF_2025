using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Stage -> Next Scene")]
    [Tooltip("ステージクリア時に表示する NEXT ボタン (StageClear UI の子でも可)")]
    [SerializeField] public GameObject nextButtonObject;
    [Tooltip("次のシーンをビルドインデックスで進めるなら true。false の場合 nextSceneName を使う")]
    [SerializeField] private bool useBuildIndexForNext = true;
    [Tooltip("useBuildIndexForNext が false の場合の次のシーン名")]
    [SerializeField] private string nextSceneName = "Scene2";

    private bool isGameOver = false; // ゲームオーバー判定フラグ
    private Coroutine spawningCoroutine;

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
        // nextButtonObject を Inspector で割り当てている想定：初期は非表示にしておく
        if (nextButtonObject != null)
            nextButtonObject.SetActive(false);
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

        // 注意: ManageSpawning は BeginBattle() で開始するようにしました。
        // StartGame 内で先に StartCoroutine してしまうと、カウントダウン前にスポーンが始まる等、
        // 状態判定がずれる原因になります。

        spawnedEnemyCount = 0;
        aliveEnemyCount = 0;

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
            // 既に同じコルーチンが動いている可能性があれば停止してから再開
            if (spawningCoroutine != null)
                StopCoroutine(spawningCoroutine);
            spawningCoroutine = StartCoroutine(enemySpawner.ManageSpawning());
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
        // 注意: このメソッドが RegisterSpawnedEnemy と併用されると spawnedEnemyCount が二重増加する可能性があります。
        // 必要なら整理してください（例: AddEnemyToList を使わない、または RegisterSpawnedEnemy に統一する）。
        spawnedEnemyCount++; // 生成した敵の数を増やす
    }

    // 敵の生成が上限に達したかを確認
    public void CheckSpawnLimit()
    {
        if (spawnedEnemyCount >= maxSpawnCount)
        {
            isSpawning = false;
            Debug.Log($"CheckSpawnLimit: reached max. spawned={spawnedEnemyCount} max={maxSpawnCount}");
        }
    }

    // ステージクリア判定（スポーン終了かつ生存敵0）
    public void CheckStageClear()
    {
        // ゲームオーバー中は処理しない
        if (isGameOver) return;

        Debug.Log($"CheckStageClear: isSpawning={isSpawning} spawned={spawnedEnemyCount} max={maxSpawnCount} alive={aliveEnemyCount}");

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
        // 敵や生成を確実に止めたい場合はスパナーに停止メソッドを実装して呼ぶのが確実
        if (enemySpawner != null)
        {
        }

        // ユニット配置を禁止
        if (PlayerUnit.Instance != null)
            PlayerUnit.Instance.SetAllowPlacement(false);

        if (stageClearObject != null)
        {
            stageClearObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("stageClearObject が Inspector に設定されていません。");
        }

        // 強化ポップアップを表示
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.ShowUpgradePopup();
        }
        else
        {
            Debug.LogWarning("UpgradeManager.Instance が存在しません。");
            // 強化なしでNEXTボタンだけ表示
            if (nextButtonObject != null)
                nextButtonObject.SetActive(true);
        }

        // NEXTボタンを明示的に表示する（Inspectorに割り当てている想定）
        if (nextButtonObject != null)
        {
            nextButtonObject.SetActive(true);
            Debug.Log("Next button activated.");
        }
        else
        {
            // nextButtonObject が null の場合、stageClearObject の中に "Next" ボタンがあるか探してみる
            if (stageClearObject != null)
            {
                var nextChild = stageClearObject.transform.Find("NextButton");
                if (nextChild != null)
                {
                    nextChild.gameObject.SetActive(true);
                    Debug.Log("Found NextButton as child of stageClearObject and activated it.");
                }
                else
                {
                    Debug.LogWarning("nextButtonObject が設定されていません。stageClearObject の子に 'NextButton' があるか確認してください。");
                }
            }
        }
    }

    // NEXT ボタンにアタッチするか、ボタンの OnClick に直接設定するメソッド
    public void OnNextButtonPressed()
    {
        // ボタン連打防止
        if (nextButtonObject != null)
            nextButtonObject.SetActive(false);

        // シーン移動を開始
        StartCoroutine(LoadNextSceneRoutine());
    }

    // 次のシーンをロードする（非同期、フェードやロード画面をここで追加できます）
    private IEnumerator LoadNextSceneRoutine()
    {
        // オプション: フェードアウト等をここで待つ
        // yield return StartCoroutine(FadeOutRoutine());

        if (useBuildIndexForNext)
        {
            int current = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = current + 1;

            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning("次のシーンがビルド設定に存在しません。MainMenuに戻ります。");
                // 末尾ならメインメニューに戻す、名前はプロジェクトに合わせて変更
                SceneManager.LoadScene("MainMenu");
                yield break;
            }
            else
            {
                Debug.Log("Loading next scene by build index: " + nextIndex);
                AsyncOperation op = SceneManager.LoadSceneAsync(nextIndex);
                op.allowSceneActivation = true;
                while (!op.isDone)
                    yield return null;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(nextSceneName))
            {
                Debug.LogError("nextSceneName が空です。ビルドインデックス方式を使うか、名前を設定してください。");
                yield break;
            }

            Debug.Log("Loading next scene by name: " + nextSceneName);
            AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);
            op.allowSceneActivation = true;
            while (!op.isDone)
                yield return null;
        }

        // ここに来たら新しいシーンがロード済み
        // 必要なら GameManager の状態をリセットする（新しいステージで継続する場合）
        ResetForNewScene();
    }

    private void ResetForNewScene()
    {
        // 新しいシーンで GameManager を残す場合は状態を初期化
        isGameStarted = false;
        isSpawning = false;
        isGameOver = false;
        spawnedEnemyCount = 0;
        aliveEnemyCount = 0;

        if (stageClearObject != null)
            stageClearObject.SetActive(false);
        if (gameOverObject != null)
            gameOverObject.SetActive(false);
        if (nextButtonObject != null)
            nextButtonObject.SetActive(false);
        if (startButtonObject != null)
            startButtonObject.SetActive(true);
    }


    public void GameOver()
    {
        if (isGameOver) return; // 二重呼び出し防止
        isGameOver = true;

        Debug.Log("GameManager: Game Over!");
        isSpawning = false; // 敵のスポーン停止

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