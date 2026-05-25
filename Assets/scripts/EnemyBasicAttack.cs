using UnityEngine;
using System.Collections;

public class EnemyBasicAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float telegraphDuration = 1f;
    public float attackDuration = 1f;
    public int damageAmount = 10;



    [Header("Visuals")]
    public SpriteRenderer telegraphEffect;
    public BoxCollider2D damageZone;

    public Color telegraphColour = new Color(1f, 0f, 0f, 0.5f); // Semi-transparent red
    public Color damageZoneColour = new Color(1f, 0f, 0f, 0.8f); // More opaque red


    void Start()
    {
        damageZone.enabled = false;
        
    }
    public void StartBasicAttack()
    {
        gameObject.SetActive(true);
        telegraphEffect.color = Color.clear;
        StartCoroutine(BasicAttackSequence());
    }

    public IEnumerator BasicAttackSequence()
    {
        // Show telegraph effect
        Debug.Log("Telegraph colour: " + telegraphColour);
        telegraphEffect.color = telegraphColour;
        damageZone.enabled = false;
        Debug.Log("Enemy is telegraphing an attack!");
        yield return new WaitForSeconds(telegraphDuration);
        // Activate damage zone
        telegraphEffect.color = damageZoneColour;
        damageZone.enabled = true;
        Debug.Log("Enemy is attacking!");
        yield return new WaitForSeconds(attackDuration);
        // Hide damage zone
        telegraphEffect.color = Color.clear;
        damageZone.enabled = false;
        
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
