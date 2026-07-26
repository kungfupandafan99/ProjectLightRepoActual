using UnityEngine;
using System.Collections;

public class PhaseManagerEnemy2 : MonoBehaviour
{
    public static PhaseManagerEnemy2 instance;

    public AttackManipulation[] attacks;

    public float timeBetweenAttacks = 2f; // Time between attacks in seconds
    public float minDuration = 2f; // Minimum duration for each attack
    public float maxDuration = 4f; // Maximum duration for each attack
    private int teachingAttackIndex = 0; // Index for the teaching attack
    private bool isTeachingAttack = true; // Flag to indicate if the teaching attack is active

    void Awake()
    {
        instance = this;
    }

    public void RunNextAttack()
    {
        StartCoroutine(DecideAttack());
    }
    
    IEnumerator DecideAttack()
    {
        if (isTeachingAttack)
        {
            yield return StartCoroutine(TeachNextAttack());
        }
        else
        {
            StartCoroutine(RunRandomAttack());
        }
        stateManager.instance.OnEnemyTurnComplete();
    }
    public void randomiseDuration(AttackManipulation attack)
    {
        float duration = Random.Range(minDuration, maxDuration);
        if (attack.scaling)
        {
            attack.scaleDuration = duration;
        }
        else if (attack.positioning)
        {
            attack.positionDuration = duration;
        }
        else if (attack.rotating)
        {
            attack.rotationDeceleration = duration;
        }
    }
    IEnumerator TeachNextAttack()
    {
        randomiseDuration(attacks[teachingAttackIndex]);
        yield return StartCoroutine(RunSingleAttack(teachingAttackIndex));
        teachingAttackIndex++;
        if (teachingAttackIndex >= attacks.Length)
        {
            isTeachingAttack = false; // Switch to random attacks after teaching all
        }
    }
    IEnumerator RunRandomAttack()
    {
        int randomIndex = Random.Range(0, attacks.Length);
        randomiseDuration(attacks[randomIndex]);
        yield return StartCoroutine(RunSingleAttack(randomIndex));
    }

    IEnumerator RunSingleAttack(int index)
    {
        attacks[index].ActivateAttack();
        yield return new WaitUntil(() => attacks[index].attackComplete);
    }
}
