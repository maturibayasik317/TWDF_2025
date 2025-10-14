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
    private GameObject attackPrefab; // �C�e�̃v���n�u
    [SerializeField]
    private Transform firePoint; // �U���̔��ˈʒu
    [SerializeField]
    private CircleCollider2D attackRange; //�U���͈͂̃R���C�_�[
    [SerializeField]
    private SpriteRenderer turretHeadSpriteRenderer; // �C�g��SpriteRenderer

    private List<EnemyController> enemiesInRange = new List<EnemyController>(); // �U���͈͓��̓G���X�g
    private EnemyController targetEnemy = null; // ���݂̃^�[�Q�b�g
    private bool isAttacking = false; // �U�����t���O
    private Coroutine attackCoroutine; // ���݂̍U���R���[�`��

    private void Update()
    {
        UpdateTargetEnemy(); // �ł��߂��G��T��
    }

 /*   private void OnTriggerStay2D(Collider2D collision)
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
                attackCoroutine = StartCoroutine(ManageAttacks());
            }
        }
    }
 */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy)) // �N�����Ă����̂��G�Ȃ��
        {
            // �G���X�g�ɒǉ�
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }

            // �U�����Ă��Ȃ���ΊJ�n
            if (!isAttacking)
            {
                isAttacking = true;
                attackCoroutine = StartCoroutine(ManageAttacks());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy)) // �o�Ă������̂��G�Ȃ��
        {
            enemiesInRange.Remove(enemy); // ���X�g����폜

            // �^�[�Q�b�g���o�čs�����ꍇ�A�ʂ̃^�[�Q�b�g��I��
            if (targetEnemy == enemy)
            {
                targetEnemy = null;
                UpdateTargetEnemy();
            }

            // �����͈͓��ɓG�����Ȃ��Ȃ�����U������߂�
            if (enemiesInRange.Count == 0)
            {
                isAttacking = false;
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
        }
    }

    // �C��f�[�^��������
    public void InitializeUnit(UnitSetting.UnitData unitData)
    {
        attackPower = unitData.attackPower; // �U���͂�ݒ�
        attackInterval = unitData.attackInterval; // �U���Ԋu��ݒ�
        attackRange.radius = unitData.attackRange; // �U���͈͂�ݒ�
        Debug.Log($"�������ꂽ�C��: {unitData.name}");
    }

    //�ł��߂��G��_��
    private void UpdateTargetEnemy()
    {
        if (enemiesInRange.Count == 0) // �U���͈͂ɓG�����Ȃ����
        {
            targetEnemy = null; // �^�[�Q�b�g�Ȃ�
            return;
        }
        float closestDistance = float.MaxValue; // �ł��߂��G�܂ł̋����ɍő�l����
        EnemyController closestEnemy = null; // �ł��߂��G
        // �U���͈͓��̂��ׂĂ̓G���`�F�b�N
        foreach (var enemy in enemiesInRange)
        {
            // �C��ƓG�̋������v�Z
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            // ���߂��G���L�^
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        targetEnemy = closestEnemy; // �ł��߂��G���^�[�Q�b�g�Ƃ��Đݒ�
    }

    // �U���Ԋu�Ǘ�
    public IEnumerator ManageAttacks()
    {
          Debug.Log("�U���Ԋu�Ǘ�");
        /* �U����Ԃ̊ԃ��[�v�������J��Ԃ�
        while (enemy != null && isAttack)
        {
            // �U�������s
            Attack();
            // ���̍U���܂őҋ@
            yield return new WaitForSeconds(attackInterval / 60.0f); // �t���[���Ԋu��b���ɕϊ�
        }
        isAttack = false; // �U�����I��
        attackCoroutine = null; // �R���[�`���Q�Ƃ��N���A
        */
        while (isAttacking)
        {
            if (targetEnemy) // �^�[�Q�b�g�����݂���ꍇ
            {
                Attack(); // �U�������s
            }
            // ���̍U���܂őҋ@
            yield return new WaitForSeconds(attackInterval / 60.0f);
        }

    }

    // �U��
    private void Attack()
    {
        Debug.Log("�U��");
        if (!targetEnemy || !attackPrefab || !firePoint) return;
        // �U���𐶐�
        GameObject shell = Instantiate(attackPrefab, firePoint.position, firePoint.rotation);
        // ShellController�ɓG����n��
        AtkObjCon atkObjCon = shell.GetComponent<AtkObjCon>();
        if (atkObjCon)
        {
            atkObjCon.Initialize(targetEnemy, attackPower);
        }
    }
}
