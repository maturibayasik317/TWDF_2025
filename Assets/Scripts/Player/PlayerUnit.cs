using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap highWayTilemap;
    public Tilemap wayTilemap;
    [SerializeField] private GameObject highWayTurretPrefab;
    [SerializeField] private GameObject wayTurretPrefab;

    [SerializeField] private int maxUnits = 5; // 最大配置数
    private List<GameObject> placedUnits = new List<GameObject>(); // 配置中ユニット
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // 砲台配置済みセルを代入

    private UnitSetting.UnitData selectedUnitData = null; // 選択された砲台のデータ

    void Update()
    {
        //ユニットの配置
        if (Input.GetMouseButtonDown(0))
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

    // Unitを選択する
    public void SelectUnit(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.UnitDataList[index];
        Debug.Log($"{selectedUnitData.name} を選択");
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