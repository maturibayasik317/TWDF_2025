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
                Debug.Log("OnBlocked呼び出し: " + enemy.gameObject.name);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // blockingEnemiesの空きをチェック
        if (blockingEnemies.Count < blockCount && collision.TryGetComponent(out EnemyController enemy))
        {
            if (!blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this);
                // EnemyController側に自分を通知（撃破時に呼び出してもらう）
                enemy.OnDestroyedByBlock += OnEnemyDestroyed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 敵が範囲外へ出たときも解除（必要なら）
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
            Debug.Log("敵撃破によるブロック解除: " + enemy.gameObject.name);
        }
    }

    // ユニット撃破時に呼ぶ（Destroy直前）
    private void OnDestroy()
    {
        foreach (var enemy in blockingEnemies)
        {
            if (enemy != null)
            {
                enemy.OnReleased();
                Debug.Log("ユニット破壊に伴うブロック解除: " + enemy.gameObject.name);
            }
        }
        blockingEnemies.Clear();
    }

    private void OnEnemyDestroyed(EnemyController enemy)
    {
        if (blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            // 必要に応じてブロック解除処理など追加
        }
    }

    // 敵からダメージを受ける
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 残りHP: {hp}");
        if (hp <= 0)
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} が撃破されました");
        }
    }
}
