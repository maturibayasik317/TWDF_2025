using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class PlayerUnit : MonoBehaviour
{
    public static PlayerUnit Instance { get; private set; }

    [Header("Tilemaps")]
    public Tilemap highWayTilemap;
    public Tilemap wayTilemap;
    [SerializeField] private GameObject highWayTurretPrefab;
    [SerializeField] private GameObject wayTurretPrefab;

    [SerializeField] private int maxUnits = 5; // 最大配置数
    private List<GameObject> placedUnits = new List<GameObject>(); // 配置中ユニット
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // 砲台配置済みセルを代入

    private UnitSetting.UnitData selectedUnitData = null; // 選択されたUnitのデータ
    private bool isPlacing = false; // 多重配置防止

    public event Action<int, int> OnPlacedCountChanged;
    // 現在の設置数を外部参照できるプロパティ
    public int CurrentPlacedCount => placedUnits.Count;
    // 最大設置数を外部参照できるプロパティ
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
        //ユニットの配置
        if (Input.GetMouseButtonDown(0) && !isPlacing)
        {
            // クリックでの配置処理
            if (placedUnits.Count >= maxUnits)
            {
                Debug.Log($"配置上限に達しています（{placedUnits.Count}/{maxUnits}）");
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridHighPos = highWayTilemap.WorldToCell(mouseWorldPos);
            Vector3Int gridWayPos = wayTilemap.WorldToCell(mouseWorldPos);

            if (highWayTilemap.HasTile(gridHighPos))
            {
                GenerateTurret(gridHighPos, true); // 高台
            }
            else if (wayTilemap.HasTile(gridWayPos))
            {
                GenerateTurret(gridWayPos, false); // 地面
            }
        }
    }

    private System.Collections.IEnumerator PlaceWithGuard(Vector3Int gridPos, bool isHighWay)
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

        // ここでも登録を試みる（ダブル登録を避けるため UnitBlock 側の登録を優先する設計でもよい）
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

        occupiedCells.Add(gridPos);
        selectedUnitData = null;
        isPlacing = false;
        yield break;
    }

    // Unit生成

    private void GenerateTurret(Vector3Int gridPos, bool isHighWay)
    {
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
        unit.transform.position = new Vector2(unit.transform.position.x + 
                                                0.5f, unit.transform.position.y + 0.5f);

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
        unitAttck.InitializeUnit(selectedUnitData);
        // 配置されたセルを登録
        placedUnits.Add(unit);
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