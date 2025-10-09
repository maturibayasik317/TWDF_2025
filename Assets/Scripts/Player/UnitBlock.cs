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

    private void OnTriggerExit2D(Collider2D collision)
    {
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
