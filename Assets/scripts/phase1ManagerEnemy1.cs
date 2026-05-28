using UnityEngine;
using System.Collections;
public class phase1ManagerEnemy1 : MonoBehaviour
{
    public Enemy1FirstMech firstMech;
    public Enemy1SecondMech secondMech;
    public Enemy1ThirdMech thirdMech;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static phase1ManagerEnemy1 instance;
    public float timeBetweenMechs = 0f; // Time between mechs
    public float mechCompletion = 1f; // Time that dictates when next mech runs.
    private int currentMech = 0; // 0 for first, 1 for second, 2 for third
    private bool isTeaching = true; // determines when to start layering attacks and adding complexity

    public float memoryTime = 5f; // Time to remember the attack pattern for the player
    private int phase2AttackCount = 0; // Counter for the number of attacks in phase 2

    private void Awake()
    {
        instance = this;
    }
    public void RunNextAttack()
    {
        StartCoroutine(DecideAttack());
    }

    IEnumerator DecideAttack()
    {
        yield return new WaitForSeconds(timeBetweenMechs);
        if (EnemyLogic.Instance.isInPhase2 == false)
        {
            if (isTeaching)
            {
                yield return StartCoroutine(NextMechanic());
            }
            else
            {
                yield return StartCoroutine(LayerAttacks());
            }
        }
        else
        {
            if (phase2AttackCount % 2 == 0)
            {
                // yield return StartCoroutine(MemoryAttack());
            }
            else
            {
                yield return StartCoroutine(LayerAttacks());
            }

        }

    }

    IEnumerator NextMechanic()
    {
        if (currentMech == 0)
        {
            firstMech.StartAttackSequence();
            yield return new WaitForSeconds((firstMech.leftAttack.telegraphDuration + firstMech.leftAttack.attackDuration + timeBetweenMechs) * 2);

        }
        else if (currentMech == 1)
        {
            secondMech.StartAttackSequence();
            yield return new WaitForSeconds(secondMech.HoriAttack.telegraphDuration + secondMech.HoriAttack.attackDuration + timeBetweenMechs);
        }
        else if (currentMech == 2)
        {
            thirdMech.StartAttackSequence();
            yield return new WaitForSeconds(thirdMech.TopRightAttack.telegraphDuration + thirdMech.TopRightAttack.attackDuration + timeBetweenMechs);
            isTeaching = false; // After the third mech, start layering attacks
        }
        currentMech++;
        stateManager.instance.OnEnemyTurnComplete();

    }

    IEnumerator LayerAttacks()
    {
        int[] order = { 0, 1, 2 };
        for (int i = order.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = order[i];
            order[i] = order[j];
            order[j] = temp;
        }

        foreach (int value in order)
        {
            if (value == 0)
            {
                firstMech.StartAttackSequence();
                yield return new WaitForSeconds((firstMech.leftAttack.telegraphDuration + firstMech.leftAttack.attackDuration) * 2 - mechCompletion);
            }
            else if (value == 1)
            {
                secondMech.StartAttackSequence();
                yield return new WaitForSeconds(secondMech.HoriAttack.telegraphDuration + secondMech.HoriAttack.attackDuration - mechCompletion);
            }
            else if (value == 2)
            {
                thirdMech.StartAttackSequence();
                yield return new WaitForSeconds(thirdMech.TopRightAttack.telegraphDuration + thirdMech.TopRightAttack.attackDuration - mechCompletion);
            }
            yield return new WaitForSeconds(timeBetweenMechs);
        }
        yield return new WaitForSeconds(timeBetweenMechs);
        stateManager.instance.OnEnemyTurnComplete();
    }


}
