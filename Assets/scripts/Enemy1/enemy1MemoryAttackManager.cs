using UnityEngine;
using System.Collections;

public class enemy1MemoryAttackManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyMemoryAttack firstMemoryAttack;
    public EnemyMemoryAttack secondMemoryAttack;
    public EnemyMemoryAttack thirdMemoryAttack;
    public float memoryAttackInterval = 10f; // Time to wait for player to memorise the attack before it is displayed
    public float telegraphDisplayInterval = 1.5f; // Time between telegraph displays
    public float attackDisplayInterval = 0.5f;
    public bool attackComplete = false; // Flag to indicate when the attack sequence is complete


    public void StartMemoryAttackSequence()
    {
        attackComplete = false;
        StartCoroutine(MemoryAttackSequence());
    }

    IEnumerator MemoryAttackSequence()
    {
        firstMemoryAttack.displayTelegraph();
        yield return new WaitForSeconds(telegraphDisplayInterval);
        secondMemoryAttack.displayTelegraph();
        yield return new WaitForSeconds(telegraphDisplayInterval);
        thirdMemoryAttack.displayTelegraph();
        yield return new WaitForSeconds(memoryAttackInterval);
        firstMemoryAttack.displayAttack();
        yield return new WaitForSeconds(attackDisplayInterval);
        secondMemoryAttack.displayAttack();
        yield return new WaitForSeconds(attackDisplayInterval);
        thirdMemoryAttack.displayAttack();
        yield return new WaitForSeconds(attackDisplayInterval);
        attackComplete = true;
    }
}
