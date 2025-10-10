using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitAttck : MonoBehaviour
{
    [SerializeField]
    private int attackPower = 1; // UŒ‚—Í
    [SerializeField]
    private float attackInterval = 60.0f; // UŒ‚ŠÔŠui’PˆÊ‚ÍƒtƒŒ[ƒ€j
    [SerializeField]
    private GameObject attackPrefab; // –C’e‚ÌƒvƒŒƒnƒu
    [SerializeField]
    private Transform firePoint; // UŒ‚‚Ì”­ËˆÊ’u
    private List<EnemyController> enemiesInRange = new List<EnemyController>(); // UŒ‚”ÍˆÍ“à‚Ì“GƒŠƒXƒg
    private EnemyController targetEnemy = null; // Œ»İ‚Ìƒ^[ƒQƒbƒg
    private bool isAttacking = false; // UŒ‚’†ƒtƒ‰ƒO
    private Coroutine attackCoroutine; // Œ»İ‚ÌUŒ‚ƒRƒ‹[ƒ`ƒ“

    private void Update()
    {
        UpdateTargetEnemy(); // Å‚à‹ß‚¢“G‚ğ’T‚·
    }

 /*   private void OnTriggerStay2D(Collider2D collision)
    {
        // UŒ‚’†‚Å‚Í‚È‚¢ê‡‚ÅA‚©‚Â“G‚Ìî•ñ‚ğ–¢æ“¾‚Ìê‡
        if (!isAttack && !enemy)

        {
            Debug.Log("“G”­Œ©");
            if (collision.gameObject.TryGetComponent(out enemy)) // “G‚ğ”­Œ©‚µ‚½‚ç
            {
                // UŒ‚ó‘Ô‚É‚·‚é
                isAttack = true;
                // UŒ‚ŠÔŠu‚ÌŠÇ—
                attackCoroutine = StartCoroutine(ManageAttacks());
            }
        }
    }
 */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy)) // N“ü‚µ‚Ä‚«‚½‚Ì‚ª“G‚È‚ç‚Î
        {
            // “GƒŠƒXƒg‚É’Ç‰Á
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }

            // UŒ‚‚µ‚Ä‚¢‚È‚¯‚ê‚ÎŠJn
            if (!isAttacking)
            {
                isAttacking = true;
                attackCoroutine = StartCoroutine(ManageAttacks());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyController enemy)) // o‚Ä‚¢‚Á‚½‚Ì‚ª“G‚È‚ç‚Î
        {
            enemiesInRange.Remove(enemy); // ƒŠƒXƒg‚©‚çíœ

            // ƒ^[ƒQƒbƒg‚ªo‚Äs‚Á‚½ê‡A•Ê‚Ìƒ^[ƒQƒbƒg‚ğ‘I‘ğ
            if (targetEnemy == enemy)
            {
                targetEnemy = null;
                UpdateTargetEnemy();
            }

            // ‚à‚µ”ÍˆÍ“à‚É“G‚ª‚¢‚È‚­‚È‚Á‚½‚çUŒ‚‚ğ‚â‚ß‚é
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

    //Å‚à‹ß‚¢“G‚ğ‘_‚¤
    private void UpdateTargetEnemy()
    {
        if (enemiesInRange.Count == 0) // UŒ‚”ÍˆÍ‚É“G‚ª‚¢‚È‚¯‚ê‚Î
        {
            targetEnemy = null; // ƒ^[ƒQƒbƒg‚È‚µ
            return;
        }
        float closestDistance = float.MaxValue; // Å‚à‹ß‚¢“G‚Ü‚Å‚Ì‹——£‚ÉÅ‘å’l‚ğ‘ã“ü
        EnemyController closestEnemy = null; // Å‚à‹ß‚¢“G
        // UŒ‚”ÍˆÍ“à‚Ì‚·‚×‚Ä‚Ì“G‚ğƒ`ƒFƒbƒN
        foreach (var enemy in enemiesInRange)
        {
            // –C‘ä‚Æ“G‚Ì‹——£‚ğŒvZ
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            // ‚æ‚è‹ß‚¢“G‚ğ‹L˜^
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        targetEnemy = closestEnemy; // Å‚à‹ß‚¢“G‚ğƒ^[ƒQƒbƒg‚Æ‚µ‚Äİ’è
    }

    // UŒ‚ŠÔŠuŠÇ—
    public IEnumerator ManageAttacks()
    {
          Debug.Log("UŒ‚ŠÔŠuŠÇ—");
        /* UŒ‚ó‘Ô‚ÌŠÔƒ‹[ƒvˆ—‚ğŒJ‚è•Ô‚·
        while (enemy != null && isAttack)
        {
            // UŒ‚‚ğÀs
            Attack();
            // Ÿ‚ÌUŒ‚‚Ü‚Å‘Ò‹@
            yield return new WaitForSeconds(attackInterval / 60.0f); // ƒtƒŒ[ƒ€ŠÔŠu‚ğ•b”‚É•ÏŠ·
        }
        isAttack = false; // UŒ‚‚ğI—¹
        attackCoroutine = null; // ƒRƒ‹[ƒ`ƒ“QÆ‚ğƒNƒŠƒA
        */
        while (isAttacking)
        {
            if (targetEnemy) // ƒ^[ƒQƒbƒg‚ª‘¶İ‚·‚éê‡
            {
                Attack(); // UŒ‚‚ğÀs
            }
            // Ÿ‚ÌUŒ‚‚Ü‚Å‘Ò‹@
            yield return new WaitForSeconds(attackInterval / 60.0f);
        }

    }

    // UŒ‚
    private void Attack()
    {
        Debug.Log("UŒ‚");
        if (!targetEnemy || !attackPrefab || !firePoint) return;
        // UŒ‚‚ğ¶¬
        GameObject shell = Instantiate(attackPrefab, firePoint.position, firePoint.rotation);
        // ShellController‚É“Gî•ñ‚ğ“n‚·
        AtkObjCon atkObjCon = shell.GetComponent<AtkObjCon>();
        if (atkObjCon)
        {
            atkObjCon.Initialize(targetEnemy, attackPower);
        }
    }
}
