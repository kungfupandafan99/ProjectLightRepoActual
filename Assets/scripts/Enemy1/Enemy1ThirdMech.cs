using System.Collections;
using UnityEngine;

public class Enemy1ThirdMech : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyBasicAttack TopRightAttack;
    public EnemyBasicAttack TopLeftAttack;
    public EnemyBasicAttack BottomRightAttack;
    public EnemyBasicAttack BottomLeftAttack;
    public float attackEndingInterval = 1f; // Time after attacks end before player turn starts
    public bool mechComplete = false;
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
            TopRightAttack.setBeatsToWait(phase3BeatsToWait);
            TopLeftAttack.setBeatsToWait(phase3BeatsToWait);
            BottomRightAttack.setBeatsToWait(phase3BeatsToWait);
            BottomLeftAttack.setBeatsToWait(phase3BeatsToWait);
        }
        TopRightAttack.StartBasicAttack();
        TopLeftAttack.StartBasicAttack();
        BottomRightAttack.StartBasicAttack();
        BottomLeftAttack.StartBasicAttack();
        yield return new WaitForSeconds(TopRightAttack.telegraphDuration + TopRightAttack.attackDuration + attackEndingInterval);
        mechComplete = true;
        
    }
}
