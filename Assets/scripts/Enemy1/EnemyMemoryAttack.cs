using UnityEngine;
using System.Collections;

public class EnemyMemoryAttack : MonoBehaviour
{
    public float telegraphDuration = 1f;
    public float attackDuration = 1f;
   
    public int damageAmount = 10;

    public SpriteRenderer telegraphMemory;
    public Collider2D damageZone;

    public Color telegraph = new Color(1.0f, 0.647f, 0.0f, 0.5f); // Semi-transparent orange
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Color attack = new Color(1f, 0f, 0f, 0.8f); // More opaque red

    public void displayTelegraph()
    {
        gameObject.SetActive(true);

        telegraphMemory.color = Color.clear;
        StartCoroutine(showTelegraph());
    }

    IEnumerator showTelegraph()
    {
        telegraphMemory.color = telegraph;
        damageZone.enabled = false;
        Debug.Log("Enemy is telegraphing attack");
        yield return new WaitForSeconds(telegraphDuration);
        telegraphMemory.color = Color.clear;
        Debug.Log("Attack has been telegraphed, now memorise");
    }

    public void displayAttack()
    {
        StartCoroutine(showAttack());
    }

    IEnumerator showAttack()
    {
        telegraphMemory.color = attack;
        damageZone.enabled = true;
        Debug.Log("Enemy is attacking!");
        yield return new WaitForSeconds(attackDuration);
        telegraphMemory.color = Color.clear;
        damageZone.enabled = false;
        gameObject.SetActive(false);
        Debug.Log("Attack has ended");
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth.Instance.TakeDamage(damageAmount);
            Debug.Log($"Player hit for {damageAmount} damage!");
        }
    }
}
