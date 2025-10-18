using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitBlock : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    private int hp;
    [SerializeField] private int blockCount = 2;
    private List<EnemyController> blockingEnemies = new List<EnemyController>();

    public Vector3Int placedCell; // PlayerUnit ���ݒ肷�� �j��ʒm
    public event Action<UnitBlock> OnUnitDestroyed;

    private bool registeredToPlayer = false;

    void Start()
    {
        hp = maxHp;

        // PlayerUnit ���V���O���g���ɂȂ��Ă���Ύ����o�^�����݂�
        if (PlayerUnit.Instance != null)
        {
            registeredToPlayer = PlayerUnit.Instance.RegisterPlacedUnit(gameObject);
            if (!registeredToPlayer)
            {
                Debug.LogWarning($"PlayerUnit �̓o�^�Ɏ��s���܂����i������߁j�B���̃��j�b�g��j�����܂�: {gameObject.name}");
                Destroy(gameObject);
                return;
            }
        }
    }

    // Data���珉�����iPlayerUnit����Ăԁj
    public void Initialize(UnitSetting.UnitData data)
    {
        if (data != null)
        {
            blockCount = Mathf.Max(0, data.blockCount);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.gameObject.TryGetComponent<EnemyController>(out enemy))
        {
            if (blockingEnemies.Count < blockCount && !blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this);
                Debug.Log("OnBlocked�Ăяo��: " + enemy.gameObject.name);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // blockingEnemies�̋󂫂��`�F�b�N
        if (blockingEnemies.Count < blockCount && collision.TryGetComponent(out EnemyController enemy))
        {
            if (!blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this);
                // EnemyController���Ɏ�����ʒm�i���j���ɌĂяo���Ă��炤�j
                enemy.OnDestroyedByBlock += OnEnemyDestroyed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �G���͈͊O�֏o���Ƃ��������i�K�v�Ȃ�j
        if (collision.TryGetComponent(out EnemyController enemy) && blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnReleased();
            enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
        }
    }

    public void OnEnemyKilled(EnemyController enemy)
    {
        if (blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
            Debug.Log("�G���j�ɂ��u���b�N����: " + enemy.gameObject.name);
        }
    }

    // ���j�b�g���j���ɌĂԁiDestroy���O�j
    private void OnDestroy()
    {
        // ���[�v���ɃR���N�V������ύX���Ȃ��悤 ToArray �ŃR�s�[���ė񋓂���
        foreach (var enemy in blockingEnemies.ToArray())
        {
            if (enemy != null)
            {
                enemy.OnReleased();
                // �C�x���g���o�^����Ă���Ή�������i���S�̂��� try-unsubscribe�j
                enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
                Debug.Log("���j�b�g�j��ɔ����u���b�N����: " + enemy.gameObject.name);
            }
        }
        // PlayerUnit �Ɏ����̓o�^�������˗�
        if (PlayerUnit.Instance != null && registeredToPlayer)
        {
            PlayerUnit.Instance.UnregisterPlacedUnit(gameObject);
            PlayerUnit.Instance.FreeOccupiedCell(placedCell);
        }
        blockingEnemies.Clear();
    }

    private void OnEnemyDestroyed(EnemyController enemy)
    {
        if (blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnDestroyedByBlock -= OnEnemyDestroyed;
        }
    }

    // �G����_���[�W���󂯂�
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} �c��HP: {hp}");
        if (hp <= 0)
        {
            // �e���֒ʒm���ēo�^����
            OnUnitDestroyed?.Invoke(this);

            // PlayerUnit �ւ��o�^�����𗊂�
            if (PlayerUnit.Instance != null && registeredToPlayer)
            {
                PlayerUnit.Instance.UnregisterPlacedUnit(gameObject);
                PlayerUnit.Instance.FreeOccupiedCell(placedCell);
            }

            Destroy(gameObject);
        }
    }

    public int CurrentBlockingCount
    {
        get { return blockingEnemies.Count; }
    }

    /// ���̃��j�b�g�̃u���b�N�\���i�O���m�F�p�j
    public int BlockCapacity
    {
        get { return blockCount; }
    }
}
