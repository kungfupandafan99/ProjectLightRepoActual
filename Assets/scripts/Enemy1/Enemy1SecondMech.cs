using UnityEngine;
using System.Collections;

public class Enemy1SecondMech : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyBasicAttack HoriAttack;
    public EnemyBasicAttack VertAttack;
    public float attackEndingInterval = 1f; // Time after attacks end before player turn starts

    public void StartAttackSequence()
    {
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        HoriAttack.StartBasicAttack();
        VertAttack.StartBasicAttack();
        yield return new WaitForSeconds(HoriAttack.telegraphDuration + HoriAttack.attackDuration + attackEndingInterval);
        
    }
}
