using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// StageFlowManager: 強化パネル表示・NEXT押下で次シーン読み込みを管理するクラス。
public class StageFlowManager : MonoBehaviour
{
    public static StageFlowManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject nextButtonObject;       // NEXTボタン（Inspectorで割当て）
    [SerializeField] private GameObject upgradePanelObject;     // 強化画面（Inspectorで割当て）

    [Header("Stage list")]
    [SerializeField] private string[] stageSceneNames; // シーン名配列
    private int currentStageIndex = 0;

    [Header("Timing")]
    [SerializeField] private float sceneLoadDelay = 0.25f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (nextButtonObject != null) nextButtonObject.SetActive(false);
        if (upgradePanelObject != null) upgradePanelObject.SetActive(false);

        // 現在のシーンに対応する index を設定
        var active = SceneManager.GetActiveScene().name;
        for (int i = 0; i < stageSceneNames.Length; i++)
        {
            if (stageSceneNames[i] == active) { currentStageIndex = i; break; }
        }
    }

    // GameManager から呼ばれる: ステージクリア時に強化フローを開始する（StageClear 表示自体は GameManager が行う）。
    public void StartUpgradeFlow()
    {
        // プレイヤーの操作を停止しておく（重複チェック）
        if (PlayerUnit.Instance != null)
            PlayerUnit.Instance.SetAllowPlacement(false);

        // 強化パネルを表示（UpgradeManager が panelRoot を制御している構成ならそちらを呼ぶ）
        if (upgradePanelObject != null)
            upgradePanelObject.SetActive(true);

        // NEXTボタンは強化完了後に表示されるフローにするか、ここで有効にするかはUI設計次第。
        if (nextButtonObject != null)
            nextButtonObject.SetActive(false); // 強化中はNEXTを隠す（UpgradeManager が終了時に有効化／StageFlowManager で遷移）
    }

    
    // UpgradeManager から呼ばれる（強化適用後）: 次シーンへ移動する
    public void ContinueToNextStage()
    {
        StartCoroutine(LoadNextStageCoroutine());
    }

    private IEnumerator LoadNextStageCoroutine()
    {
        if (upgradePanelObject != null) upgradePanelObject.SetActive(false);
        if (nextButtonObject != null) nextButtonObject.SetActive(false);

        yield return new WaitForSeconds(sceneLoadDelay);

        int nextIndex = currentStageIndex + 1;
        if (stageSceneNames == null || stageSceneNames.Length == 0)
        {
            Debug.LogWarning("StageFlowManager: stageSceneNames が未設定です。");
            yield break;
        }
        if (nextIndex >= stageSceneNames.Length)
        {
            Debug.Log("StageFlowManager: 最後のステージです。リザルト等に遷移してください。");
            yield break;
        }

        string nextScene = stageSceneNames[nextIndex];
        AsyncOperation ao = SceneManager.LoadSceneAsync(nextScene);
        while (!ao.isDone) yield return null;

        currentStageIndex = nextIndex;

        // シーンロード直後に PlayerUnit の初期化処理を呼ぶ
        if (PlayerUnit.Instance != null)
        {
            PlayerUnit.Instance.ClearAllUnits();
            PlayerUnit.Instance.ApplyPersistentUpgrades();
            PlayerUnit.Instance.SetAllowPlacement(false); // GameManager.StartGame のタイミングで許可
        }
    }
}