using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap highWayTilemap; // ����Tilemap
    public Tilemap wayTilemap;     // �ʘHTilemap
    [SerializeField]
    private GameObject highWayTurretPrefab; // ���䃆�j�b�gPrefab
    [SerializeField]
    private GameObject wayTurretPrefab;     // �ʘH���j�b�gPrefab

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
    /// �^�C�������Ƀ��j�b�gPrefab�����i�Y���␳���܂ށj
    /// </summary>
    private void GenerateTurret(Vector3Int gridPos, GameObject prefab, Tilemap tilemap)
    {
        // �^�C���������W
        Vector3 worldPos = tilemap.GetCellCenterWorld(gridPos);

        // --- �����ŃY���␳���s�� ---
        // ��FPrefab��pivot�������Ȃ�A�^�C���̔�������X,Y���v���X����
        //      (Tilemap��Tile�T�C�Y��1�Ȃ� 0.5f, �^�C���T�C�Y���Ⴄ�ꍇ��tilemap.cellSize.x/y�Ŏ擾)
        Vector3 offset = Vector3.zero;

        // �^�C���T�C�Y�擾
        Vector3 tileSize = tilemap.cellSize;
        // SpriteRenderer��pivot�������Ȃ�offset�s�v�A�����Ȃ�␳
        // �K�v�ɉ����Ēl��ύX
        // offset = new Vector3(tileSize.x / 2f, tileSize.y / 2f, 0);

        // --- �␳��̍��W�Ő��� ---
        Instantiate(prefab, worldPos + offset, Quaternion.identity);
    }
}