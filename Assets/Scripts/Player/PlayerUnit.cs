using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerUnit : MonoBehaviour
{
    !!!�����ɂ�10�Ԃɂ���TurretGenerato�����𒼂�
    [Header("Tilemaps")]
    public Tilemap highWayTilemap;
    public Tilemap wayTilemap;
    [SerializeField] private GameObject highWayTurretPrefab;
    [SerializeField] private GameObject wayTurretPrefab;
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>(); // �C��z�u�ς݃Z������

    private UnitSetting.UnitData selectedUnitData = null; // �I�����ꂽ�C��̃f�[�^

    void Update()
    {
        //���j�b�g�̔z�u
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridHighPos = highWayTilemap.WorldToCell(mouseWorldPos);
            Vector3Int gridPos = wayTilemap.WorldToCell(mouseWorldPos);
        }
    }

    // �C�䐶��

    private void GenerateTurret(Vector3Int gridPos)
    {
        // �C�䂪�I������Ă��Ȃ���Ή������Ȃ�
        if (selectedUnitData == null)
        {
            Debug.Log("�C�䂪�I������Ă��܂���");
            return;
        }
        // �z�u�ς݂̏ꍇ�͏����𒆒f
        if (occupiedCells.Contains(gridPos))
        {
            Debug.Log("���̃Z���ɂ͂��łɖC�䂪�z�u����Ă��܂�");
            return;
        }
        // �N���b�N�����ʒu�ɖC���z�u
        GameObject turret = Instantiate(Prefab, gridPos, Quaternion.identity);
        // �C��̈ʒu���^�C���̍����� 0,0 �Ƃ��Đ������Ă���̂ŁA�^�C���̒����ɂ���悤�Ɉʒu�𒲐�
        turret.transform.position = new Vector2(turret.transform.position.x + 0.5f, turret.transform.position.y + 0.5f);
        // TurretController���擾����
        AtkObjCon turretController = turret.GetComponent<AtkObjCon>();
        // �z�u���ꂽ�Z����o�^
        occupiedCells.Add(gridPos);
        // �C���ݒu������I�������Z�b�g
        selectedUnitData = null;
    }

    // �C���I������
    public void SelectTurret(int index)
    {
        selectedUnitData = DBManager.instance.unitSetting.turretDataList[index];
        Debug.Log($"{selectedUnitData.name} ��I��");
    }
}