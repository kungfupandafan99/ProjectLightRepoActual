using UnityEngine;
using System.Collections;
using System.Linq;

public class PhaseManagerEnemy2 : MonoBehaviour
{
    public static PhaseManagerEnemy2 instance;

    public AttackManipulation[] attacks;
    public AttackManipulation[] harderAttacks;
    public AttackController[] harderControllers;
    public AttackManipulation[] finalAttacks;
    public float timeBetweenAttacks = 2f; // Time between attacks in seconds
    public float timeBetweenFinalAttacks = 1f;
    public float minDuration = 2f; // Minimum duration for each attack
    public float maxDuration = 4f; // Maximum duration for each attack
    private int teachingAttackIndex = 0; // Index for the teaching attack
    public bool isTeachingAttack = true; // Flag to indicate if the teaching attack is active
    public int phase2AttackCount = 0; // Counter for the number of attacks in phase 2
    public int phase3AttackCount = 0; //Counter for the number of attacks in phase 3
    [SerializeField] private GameObject ricochetBullet;
    public Transform bulletSpawn;

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
        else if (EnemyLogic.Instance.isInPhase2 != true)
        {
            yield return StartCoroutine(RunRandomAttack());
        }
        else if (EnemyLogic.Instance.isInPhase2 && !EnemyLogic.Instance.isInPhase3)
        {
            if (phase2AttackCount % 2 == 0)
            {
                yield return StartCoroutine(RunBulletRicochet());
            }
            else
            {
                int randomChoice = Random.Range(0, 2); // Randomly choose between 0 and 1
                
               // int randomChoice = 0; // test
                Debug.Log("Random choice for multiple attacks: " + randomChoice);
                if (randomChoice == 0)
                {
                    yield return StartCoroutine(RunRandomMultipleAttacks());
                }
                else
                {
                    yield return StartCoroutine(RunRandomMultipleAttackControllers());
                } 
            }
            phase2AttackCount++;
            
        }
        else
        {
            if (phase3AttackCount % 3 == 0)
            {
                yield return StartCoroutine(FinalAttack());
                
            }
            else if (phase3AttackCount % 3 == 1)
            {
                StartCoroutine(RunBulletRicochet());
                yield return new WaitForSeconds(timeBetweenAttacks);
                int randomChoice = Random.Range(0, 2); // Randomly choose between 0 and 1

                // int randomChoice = 0; // test
                Debug.Log("Random choice for multiple attacks: " + randomChoice);
                if (randomChoice == 0)
                {
                    yield return StartCoroutine(RunRandomMultipleAttacks());
                }
                else
                {
                    yield return StartCoroutine(RunRandomMultipleAttackControllers());
                }
            }
            else
            {
                StartCoroutine(RunBulletRicochet());
                yield return StartCoroutine(RunBulletRicochet());
            }
            phase3AttackCount++;
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
        if (attack.positioning)
        {
            attack.positionDuration = duration;
        }
        if (attack.rotating)
        {
            attack.rotationDeceleration = duration;
        }
    }
    IEnumerator RunBulletRicochet()
    {
        GameObject bullet = Instantiate(ricochetBullet, bulletSpawn.position, Quaternion.identity);
        RicochettingBullet bulletScript = bullet.GetComponent<RicochettingBullet>();
        yield return new WaitUntil(() => bulletScript.isActive == false);
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
        int randomIndex = Random.Range(0, harderAttacks.Length);
        Debug.Log("Running random attack: " + randomIndex);
        randomiseDuration(harderAttacks[randomIndex]);
        yield return StartCoroutine(RunSingleAttack(randomIndex));
    }
    IEnumerator RunRandomMultipleAttacks()
    {
        int randomIndex1 = Random.Range(0, harderAttacks.Length);
        int randomIndex2 = Random.Range(0, harderAttacks.Length);
        while (randomIndex2 == randomIndex1)
        {
            randomIndex2 = Random.Range(0, harderAttacks.Length);
        }
        Debug.Log("Running random attacks: " + randomIndex1 + " and " + randomIndex2);
        randomiseDuration(harderAttacks[randomIndex1]);
        randomiseDuration(harderAttacks[randomIndex2]);
        StartCoroutine(RunSingleAttack(randomIndex1));
        yield return new WaitForSeconds(timeBetweenAttacks);
        yield return StartCoroutine(RunSingleAttack(randomIndex2));
        yield return new WaitForSeconds(3f);
    }
    IEnumerator RunRandomMultipleAttackControllers()
    {
        int randomIndex1 = Random.Range(0, harderControllers.Length);
        
        Debug.Log("Running random attack controllers: " + randomIndex1);
        harderControllers[randomIndex1].RunAttack();
        
        
        yield return new WaitUntil(() => !harderControllers[randomIndex1].isAttackActive);
        yield return new WaitForSeconds(2f);
    }
    IEnumerator RunSingleAttack(int index)
    {
        if (isTeachingAttack)
        {
            attacks[index].ActivateAttack();
            yield return new WaitUntil(() => attacks[index].attackComplete);
        }
        else
        {
            harderAttacks[index].ActivateAttack();
            yield return new WaitUntil(() => harderAttacks[index].attackComplete);
        }
    }
    IEnumerator FinalAttack()
    {
        int randomIndex = Random.Range(0, finalAttacks.Length);
        int[] completeIndices = new int[finalAttacks.Length];
        int[] finalAttackOrder = ShuffleIndices(finalAttacks.Length);

        for (int i = 0; i < finalAttackOrder.Length; i++)
        {
            int index = finalAttackOrder[i];
            randomiseDuration(finalAttacks[index]);
            finalAttacks[index].ActivateAttack();
            yield return new WaitForSeconds(timeBetweenFinalAttacks);
        }
        yield return new WaitUntil(() => finalAttacks[finalAttackOrder[^1]].attackComplete);
    }
    int[] ShuffleIndices(int length)
    {
        int[] order = new int[length];
        for (int i = 0; i < length; i++) order[i] = i;

        for (int i = order.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }
        return order;
    }
}
