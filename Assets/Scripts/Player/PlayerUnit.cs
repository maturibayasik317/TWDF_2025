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

    [SerializeField] private int maxUnits = 5; // �ő�z�u��
    private List<GameObject> placedUnits = new List<GameObject>(); // �z�u�����j�b�g
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // �C��z�u�ς݃Z������

    private UnitSetting.UnitData selectedUnitData = null; // �I�����ꂽUnit�̃f�[�^
    private bool isPlacing = false; // ���d�z�u�h�~

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Scene �ɕ����� PlayerUnit �����݂��܂��B�Â��C���X�^���X��j�����܂��B");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        //���j�b�g�̔z�u
        if (Input.GetMouseButtonDown(0) && !isPlacing)
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

    private System.Collections.IEnumerator PlaceWithGuard(Vector3Int gridPos, bool isHighWay)
    {
        isPlacing = true;

        if (selectedUnitData == null)
        {
            Debug.Log("Unit���I������Ă��܂���");
            isPlacing = false;
            yield break;
        }

        if (occupiedCells.Contains(gridPos))
        {
            Debug.Log("���̃Z���ɂ͂��ł�Unit���z�u����Ă��܂�");
            isPlacing = false;
            yield break;
        }

        // �ēx����`�F�b�N�i���o�H�ő����Ă���\���j
        if (placedUnits.Count >= maxUnits)
        {
            Debug.Log($"�z�u����ɒB���Ă��܂��i{placedUnits.Count}/{maxUnits}�j");
            isPlacing = false;
            yield break;
        }

        GameObject prefabToUse = selectedUnitData.unitPrefab != null ? selectedUnitData.unitPrefab : (isHighWay ? highWayTurretPrefab : wayTurretPrefab);
        GameObject unit = Instantiate(prefabToUse, gridPos, Quaternion.identity);
        unit.transform.position = new Vector2(unit.transform.position.x + 0.5f, unit.transform.position.y + 0.5f);

        // �����ł��o�^�����݂�i�_�u���o�^������邽�� UnitBlock ���̓o�^��D�悷��݌v�ł��悢�j
        if (!RegisterPlacedUnit(unit))
        {
            Debug.Log("�o�^�Ɏ��s�������ߐ����������j�b�g��j�����܂��i������߁j");
            Destroy(unit);
            isPlacing = false;
            yield break;
        }

        // UnitAttck �������iselectedUnitData���K�v�j
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
            unitBlock.Initialize(selectedUnitData);
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

    // �O���iUnitBlock.Start �Ȃǁj����Ă΂��o�^API
    public bool RegisterPlacedUnit(GameObject unit)
    {
        if (unit == null) return false;
        if (placedUnits.Count >= maxUnits) return false;
        if (placedUnits.Contains(unit)) return true; // ���ɓo�^�ς݂Ȃ琬������
        placedUnits.Add(unit);
        Debug.Log($"���j�b�g�o�^: {unit.name} ���ݐ� {placedUnits.Count}/{maxUnits}");
        return true;
    }

    // ����API�i���j�b�g�̔j�󎞂ɌĂ΂��j
    public void UnregisterPlacedUnit(GameObject unit)
    {
        if (unit == null) return;
        if (placedUnits.Remove(unit))
        {
            Debug.Log($"���j�b�g�o�^����: {unit.name} ���ݐ� {placedUnits.Count}/{maxUnits}");
        }
    }

    // Unit��I������
    public void SelectUnit(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.UnitDataList[index];
        Debug.Log($"{selectedUnitData.name} ��I��");
    }

    // occupiedCells �̉���iUnitBlock ����Ăׂ���J���\�b�h�j
    public void FreeOccupiedCell(Vector3Int cell)
    {
        if (occupiedCells.Contains(cell))
        {
            occupiedCells.Remove(cell);
        }
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