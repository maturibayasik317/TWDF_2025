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
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // 砲台配置済みセルを代入

    private UnitSetting.UnitData selectedUnitData = null; // 選択された砲台のデータ

    void Update()
    {
        //ユニットの配置
        if (Input.GetMouseButtonDown(0))
        {
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

    // 砲台生成

    private void GenerateTurret(Vector3Int gridPos, bool isHighWay)
    {
        // 砲台が選択されていなければ何もしない
        if (selectedUnitData == null)
        {
            Debug.Log("砲台が選択されていません");
            return;
        }
        // 配置済みの場合は処理を中断
        if (occupiedCells.Contains(gridPos))
        {
            Debug.Log("このセルにはすでに砲台が配置されています");
            return;
        }
        // クリックした位置に砲台を配置
        GameObject prefabToUse = selectedUnitData.unitPrefab;
        GameObject unit = Instantiate(prefabToUse, gridPos, Quaternion.identity);
        // 砲台の位置がタイルの左下を 0,0 として生成しているので、タイルの中央にくるように位置を調整
        unit.transform.position = new Vector2(unit.transform.position.x + 
                                                0.5f, unit.transform.position.y + 0.5f);
        //UnitAttackを取得
        UnitAttck unitAttck = unit.GetComponent<UnitAttck>();
        // 砲台データの初期化
        unitAttck.InitializeUnit(selectedUnitData);
        // 配置されたセルを登録
        occupiedCells.Add(gridPos);
        // 砲台を設置したら選択をリセット
        selectedUnitData = null;
    }

    // Unitを選択する
    public void SelectUnit(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.UnitDataList[index];
        Debug.Log($"{selectedUnitData.name} を選択");
    }
}