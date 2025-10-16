using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtkObjCon : MonoBehaviour
{
    [SerializeField]
    private int attackPower; // –C’e‚ÌUŒ‚—Í
    private float speed = 5.0f; // –C’e‚Ì‘¬“x
    private EnemyController targetEnemy; // ƒ^[ƒQƒbƒg‚Æ‚È‚é“G

    private void Update()
    {
        if (targetEnemy == null) // ƒ^[ƒQƒbƒg‚ª‚¢‚È‚¢ê‡
        {
            Destroy(gameObject); // –C’e‚ğ”j‰ó
            return;
        }
        // “G‚ÉŒü‚©‚Á‚ÄˆÚ“®
        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // –C’e‚Ì‰Šú‰»
    public void Initialize(EnemyController enemy, int power)
    {
        targetEnemy = enemy;
        attackPower = power;
    }

    // –C’e‚ª“G‚É“–‚½‚Á‚½‚Æ‚«‚Ìˆ—
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy))
        {
            // ‘ÎÛ‚Ì“G‚©‚Ç‚¤‚©Šm”F
            if (enemy == targetEnemy)
            {
                enemy.CalcDamage(attackPower); // ƒ_ƒ[ƒW‚ğ—^‚¦‚é
                Destroy(gameObject); // –C’e‚ğ”j‰ó
            }
        }
    }
}
