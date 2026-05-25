using System.Collections;
using UnityEngine;

public class Enemy1FirstMech : MonoBehaviour
{
    public EnemyBasicAttack centreAttack;
    public EnemyBasicAttack leftAttack;
    public EnemyBasicAttack rightAttack;
    public float attackInterval = 2f; // Time between attacks
    public bool sidesFirstMech = false;

    public void StartAttackSequence()
    {
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        if (!sidesFirstMech)
        {
            sidesFirstMech=true;
            centreAttack.StartBasicAttack();

            yield return new WaitForSeconds(leftAttack.telegraphDuration + leftAttack.attackDuration + attackInterval);


            leftAttack.StartBasicAttack();
            rightAttack.StartBasicAttack();

            yield return new WaitForSeconds(leftAttack.telegraphDuration + leftAttack.attackDuration);

            stateManager.instance.OnEnemyTurnComplete();
        }
        else
        {
            sidesFirstMech = false;
            leftAttack.StartBasicAttack();
            rightAttack.StartBasicAttack();
            yield return new WaitForSeconds(leftAttack.telegraphDuration + leftAttack.attackDuration + attackInterval);
            centreAttack.StartBasicAttack();
            yield return new WaitForSeconds(centreAttack.telegraphDuration + centreAttack.attackDuration);
            stateManager.instance.OnEnemyTurnComplete();
        }

    }
}
