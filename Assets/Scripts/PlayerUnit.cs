using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    public Tilemap highWayTilemap;      // 高台Tilemap
    public Tilemap wayTilemap;          // 通路Tilemap
    public GameObject unitPrefab;       // 配置するユニットのPrefab

    public enum UnitType { HighWayUnit, WayUnit }
    public UnitType unitType = UnitType.WayUnit; // デフォルトは地面ユニット

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリック
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = wayTilemap.WorldToCell(mouseWorldPos);

            // 配置可能判定
            if (CanPlaceUnit(cellPos))
            {
                // ユニット設置
                Vector3 placePos = wayTilemap.GetCellCenterWorld(cellPos);
                Instantiate(unitPrefab, placePos, Quaternion.identity);
            }
        }
    }

    // 配置可能か判定
    bool CanPlaceUnit(Vector3Int cellPos)
    {
        if (unitType == UnitType.HighWayUnit)
        {
            return highWayTilemap.GetTile(cellPos) != null;
        }
        else if (unitType == UnitType.WayUnit)
        {
            return wayTilemap.GetTile(cellPos) != null;
        }
        return false;
    }
}