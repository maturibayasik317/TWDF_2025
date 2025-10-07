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
            // ここでblockingEnemiesリストの条件をチェック
            if (blockingEnemies.Count < blockCount && !blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this);
                Debug.Log("OnBlocked呼び出し: " + enemy.gameObject.name);
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
            Debug.Log("ブロック解除: " + enemy.gameObject.name);
        }
    }
}
