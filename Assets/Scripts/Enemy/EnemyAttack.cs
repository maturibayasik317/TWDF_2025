using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackPower = 1; // �U����
    [SerializeField] private float attackInterval = 60.0f; // �U���Ԋu�i�t���[���j
    private bool isAttack = false;
    private UnitBlock targetUnit; // �U���Ώ�

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isAttack && !targetUnit)
        {
            if (collision.gameObject.TryGetComponent<UnitBlock>(out targetUnit))
            {
                isAttack = true;
                StartCoroutine(ManageAttacks());
                Debug.Log("�������j�b�g�����A�U���J�n");
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
                Debug.Log("�������j�b�g���͈͊O�ցA�U���I��");
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
            Debug.Log("�G�l�~�[�U���I");
            targetUnit.TakeDamage(attackPower);
        }
    }
}
