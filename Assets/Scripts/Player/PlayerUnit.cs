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

    [SerializeField] private int maxUnits = 5; // �ő�z�u��
    private List<GameObject> placedUnits = new List<GameObject>(); // �z�u�����j�b�g
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // �C��z�u�ς݃Z������

    private UnitSetting.UnitData selectedUnitData = null; // �I�����ꂽ�C��̃f�[�^

    void Update()
    {
        //���j�b�g�̔z�u
        if (Input.GetMouseButtonDown(0))
        {
            // �N���b�N�ł̔z�u����
            if (placedUnits.Count >= maxUnits)
            {
                Debug.Log($"�z�u����ɒB���Ă��܂��i{placedUnits.Count}/{maxUnits}�j");
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridHighPos = highWayTilemap.WorldToCell(mouseWorldPos);
            Vector3Int gridWayPos = wayTilemap.WorldToCell(mouseWorldPos);

            if (highWayTilemap.HasTile(gridHighPos))
            {
                GenerateTurret(gridHighPos, true); // ����
            }
            else if (wayTilemap.HasTile(gridWayPos))
            {
                GenerateTurret(gridWayPos, false); // �n��
            }
        }
    }

    // Unit����

    private void GenerateTurret(Vector3Int gridPos, bool isHighWay)
    {
        // Unit���I������Ă��Ȃ���Ή������Ȃ�
        if (selectedUnitData == null)
        {
            Debug.Log("Unit���I������Ă��܂���");
            return;
        }
        // �z�u�ς݂̏ꍇ�͏����𒆒f
        if (occupiedCells.Contains(gridPos))
        {
            Debug.Log("���̃Z���ɂ͂��ł�Unit���z�u����Ă��܂�");
            return;
        }

        // �ēx����`�F�b�N
        if (placedUnits.Count >= maxUnits)
        {
            Debug.Log($"�z�u����ɒB���Ă��܂��i{placedUnits.Count}/{maxUnits}�j");
            return;
        }
        // �N���b�N�����ʒu��Unit��z�u
        GameObject prefabToUse = selectedUnitData.unitPrefab;
        GameObject unit = Instantiate(prefabToUse, gridPos, Quaternion.identity);
        // Unit�̈ʒu���^�C���̍����� 0,0 �Ƃ��Đ������Ă���̂ŁA�^�C���̒����ɂ���悤�Ɉʒu�𒲐�
        unit.transform.position = new Vector2(unit.transform.position.x + 
                                                0.5f, unit.transform.position.y + 0.5f);

        // UnitBlock �R���|�[�l���g���擾���ď�����
        UnitBlock unitBlock = unit.GetComponent<UnitBlock>();
        if (unitBlock != null)
        {
            // �z�u�Z�����Z�b�g
            unitBlock.placedCell = gridPos;
            unitBlock.OnUnitDestroyed += OnUnitDestroyed;
        }
        //UnitAttack���擾
        UnitAttck unitAttck = unit.GetComponent<UnitAttck>();
        // Unit�f�[�^�̏�����
        unitAttck.InitializeUnit(selectedUnitData);
        // �z�u���ꂽ�Z����o�^
        placedUnits.Add(unit);
        occupiedCells.Add(gridPos);
        // Unit��ݒu������I�������Z�b�g
        selectedUnitData = null;
    }

    // Unit��I������
    public void SelectUnit(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.UnitDataList[index];
        Debug.Log($"{selectedUnitData.name} ��I��");
    }

    // ���j�b�g���j�󂳂ꂽ�Ƃ��� UnitBlock ����Ă΂��
    private void OnUnitDestroyed(UnitBlock unitBlock)
    {
        // null�`�F�b�N
        if (unitBlock == null) return;

        // �o�^�����i���S�̂��߁j
        unitBlock.OnUnitDestroyed -= OnUnitDestroyed;

        // occupiedCells �̊J��
        if (occupiedCells.Contains(unitBlock.placedCell))
        {
            occupiedCells.Remove(unitBlock.placedCell);
        }

        // placedUnits ���珜��
        placedUnits.Remove(unitBlock.gameObject);

        Debug.Log($"���j�b�g���j�󂳂�܂����B���݂̔z�u��: {placedUnits.Count}/{maxUnits}");
    }
}