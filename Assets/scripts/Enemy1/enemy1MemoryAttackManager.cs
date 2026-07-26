using UnityEngine;
using System.Collections;

public class enemy1MemoryAttackManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyMemoryAttack firstMemoryAttack;
    public EnemyMemoryAttack secondMemoryAttack;
    public EnemyMemoryAttack thirdMemoryAttack;
    public float memoryAttackInterval = 13f; // Beats to wait for player to memorise the attack before it is displayed
    public float telegraphDisplayInterval = 1.5f; // Time between telegraph displays
    public float attackDisplayInterval = 0.5f;
    public bool attackComplete = false; // Flag to indicate when the attack sequence is complete
    public bool executeOtherAttack = false;
    public bool telegraphingComplete = false;
    public int beatCounter = 0;
    public int beatsToWait = 5;
    


    public void StartMemoryAttackSequence()
    {
        executeOtherAttack = false;
        attackComplete = false;
        StartCoroutine(MemoryAttackSequence());
    }

    public void StartMemoryTelegraphing()
    {
        telegraphingComplete = false;
        executeOtherAttack = false;
    
        StartCoroutine(MemoryTelegraphing());
    }
    IEnumerator MemoryTelegraphing()
    {
        audioSyncManager.instance.OnBeat += beatIncrementor;
        beatCounter = 0;
        firstMemoryAttack.displayTelegraph();
        while (beatCounter < beatsToWait)
        {
            yield return null;
        }
        beatCounter = 0;

        secondMemoryAttack.displayTelegraph();
        while (beatCounter < beatsToWait)
        {
            yield return null;
        }
        audioSyncManager.instance.OnBeat -= beatIncrementor;
        beatCounter = 0;
        thirdMemoryAttack.displayTelegraph();
        telegraphingComplete = true;
        executeOtherAttack = true;
        
    }

    IEnumerator MemoryAttackSequence()
    {
        audioSyncManager.instance.OnBeat += beatIncrementor;
        while (beatCounter < memoryAttackInterval)
        {
            yield return null;
        }
        beatCounter = 0;
        firstMemoryAttack.displayAttack();
        while (beatCounter < beatsToWait)
        {
            yield return null;
        }
        beatCounter = 0;
        secondMemoryAttack.displayAttack();
        while (beatCounter < beatsToWait)
        {
            yield return null;
        }
        beatCounter = 0;
        thirdMemoryAttack.displayAttack();
        while (beatCounter < beatsToWait)
        {
            yield return null;
        }
        audioSyncManager.instance.OnBeat -= beatIncrementor;
        beatCounter = 0;
        attackComplete = true;
    }

    public int GetBeatMemoryDuration()
    {
        return beatsToWait;
    }

    public void SetBeatMemoryDuration(int beats)
    {
        beatsToWait = beats;
    }

    void beatIncrementor()
    {
        
        beatCounter++;
    }


}
