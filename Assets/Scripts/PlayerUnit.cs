using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    public Tilemap highWayTilemap;      // ����Tilemap
    public Tilemap wayTilemap;          // �ʘHTilemap
    public GameObject unitPrefab;       // �z�u���郆�j�b�g��Prefab

    public enum UnitType { HighWayUnit, WayUnit }
    public UnitType unitType = UnitType.WayUnit; // �f�t�H���g�͒n�ʃ��j�b�g

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���N���b�N
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = wayTilemap.WorldToCell(mouseWorldPos);

            // �z�u�\����
            if (CanPlaceUnit(cellPos))
            {
                // ���j�b�g�ݒu
                Vector3 placePos = wayTilemap.GetCellCenterWorld(cellPos);
                Instantiate(unitPrefab, placePos, Quaternion.identity);
            }
        }
    }

    // �z�u�\������
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