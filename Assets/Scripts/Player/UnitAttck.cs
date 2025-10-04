using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttck : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 1; // �U����
    [SerializeField]
    private float attackInterval = 60.0f; // �U���Ԋu�i�P�ʂ̓t���[���j
    [SerializeField]
    private bool isAttack; // �U�����t���O
    [SerializeField]
    private EnemyController enemy; // �G

    [SerializeField] private int blockCount = 2; // �u���b�N���
    private List<EnemyController> blockingEnemies = new List<EnemyController>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        // �U�����ł͂Ȃ��ꍇ�ŁA���G�̏��𖢎擾�̏ꍇ
        if (!isAttack && !enemy)
        {
            Debug.Log("�G����");
            if (collision.gameObject.TryGetComponent(out enemy)) // �G�𔭌�������
            {
                // �U����Ԃɂ���
                isAttack = true;
                // �U���Ԋu�̊Ǘ�
                StartCoroutine(ManageAttacks());
            }
        }

        if (blockingEnemies.Count < blockCount)
        {
            EnemyController enemy;
            if (collision.gameObject.TryGetComponent(out enemy) && !blockingEnemies.Contains(enemy))
            {
                blockingEnemies.Add(enemy);
                enemy.OnBlocked(this); // �u���b�N�J�n
                Debug.Log($"�u���b�N�J�n: {enemy.gameObject.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        EnemyController enemy;
        if (collision.gameObject.TryGetComponent(out enemy))
        {
            Debug.Log("�G�Ȃ�");
            isAttack = false;
            enemy = null;
        }

        if (collision.gameObject.TryGetComponent(out enemy) && blockingEnemies.Contains(enemy))
        {
            blockingEnemies.Remove(enemy);
            enemy.OnReleased(); // �u���b�N����
            Debug.Log($"�u���b�N����: {enemy.gameObject.name}");
        }
    }

    // �U���Ԋu�Ǘ�
    public IEnumerator ManageAttacks()
    {
        Debug.Log("�U���Ԋu�Ǘ�");
        int timer = 0;
        // �U����Ԃ̊ԃ��[�v�������J��Ԃ�
        while (isAttack)
        {
            timer++;
            if (timer > attackInterval) // �ҋ@���Ԃ��o�߂�����
            {
                timer = 0; // �^�C�}�[�����Z�b�g
                Attack(); // �U��
            }
            yield return null; // 1�t���[�������𒆒f����
        }
    }

    // �U��
    private void Attack()
    {
        Debug.Log("�U��");
        enemy.CalcDamage(attackPower);
    }
}
