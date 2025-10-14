using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    !!!ここにが10番にあるTurretGeneratoここを直せ
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
            Vector3Int gridPos = wayTilemap.WorldToCell(mouseWorldPos);
        }
    }

    // 砲台生成

    private void GenerateTurret(Vector3Int gridPos)
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
        GameObject turret = Instantiate(Prefab, gridPos, Quaternion.identity);
        // 砲台の位置がタイルの左下を 0,0 として生成しているので、タイルの中央にくるように位置を調整
        turret.transform.position = new Vector2(turret.transform.position.x + 0.5f, turret.transform.position.y + 0.5f);
        // TurretControllerを取得する
        AtkObjCon turretController = turret.GetComponent<AtkObjCon>();
        // 配置されたセルを登録
        occupiedCells.Add(gridPos);
        // 砲台を設置したら選択をリセット
        selectedUnitData = null;
    }

    // 砲台を選択する
    public void SelectTurret(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.turretDataList[index];
        Debug.Log($"{selectedUnitData.name} を選択");
    }
}