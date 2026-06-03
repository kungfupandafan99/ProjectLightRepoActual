using UnityEngine;
using System.Collections;
public class phase1ManagerEnemy1 : MonoBehaviour
{
    public Enemy1FirstMech firstMech;
    public Enemy1SecondMech secondMech;
    public Enemy1ThirdMech thirdMech;
    public enemy1MemoryAttackManager memoryAttackManager1;
    public enemy1MemoryAttackManager memoryAttackManager2;
    public enemy1MemoryAttackManager memoryAttackManager3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static phase1ManagerEnemy1 instance;
    public float timeBetweenMechs = 0f; // Time between mechs
    public float mechCompletion = 1f; // Time that dictates when next mech runs.
    private int currentMech = 0; // 0 for first, 1 for second, 2 for third
    private bool isTeaching = true; // determines when to start layering attacks and adding complexity

    public float memoryTime = 5f; // Time to remember the attack pattern for the player
    public int phase2AttackCount = 0; // Counter for the number of attacks in phase 2
    public int phase3AttackCount = 0;
    private int memoryAttackCount = 0;
    private bool isLayeringMemoryAttacks = false; // Flag to indicate if layered memory attacks are currently active
    public enemy1MemoryAttackManager[] memoryAttackManagers; // Array to hold the memory attack managers for phase 2
    public EnemyBasicAttack[] finalAttacks; // Array to hold all basic attacks for potential use in phase 3
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
        else if (EnemyLogic.Instance.isInPhase2 == true && EnemyLogic.Instance.isInPhase3 == false)
        {

            if (phase2AttackCount % 2 == 0)
            {
                if (phase2AttackCount >= 6)
                {

                    StartCoroutine(LayeredMemoryAttacks());
                }
                else
                {
                    StartCoroutine(MemoryAttack());
                }
                phase2AttackCount++;

            }
            else
            {
                if (phase2AttackCount >= 6)
                {
                    isLayeringMemoryAttacks = true;
                    StartCoroutine(MemoryAttack());
                    yield return StartCoroutine(LayerAttacks());
                }
                else
                {
                    yield return StartCoroutine(LayerAttacks());
                }
                phase2AttackCount++;

            }

        }
        else if (EnemyLogic.Instance.isInPhase3 == true)
        {
            
           StartCoroutine(FinalAttackSequence());
            
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
        if (!isLayeringMemoryAttacks)
        {
            stateManager.instance.OnEnemyTurnComplete();
        }
        isLayeringMemoryAttacks = false;

    }

    IEnumerator MemoryAttack()
    {
        if (memoryAttackCount == 0)
        {
            memoryAttackManager1.StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManager1.attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);
            stateManager.instance.OnEnemyTurnComplete();
            memoryAttackCount++;
        }
        else if (memoryAttackCount == 1)
        {
            memoryAttackManager2.StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManager2.attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);
            stateManager.instance.OnEnemyTurnComplete();
            memoryAttackCount++;
        }
        else if (memoryAttackCount == 2)
        {
            memoryAttackManager3.StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManager3.attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);
            stateManager.instance.OnEnemyTurnComplete();
            memoryAttackCount = 0;
        }


    }

    IEnumerator LayeredMemoryAttacks()
    {
        int i = Random.Range(0, memoryAttackManagers.Length);
        int j;
        do
        {
            j = Random.Range(0, memoryAttackManagers.Length);
        } while (j == i);
        Debug.Log($"Starting layered memory attacks with managers {i} and {j}");
        memoryAttackManagers[j].memoryAttackInterval -= 1f;
        memoryAttackManagers[i].memoryAttackInterval += 1f;
        memoryAttackManagers[i].StartMemoryAttackSequence();
        yield return new WaitUntil(() => memoryAttackManagers[i].executeOtherAttack);
        yield return new WaitForSeconds(1f);
        //Reduce memoryAttackInterval i by how much u wait here so that the transition to the next attack is smoother 


        Debug.Log("First memory attack sequence has reached the point to execute the other attack, starting second memory attack sequence...");
        memoryAttackManagers[j].StartMemoryAttackSequence();
        yield return new WaitUntil(() => memoryAttackManagers[j].attackComplete);
        yield return new WaitForSeconds(timeBetweenMechs);
        memoryAttackManagers[j].memoryAttackInterval += 1f;
        memoryAttackManagers[i].memoryAttackInterval -= 1f;
        stateManager.instance.OnEnemyTurnComplete();
    }

    IEnumerator FinalAttackSequence()
    {
        foreach (EnemyBasicAttack attack in finalAttacks)
        {
            attack.StartBasicAttack();
            yield return new WaitForSeconds((attack.telegraphDuration + attack.attackDuration) * 0.8f);
        }
        yield return new WaitForSeconds(timeBetweenMechs);
        stateManager.instance.OnEnemyTurnComplete();
    }

}
