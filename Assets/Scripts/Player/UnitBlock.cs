using UnityEngine;
using System.Collections.Generic;

public class UnitBlock : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    private int hp;
    [SerializeField] private int blockCount = 2;
    private List<EnemyController> blockingEnemies = new List<EnemyController>();

    void Start()
    {
        hp = maxHp;
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
            Debug.Log("�G���j�ɂ��u���b�N����: " + enemy.gameObject.name);
        }
    }

    // ���j�b�g���j���ɌĂԁiDestroy���O�j
    private void OnDestroy()
    {
        foreach (var enemy in blockingEnemies)
        {
            if (enemy != null)
            {
                enemy.OnReleased();
                Debug.Log("���j�b�g�j��ɔ����u���b�N����: " + enemy.gameObject.name);
            }
        }
        blockingEnemies.Clear();
    }

    private void OnEnemyDestroyed(EnemyController enemy)
    {
        if (blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            // �K�v�ɉ����ău���b�N���������Ȃǒǉ�
        }
    }

    // �G����_���[�W���󂯂�
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} �c��HP: {hp}");
        if (hp <= 0)
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} �����j����܂���");
        }
    }
}
