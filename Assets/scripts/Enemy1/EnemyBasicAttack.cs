using UnityEngine;
using System.Collections;

public class EnemyBasicAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float telegraphDuration = 1f;
    public float attackDuration = 1f;
    public int damageAmount = 10;

    private int beatsToWait = 8; // Number of beats to wait before starting the attack
    public int beatCounter = 0; // Counter to track the number of beats

    [Header("Visuals")]
    public SpriteRenderer telegraphEffect;
    public Collider2D damageZone;

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
        if (EnemyLogic.Instance.isInPhase3)
        {
            telegraphDuration -= 0.005f; // Reduce telegraph duration by 0.5 seconds in phase 3
            attackDuration -= 0.005f; // Reduce attack duration by 0.5 seconds in phase 3

        }
        audioSyncManager.instance.OnBeat += BeatsCounted;



        // Show telegraph effect
        Debug.Log("Telegraph colour: " + telegraphColour);
        telegraphEffect.color = telegraphColour;
        damageZone.enabled = false;
        Debug.Log("Enemy is telegraphing an attack!");
        while (beatCounter < beatsToWait)
        {
            yield return null; // Wait until the specified number of beats have passed
        }
        audioSyncManager.instance.OnBeat -= BeatsCounted; // Unsubscribe from the beat event to stop counting beats

        // Activate damage zone
        telegraphEffect.color = damageZoneColour;
        damageZone.enabled = true;
        Debug.Log("Enemy is attacking!");
        beatCounter = 0; // Reset beat counter for the next attack
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

    void BeatsCounted()
    {
        beatCounter++;

    }

    public int getBeatsToWait()
    {
        return beatsToWait;
    }

    public void setBeatsToWait(int newBeatsToWait)
    {
        beatsToWait = newBeatsToWait;
        
    }
}
