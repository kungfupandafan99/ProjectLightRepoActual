using System.Collections;
using UnityEngine;

public class Enemy1FirstMech : MonoBehaviour
{
    public EnemyBasicAttack centreAttack;
    public EnemyBasicAttack leftAttack;
    public EnemyBasicAttack rightAttack;
    public float attackInterval = 2f; // Time between attacks
    public bool sidesFirstMech = false;
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
            centreAttack.setBeatsToWait(phase3BeatsToWait);
            leftAttack.setBeatsToWait(phase3BeatsToWait);
            rightAttack.setBeatsToWait(phase3BeatsToWait);
        }
        if (!sidesFirstMech)
        {
            sidesFirstMech=false;
            centreAttack.StartBasicAttack();

            yield return new WaitForSeconds(leftAttack.telegraphDuration + leftAttack.attackDuration + attackInterval);


            leftAttack.StartBasicAttack();
            rightAttack.StartBasicAttack();

            yield return new WaitForSeconds(leftAttack.telegraphDuration + leftAttack.attackDuration);
            mechComplete = true;
            
        }
        else
        {
            sidesFirstMech = false;
            leftAttack.StartBasicAttack();
            rightAttack.StartBasicAttack();
            yield return new WaitForSeconds(leftAttack.telegraphDuration + leftAttack.attackDuration + attackInterval);
            centreAttack.StartBasicAttack();
            yield return new WaitForSeconds(centreAttack.telegraphDuration + centreAttack.attackDuration);
            mechComplete = true;
        }

    }
}
