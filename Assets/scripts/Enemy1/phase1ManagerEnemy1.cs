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
    public int beatCounter = 0; // Counter to keep track of beats for timing attacks
    public int memoryBeatCounter = 0; // Counter to keep track of beats for timing memory attacks
    private int beatsBetweenMechs = 9;
    public float memoryTime = 5f; // Time to remember the attack pattern for the player
    public int phase2AttackCount = 0; // Counter for the number of attacks in phase 2
    public int phase3AttackCount = 0;
    private int memoryAttackCount = 0;
    private bool isLayeringAttacks = false; // Flag to indicate if layered memory attacks are currently active
    public enemy1MemoryAttackManager[] memoryAttackManagers; // Array to hold the memory attack managers for phase 2
    public EnemyBasicAttack[] finalAttacks; // Array to hold all basic attacks for potential use in phase 3
    private bool memoryAttacking = false;
    private bool turnComplete = false;
    private bool memoryAttackLayering = false; // Flag to indicate if a memory attack is currently in progress
    public int beatsToWaitFinal = 2;

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

                    yield return StartCoroutine(LayeredMemoryAttacks());
                }
                else
                {
                    yield return StartCoroutine(MemoryAttack());
                }
                phase2AttackCount++;

            }
            else
            {
                if (phase2AttackCount >= 6)
                {
                    memoryAttackLayering = true;
                    isLayeringAttacks = true;
                    StartCoroutine(MemoryAttack());
                    
                    yield return StartCoroutine(LayerAttacks());
                    yield return new WaitUntil(() => turnComplete == true); // Wait until the layered memory attacks are done before proceeding
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
            Debug.Log("Phase 3 attack count: " + phase3AttackCount);
            if (phase3AttackCount % 3 == 0)
            {

                yield return StartCoroutine(FinalAttackSequence());
            }
            else if (phase3AttackCount % 3 == 1)
            {


                yield return StartCoroutine(LayeredMemoryAttacks());
            }
            else
            {
                isLayeringAttacks = true;
                
                
                StartCoroutine(LayeredMemoryAttacks());
                yield return new WaitUntil(() => memoryAttacking == false); // Wait until the layered memory attacks are done before proceeding
                StartCoroutine(LayerAttacks());
                yield return new WaitUntil(() => isLayeringAttacks == false); // Wait until the layered attacks are done before proceeding
                
                yield return new WaitUntil(() => memoryAttacking == false); // Wait until the layered memory attacks are done before proceeding
                isLayeringAttacks = false; // Reset the flag after the layered attacks are done
            }
            phase3AttackCount++;
        }
        stateManager.instance.OnEnemyTurnComplete();

    }

    IEnumerator NextMechanic()
    {
        if (currentMech == 0)
        {
            firstMech.StartAttackSequence();
            yield return new WaitUntil(() => firstMech.mechComplete);
            yield return new WaitForSeconds((firstMech.leftAttack.telegraphDuration + firstMech.leftAttack.attackDuration + timeBetweenMechs) * 2);

        }
        else if (currentMech == 1)
        {
            secondMech.StartAttackSequence();
            yield return new WaitUntil(() => secondMech.mechComplete);
            yield return new WaitForSeconds(secondMech.HoriAttack.telegraphDuration + secondMech.HoriAttack.attackDuration + timeBetweenMechs);
        }
        else if (currentMech == 2)
        {
            thirdMech.StartAttackSequence();
            yield return new WaitUntil(() => thirdMech.mechComplete);
            yield return new WaitForSeconds(thirdMech.TopRightAttack.telegraphDuration + thirdMech.TopRightAttack.attackDuration + timeBetweenMechs);
            isTeaching = false; // After the third mech, start layering attacks
        }
        currentMech++;


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
        Debug.Log("Are you being ran?");
        isLayeringAttacks = true;
        
        if (memoryAttackLayering == true)
        {
            audioSyncManager.instance.OnBeat += beatCount;
            while (beatCounter < 12)
            {
                yield return null;
            }
            audioSyncManager.instance.OnBeat -= beatCount;
            beatCounter = 0;
            memoryAttackLayering = false;
        }
        Debug.Log("Starting layered attacks in order: " + string.Join(", ", order));
        foreach (int value in order)
        {
            
            audioSyncManager.instance.OnBeat += beatCount;
            
            
            if (EnemyLogic.Instance.isInPhase3)
            {
                firstMech.isInPhase3 = true;
                secondMech.isInPhase3 = true;
                thirdMech.isInPhase3 = true;
                beatsBetweenMechs = 4; // Reduce the number of beats between mechs in phase 3
                
            }
            // Subscribe to the beat event to start counting beats for timing the next attack
            
            if (value == 0)
            {
                firstMech.StartAttackSequence();

            }
            else if (value == 1) 
            {
                secondMech.StartAttackSequence();

            }
            else if (value == 2)
            {
                thirdMech.StartAttackSequence();

            }

            while (beatCounter < beatsBetweenMechs)
            {
                yield return null; // Wait until the next frame and check again
            }
            // Instead of yield return compare beat counter to waitingBeats for layeredAttacks
            // KEEP SUBSCRIBED TO BEAT EVENT WE NEED TO CONTINUE COUNTING BEATS TO SYNC THE LAYERED ATTACKS
            // Reset Beat counter here for checks to work
            beatCounter = 0;
            
            audioSyncManager.instance.OnBeat -= beatCount;
            
            
            // Unsubscribe and resubscribe to reset the beat counter for the next attack timing (This is testing purposes in case staying subscribed causes sync issues)

        }
        yield return new WaitUntil(() => firstMech.mechComplete && secondMech.mechComplete && thirdMech.mechComplete);
        isLayeringAttacks = false; // Reset the flag after the layered attacks are done
        yield return new WaitForSeconds(timeBetweenMechs + 2f);




    }

    IEnumerator MemoryAttack()
    {
        turnComplete = false;
        if (memoryAttackCount == 0)
        {
            memoryAttackManager1.StartMemoryTelegraphing();
          
            yield return new WaitUntil(() => memoryAttackManager1.telegraphingComplete);
            
            yield return new WaitUntil(() => isLayeringAttacks == false);
            memoryAttackManager1.StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManager1.attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);

            memoryAttackCount++;
        }
        else if (memoryAttackCount == 1)
        {
            memoryAttackManager2.StartMemoryTelegraphing();
            
            yield return new WaitUntil(() => memoryAttackManager2.telegraphingComplete);
            
            yield return new WaitUntil(() => isLayeringAttacks == false);
            memoryAttackManager2.StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManager2.attackComplete);

            yield return new WaitForSeconds(timeBetweenMechs);

            memoryAttackCount++;
        }
        else if (memoryAttackCount == 2)
        {
            memoryAttackManager3.StartMemoryTelegraphing();
            
            yield return new WaitUntil(() => memoryAttackManager3.telegraphingComplete);
            
            yield return new WaitUntil(() => isLayeringAttacks == false);
            memoryAttackManager3.StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManager3.attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);

            memoryAttackCount = 0;
        }
        turnComplete = true;


    }


    // For phase 3 we need a separate beat counter so that it doesn't interfere with the layered attacks.
    // This is because the layered attacks are still running on the same beat counter as the memory attacks,
    // and we need to keep track of the beats for the memory attacks separately.
    IEnumerator LayeredMemoryAttacks()
    {
        memoryBeatCounter = 0;
        memoryAttacking = true;
        if ((EnemyLogic.Instance.isInPhase2 == true && EnemyLogic.Instance.isInPhase3 == false) || (isLayeringAttacks == true && EnemyLogic.Instance.isInPhase3 == true))
        {
            int i = Random.Range(0, memoryAttackManagers.Length);
            int j;
            do
            {
                j = Random.Range(0, memoryAttackManagers.Length);
            } while (j == i);

            Debug.Log($"Starting layered memory attacks with managers {i} and {j}");
            memoryAttackManagers[i].StartMemoryTelegraphing();
            yield return new WaitUntil(() => memoryAttackManagers[i].executeOtherAttack);
            yield return new WaitForSeconds(1f);
            //Reduce memoryAttackInterval i by how much u wait here so that the transition to the next attack is smoother 


            Debug.Log("First memory attack sequence has reached the point to execute the other attack, starting second memory attack sequence...");
            memoryAttackManagers[j].StartMemoryTelegraphing();
            yield return new WaitUntil(() => memoryAttackManagers[j].executeOtherAttack);
            memoryAttacking = false;
            if (EnemyLogic.Instance.isInPhase3 == true)
            {
                audioSyncManager.instance.OnBeat += memoryBeatCount;
                while (memoryBeatCounter < 4)
                {
                    yield return null;
                }
                audioSyncManager.instance.OnBeat -= memoryBeatCount;
                memoryBeatCounter = 0;
            }
            memoryAttackManagers[i].StartMemoryAttackSequence();
            audioSyncManager.instance.OnBeat += memoryBeatCount;
            while (memoryBeatCounter < 12)
            {
                yield return null;
            }
            memoryAttackManagers[j].StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManagers[j].attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);
        }
        else if (EnemyLogic.Instance.isInPhase3 == true && isLayeringAttacks == false)

        {
            int i = Random.Range(0, memoryAttackManagers.Length);
            int j;
            int k;
            do
            {
                k = Random.Range(0, memoryAttackManagers.Length);
                j = Random.Range(0, memoryAttackManagers.Length);
            } while (j == i || j == k || k == i);

            memoryAttackManagers[i].StartMemoryTelegraphing();
            yield return new WaitUntil(() => memoryAttackManagers[i].executeOtherAttack);
            yield return new WaitForSeconds(1f);
            memoryAttackManagers[j].StartMemoryTelegraphing();
            yield return new WaitUntil(() => memoryAttackManagers[j].executeOtherAttack);
            yield return new WaitForSeconds(1f);
            memoryAttackManagers[k].StartMemoryTelegraphing();
            yield return new WaitUntil(() => memoryAttackManagers[k].executeOtherAttack);
            
            memoryAttackManagers[i].StartMemoryAttackSequence();
            audioSyncManager.instance.OnBeat += beatCount;
            while (beatCounter < 12)
            {
                yield return null;
            }
            beatCounter = 0;
            memoryAttackManagers[j].StartMemoryAttackSequence();
            while (beatCounter < 12)
            {
                yield return null;
            }
            memoryAttackManagers[k].StartMemoryAttackSequence();
            yield return new WaitUntil(() => memoryAttackManagers[k].attackComplete);
            yield return new WaitForSeconds(timeBetweenMechs);

        }
        memoryAttacking = false;
    }

    IEnumerator FinalAttackSequence()
    {
        beatCounter = 0;
        audioSyncManager.instance.OnBeat += beatCount;
        foreach (EnemyBasicAttack attack in finalAttacks)
        {

            attack.setBeatsToWait(beatsToWaitFinal);
            attack.StartBasicAttack();
            while (beatCounter < 2)
            {
                yield return null;
            }
            beatCounter = 0;
        }
        audioSyncManager.instance.OnBeat -= beatCount;
        beatCounter = 0;
        yield return new WaitForSeconds(timeBetweenMechs);

    }


    void beatCount()
    {
        beatCounter++;
    }

    void memoryBeatCount()
    {
        memoryBeatCounter++;
    }
}
