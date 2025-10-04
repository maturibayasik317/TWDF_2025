using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap highWayTilemap;
    public Tilemap wayTilemap;
    [SerializeField] private GameObject highWayTurretPrefab;
    [SerializeField] private GameObject wayTurretPrefab;

    [Header("Offset Settings")]
    public Vector2 highWayOffset = new Vector2(-2.4836f, 0.07333f);
    public Vector2 wayOffset = new Vector2(-3.14f, -0.43f);

    void Update()
    {
        //ユニットの配置
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridHighPos = highWayTilemap.WorldToCell(mouseWorldPos);
            Vector3Int gridPos = wayTilemap.WorldToCell(mouseWorldPos);

            if (highWayTilemap.GetTile(gridHighPos) != null)
            {
                GenerateTurret(gridHighPos, highWayTurretPrefab, highWayTilemap, highWayOffset);
            }
            else if (wayTilemap.GetTile(gridPos) != null)
            {
                GenerateTurret(gridPos, wayTurretPrefab, wayTilemap, wayOffset);
            }
        }
    }

    private void GenerateTurret(Vector3Int gridPos, GameObject prefab, Tilemap tilemap, Vector2 offset)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(gridPos);
        Instantiate(prefab, worldPos + (Vector3)offset, Quaternion.identity);
    }
}