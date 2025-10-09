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

    private void OnTriggerExit2D(Collider2D collision)
    {
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
