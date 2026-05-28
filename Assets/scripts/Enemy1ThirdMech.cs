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
    public void StartAttackSequence()
    {
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        TopRightAttack.StartBasicAttack();
        TopLeftAttack.StartBasicAttack();
        BottomRightAttack.StartBasicAttack();
        BottomLeftAttack.StartBasicAttack();
        yield return new WaitForSeconds(TopRightAttack.telegraphDuration + TopRightAttack.attackDuration + attackEndingInterval);
        
    }
}
