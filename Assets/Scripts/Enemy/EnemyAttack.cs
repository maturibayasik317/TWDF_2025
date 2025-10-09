using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackPower = 1; // 攻撃力
    [SerializeField] private float attackInterval = 60.0f; // 攻撃間隔（フレーム）
    private bool isAttack = false;
    private UnitBlock targetUnit; // 攻撃対象

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isAttack && !targetUnit)
        {
            if (collision.gameObject.TryGetComponent<UnitBlock>(out targetUnit))
            {
                isAttack = true;
                StartCoroutine(ManageAttacks());
                Debug.Log("味方ユニット発見、攻撃開始");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UnitBlock unit;
        if (collision.gameObject.TryGetComponent<UnitBlock>(out unit))
        {
            if (unit == targetUnit)
            {
                isAttack = false;
                targetUnit = null;
                Debug.Log("味方ユニットが範囲外へ、攻撃終了");
            }
        }
    }

    private IEnumerator ManageAttacks()
    {
        int timer = 0;
        while (isAttack && targetUnit != null)
        {
            timer++;
            if (timer > attackInterval)
            {
                timer = 0;
                Attack();
            }
            yield return null;
        }
    }

    private void Attack()
    {
        if (targetUnit != null)
        {
            Debug.Log("エネミー攻撃！");
            targetUnit.TakeDamage(attackPower);
        }
    }
}
