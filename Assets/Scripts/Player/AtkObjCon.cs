using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtkObjCon : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f; // �C�e�̑��x
    private EnemyController targetEnemy; // �^�[�Q�b�g�ƂȂ�G
    private int attackPower; // �C�e�̍U����

    private void Update()
    {
        if (targetEnemy == null) // �^�[�Q�b�g�����Ȃ��ꍇ
        {
            Destroy(gameObject); // �C�e��j��
            return;
        }
        // �G�Ɍ������Ĉړ�
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// �C�e�̏�����
    /// </summary>
    public void Initialize(EnemyController enemy, int power)
    {
        targetEnemy = enemy;
        attackPower = power;
    }

    /// <summary>
    /// �C�e���G�ɓ��������Ƃ��̏���
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy))
        {
            // �Ώۂ̓G���ǂ����m�F
            if (enemy == targetEnemy)
            {
                enemy.CalcDamage(attackPower); // �_���[�W��^����
                Destroy(gameObject); // �C�e��j��
            }
        }
    }
}
