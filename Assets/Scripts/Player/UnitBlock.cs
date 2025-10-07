using UnityEngine;
using System.Collections.Generic;

public class UnitBlock : MonoBehaviour
{
    [SerializeField] private int blockCount = 2;
    private List<EnemyController> blockingEnemies = new List<EnemyController>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.gameObject.TryGetComponent<EnemyController>(out enemy))
        {
            // ������blockingEnemies���X�g�̏������`�F�b�N
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
        EnemyController enemy;
        if (collision.gameObject.TryGetComponent<EnemyController>(out enemy) && blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnReleased();
            Debug.Log("�u���b�N����: " + enemy.gameObject.name);
        }
    }
}
