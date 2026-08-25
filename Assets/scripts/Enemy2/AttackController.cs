using System.Collections;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public AttackManipulation[] attacks;
    public bool isAttackActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float minDuration = 2f; // Minimum duration for each attack
    public float maxDuration = 4f; // Maximum duration for each attack

    // Update is called once per frame
    public void RunAttack()
    {
        StartCoroutine(RunAttackCoroutine());

    }

    IEnumerator RunAttackCoroutine()
    {
        isAttackActive = true;
        foreach (AttackManipulation attack in attacks)
        {
            randomiseDuration(attack);
            attack.ActivateAttack();
            
        }
        yield return new WaitUntil(() => attacks[0].attackComplete);
        isAttackActive = false;
        
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
}
