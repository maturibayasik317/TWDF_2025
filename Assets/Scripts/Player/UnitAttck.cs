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
    private bool isAttack; // UŒ‚’†ƒtƒ‰ƒO
    [SerializeField]
    private EnemyController enemy; // “G

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out enemy))
        {
            Debug.Log("“G‚È‚µ");
            isAttack = false;
            enemy = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
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
                StartCoroutine(ManageAttacks());
            }
        }
    }

    // UŒ‚ŠÔŠuŠÇ—
    public IEnumerator ManageAttacks()
    {
        Debug.Log("UŒ‚ŠÔŠuŠÇ—");
        int timer = 0;
        // UŒ‚ó‘Ô‚ÌŠÔƒ‹[ƒvˆ—‚ğŒJ‚è•Ô‚·
        while (isAttack)
        {
            timer++;
            if (timer > attackInterval) // ‘Ò‹@ŠÔ‚ªŒo‰ß‚µ‚½‚ç
            {
                timer = 0; // ƒ^ƒCƒ}[‚ğƒŠƒZƒbƒg
                Attack(); // UŒ‚
            }
            yield return null; // 1ƒtƒŒ[ƒ€ˆ—‚ğ’†’f‚·‚é
        }
    }

    // UŒ‚
    private void Attack()
    {
        Debug.Log("UŒ‚");
        enemy.CalcDamage(attackPower);
    }
}
