using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using TMPro;

public class PlayerUnit : MonoBehaviour
{
    public static PlayerUnit Instance { get; private set; }

    [Header("Tilemaps設定")]
    public Tilemap highWayTilemap;
    public Tilemap wayTilemap;
    [SerializeField] private GameObject highWayTurretPrefab;
    [SerializeField] private GameObject wayTurretPrefab;

    [Header("Unit 設定")]
    [SerializeField] private int maxUnits = 5; // 最大配置数
    [SerializeField] private float placementCooldown = 4f; // 配置インターバル（秒）


    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI cooldownText;

    private float placementTimer = 0f; //タイマー
    private bool isOnCooldown = false; //クールダウン中フラグ
    private List<GameObject> placedUnits = new List<GameObject>(); //配置中ユニット
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); //砲台配置済みセルを代入

    private UnitSetting.UnitData selectedUnitData = null; //選択されたUnitのデータ
    private bool isPlacing = false; //多重配置防止

    public event Action<int, int> OnPlacedCountChanged;
    // 現在の設置数を外部参照できるプロパティ
    public int CurrentPlacedCount => placedUnits.Count;
    // 最大設置数を外部参照できるプロパティ

    private bool allowPlacement = false; 
    public bool AllowPlacement => allowPlacement;
    // SetAllowPlacement は GameManager.StartGame から呼ぶ
    public void SetAllowPlacement(bool allowed)
    {
        allowPlacement = allowed;
        Debug.Log($"PlayerUnit: AllowPlacement set to {allowed}");
    }


    public int MaxUnits => maxUnits;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Scene に複数の PlayerUnit が存在します。古いインスタンスを破棄します。");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        // クールダウン更新（allowPlacement関係なく動作）
        if (isOnCooldown)
        {
            placementTimer -= Time.deltaTime;
            if (placementTimer <= 0f)
            {
                isOnCooldown = false;
                placementTimer = 0f;
                Debug.Log("ユニット配置が再び可能になりました。");
            }

            if (cooldownText != null)
                cooldownText.text = $"次の配置まで：{placementTimer:F1} 秒";
        }
        else
        {
            if (cooldownText != null)
                cooldownText.text = allowPlacement ? "設置可能！" : "待機中...";
        }

        // ここから下は allowPlacement が true でないと実行されない
        if (!allowPlacement)
            return;

        if (GameManager.Instance == null || !GameManager.Instance.isSpawning)
            return;

        // クールダウン中は配置できない
        if (isOnCooldown)
            return;

        // 配置処理
        if (Input.GetMouseButtonDown(0) && !isPlacing)
        {
            if (placedUnits.Count >= maxUnits)
            {
                Debug.Log($"配置上限に達しています（{placedUnits.Count}/{maxUnits}）");
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridHighPos = highWayTilemap.WorldToCell(mouseWorldPos);
            Vector3Int gridWayPos = wayTilemap.WorldToCell(mouseWorldPos);

            if (highWayTilemap.HasTile(gridHighPos))
                StartCoroutine(PlaceWithGuard(gridHighPos, true));
            else if (wayTilemap.HasTile(gridWayPos))
                StartCoroutine(PlaceWithGuard(gridWayPos, false));
        }
    }



    private IEnumerator PlaceWithGuard(Vector3Int gridPos, bool isHighWay)
    {
        isPlacing = true;

        if (selectedUnitData == null)
        {
            Debug.Log("Unitが選択されていません");
            isPlacing = false;
            yield break;
        }

        if (occupiedCells.Contains(gridPos))
        {
            Debug.Log("このセルにはすでにUnitが配置されています");
            isPlacing = false;
            yield break;
        }

        // 再度上限チェック（他経路で増えている可能性）
        if (placedUnits.Count >= maxUnits)
        {
            Debug.Log($"配置上限に達しています（{placedUnits.Count}/{maxUnits}）");
            isPlacing = false;
            yield break;
        }

        GameObject prefabToUse = selectedUnitData.unitPrefab != null ? selectedUnitData.unitPrefab : (isHighWay ? highWayTurretPrefab : wayTurretPrefab);
        GameObject unit = Instantiate(prefabToUse, gridPos, Quaternion.identity);
        unit.transform.position = new Vector2(unit.transform.position.x + 0.5f, unit.transform.position.y + 0.5f);

        // ここでも登録を試みる（他の生成経路からの生成でも一元管理できる）
        if (!RegisterPlacedUnit(unit))
        {
            Debug.Log("登録に失敗したため生成したユニットを破棄します（上限超過）");
            Destroy(unit);
            isPlacing = false;
            yield break;
        }

        // UnitAttck 初期化（selectedUnitDataが必要）
        UnitAttck unitAttck = unit.GetComponent<UnitAttck>();
        if (unitAttck != null)
        {
            unitAttck.InitializeUnit(selectedUnitData);
        }

        // UnitBlock があれば初期化（blockCount等）
        UnitBlock unitBlock = unit.GetComponent<UnitBlock>();
        if (unitBlock != null)
        {
            unitBlock.placedCell = gridPos;
            unitBlock.OnUnitDestroyed += OnUnitDestroyed; // PlayerUnit がユニット破壊通知を受け取る
            unitBlock.Initialize(selectedUnitData); // Data から blockCount などを設定
        }

        occupiedCells.Add(gridPos);
        selectedUnitData = null;
        isPlacing = false;

        //配置完了後にインターバル開始
        isOnCooldown = true;
        placementTimer = placementCooldown;
        Debug.Log($"ユニット配置後クールダウン開始: {placementCooldown}秒");

        //クールダウンUI更新
        if (cooldownText != null)
            cooldownText.text = $"次の配置まで：{placementCooldown:F1} 秒";

        yield break;
    }

    // Unit生成
    private void GenerateTurret(Vector3Int gridPos, bool isHighWay)
    {
        if (!allowPlacement)
        {
            Debug.Log("現在はユニットの配置が許可されていません");
            return;
        }

        // Unitが選択されていなければ何もしない
        if (selectedUnitData == null)
        {
            Debug.Log("Unitが選択されていません");
            return;
        }
        // 配置済みの場合は処理を中断
        if (occupiedCells.Contains(gridPos))
        {
            Debug.Log("このセルにはすでにUnitが配置されています");
            return;
        }

        // 再度上限チェック
        if (placedUnits.Count >= maxUnits)
        {
            Debug.Log($"配置上限に達しています（{placedUnits.Count}/{maxUnits}）");
            return;
        }
        // クリックした位置にUnitを配置
        GameObject prefabToUse = selectedUnitData.unitPrefab;
        GameObject unit = Instantiate(prefabToUse, gridPos, Quaternion.identity);
        // Unitの位置がタイルの左下を 0,0 として生成しているので、タイルの中央にくるように位置を調整
        unit.transform.position = new Vector2(unit.transform.position.x + 0.5f, unit.transform.position.y + 0.5f);

        // UnitBlock コンポーネントを取得して初期化
        UnitBlock unitBlock = unit.GetComponent<UnitBlock>();
        if (unitBlock != null)
        {
            // 配置セルをセット
            unitBlock.placedCell = gridPos;
            unitBlock.OnUnitDestroyed += OnUnitDestroyed;
            unitBlock.Initialize(selectedUnitData);
        }
        //UnitAttackを取得
        UnitAttck unitAttck = unit.GetComponent<UnitAttck>();
        // Unitデータの初期化
        if (unitAttck != null)
        {
            unitAttck.InitializeUnit(selectedUnitData);
        }
        // 配置されたセルを登録（ここでも登録）
        placedUnits.Add(unit);
        // イベント通知（UIなどが購読）
        OnPlacedCountChanged?.Invoke(placedUnits.Count, maxUnits);

        occupiedCells.Add(gridPos);
        // Unitを設置したら選択をリセット
        selectedUnitData = null;
    }

    // 外部（UnitBlock.Start など）から呼ばれる登録API
    public bool RegisterPlacedUnit(GameObject unit)
    {
        if (unit == null) return false;
        if (placedUnits.Count >= maxUnits) return false;
        if (placedUnits.Contains(unit)) return true; // 既に登録済みなら成功扱い
        placedUnits.Add(unit);
        Debug.Log($"ユニット登録: {unit.name} 現在数 {placedUnits.Count}/{maxUnits}");
        return true;
    }

    // 解除API（ユニットの破壊時に呼ばれる）
    public void UnregisterPlacedUnit(GameObject unit)
    {
        if (unit == null) return;
        if (placedUnits.Remove(unit))
        {
            Debug.Log($"ユニット登録解除: {unit.name} 現在数 {placedUnits.Count}/{maxUnits}");
        }
    }

    // Unitを選択する
    public void SelectUnit(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.UnitDataList[index];
        Debug.Log($"{selectedUnitData.name} を選択");
    }

    // occupiedCells の解放（UnitBlock から呼べる公開メソッド）
    public void FreeOccupiedCell(Vector3Int cell)
    {
        if (occupiedCells.Contains(cell))
        {
            occupiedCells.Remove(cell);
        }
    }

    // ユニットが破壊されたときに UnitBlock から呼ばれる
    private void OnUnitDestroyed(UnitBlock unitBlock)
    {
        // nullチェック
        if (unitBlock == null) return;

        // 登録解除（安全のため）
        unitBlock.OnUnitDestroyed -= OnUnitDestroyed;

        // occupiedCells の開放
        if (occupiedCells.Contains(unitBlock.placedCell))
        {
            occupiedCells.Remove(unitBlock.placedCell);
        }

        // placedUnits から除去
        placedUnits.Remove(unitBlock.gameObject);

        Debug.Log($"ユニットが破壊されました。現在の配置数: {placedUnits.Count}/{maxUnits}");
    }
}