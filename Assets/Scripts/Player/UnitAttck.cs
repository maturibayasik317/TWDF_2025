using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttck : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 1; // 攻撃力
    [SerializeField]
    private float attackInterval = 60.0f; // 攻撃間隔（単位はフレーム）
    [SerializeField]
    private bool isAttack; // 攻撃中フラグ
    [SerializeField]
    private EnemyController enemy; // 敵

    [SerializeField] private int blockCount = 2; // ブロック上限
    private List<EnemyController> blockingEnemies = new List<EnemyController>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 攻撃中ではない場合で、かつ敵の情報を未取得の場合
        if (!isAttack && !enemy)
        {
            Debug.Log("敵発見");
            if (collision.gameObject.TryGetComponent(out enemy)) // 敵を発見したら
            {
                // 攻撃状態にする
                isAttack = true;
                // 攻撃間隔の管理
                StartCoroutine(ManageAttacks());
            }
        }

        if (blockingEnemies.Count < blockCount)
        {
            EnemyController enemy;
            if (collision.gameObject.TryGetComponent(out enemy) && !blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this); // ブロック開始
                Debug.Log($"ブロック開始: {enemy.gameObject.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        EnemyController enemy;
        if (collision.gameObject.TryGetComponent(out enemy))
        {
            Debug.Log("敵なし");
            isAttack = false;
            enemy = null;
        }

        if (collision.gameObject.TryGetComponent(out enemy) && blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnReleased(); // ブロック解除
            Debug.Log($"ブロック解除: {enemy.gameObject.name}");
        }
    }

    // 攻撃間隔管理
    public IEnumerator ManageAttacks()
    {
        Debug.Log("攻撃間隔管理");
        int timer = 0;
        // 攻撃状態の間ループ処理を繰り返す
        while (isAttack)
        {
            timer++;
            if (timer > attackInterval) // 待機時間が経過したら
            {
                timer = 0; // タイマーをリセット
                Attack(); // 攻撃
            }
            yield return null; // 1フレーム処理を中断する
        }
    }

    // 攻撃
    private void Attack()
    {
        Debug.Log("攻撃");
        enemy.CalcDamage(attackPower);
    }
}
