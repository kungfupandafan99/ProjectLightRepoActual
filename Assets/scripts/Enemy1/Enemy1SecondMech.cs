using UnityEngine;
using System.Collections;

public class Enemy1SecondMech : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyBasicAttack HoriAttack;
    public EnemyBasicAttack VertAttack;
    public bool mechComplete = false;
    public float attackEndingInterval = 1f; // Time after attacks end before player turn starts
    public bool isInPhase3 = false;
    public int phase3BeatsToWait = 4;
    public void StartAttackSequence()
    {
        mechComplete = false;
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        if (isInPhase3)
        {
            HoriAttack.setBeatsToWait(phase3BeatsToWait);
            VertAttack.setBeatsToWait(phase3BeatsToWait);
        }
        HoriAttack.StartBasicAttack();
        VertAttack.StartBasicAttack();
        yield return new WaitForSeconds(HoriAttack.telegraphDuration + HoriAttack.attackDuration + attackEndingInterval);
        mechComplete = true;

    }
}
