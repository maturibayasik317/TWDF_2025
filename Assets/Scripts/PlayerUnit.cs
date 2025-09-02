using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap highWayTilemap; // 高台Tilemap
    public Tilemap wayTilemap;     // 通路Tilemap
    [SerializeField]
    private GameObject highWayTurretPrefab; // 高台ユニットPrefab
    [SerializeField]
    private GameObject wayTurretPrefab;     // 通路ユニットPrefab

    private Vector3Int gridPos;
    private Vector3Int gridHighPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gridHighPos = highWayTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            gridPos = wayTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
 
            if (highWayTilemap.GetTile(gridHighPos) != null)
            {
                GenerateTurret(gridHighPos);
            }
            else if (wayTilemap.GetTile(gridPos) != null)
            {
                GenerateTurret(gridPos);
            }
        }
    }

    /// <summary>
    /// タイル中央にユニットPrefab生成（ズレ補正も含む）
    /// </summary>
    private void GenerateTurret(Vector3Int gridPos, GameObject prefab, Tilemap tilemap)
    {
        // タイル中央座標
        Vector3 worldPos = tilemap.GetCellCenterWorld(gridPos);

        // --- ここでズレ補正を行う ---
        // 例：Prefabのpivotが左下なら、タイルの半分だけX,Yをプラスする
        //      (TilemapのTileサイズが1なら 0.5f, タイルサイズが違う場合はtilemap.cellSize.x/yで取得)
        Vector3 offset = Vector3.zero;

        // タイルサイズ取得
        Vector3 tileSize = tilemap.cellSize;
        // SpriteRendererのpivotが中央ならoffset不要、左下なら補正
        // 必要に応じて値を変更
        // offset = new Vector3(tileSize.x / 2f, tileSize.y / 2f, 0);

        // --- 補正後の座標で生成 ---
        Instantiate(prefab, worldPos + offset, Quaternion.identity);
    }
}