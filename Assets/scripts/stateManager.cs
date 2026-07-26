using System.Collections;
using UnityEngine;

public class stateManager : MonoBehaviour
{
    public static stateManager instance;
    public enum CombatState { PlayerTurn, PlayerAttack, PrepWindow, EnemyTurn, Victory, Defeat }
    public CombatState currentState;
    public int prepWindowBeats = 4;
    public int beatCounterValue = 0;
    public bool onEnemy2 = false;
    public bool onEnemy3 = false;

    void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnterPlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnterPlayerTurn()
    {
        currentState = CombatState.PlayerTurn;
        Debug.Log("Player's turn");
        playerMovement.instance.canMove = false;
        playerMovement.instance.canDash = false;
        

        TurnBasedUI.Instance.showTBCUI();
        
    }

    public void EnterEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;
        Debug.Log("Enemy's turn");
        TurnBasedUI.Instance.hideTBCUI();
        if (onEnemy3 == true)
        {
            // for when enemy 3 is implemented
        }
        else if (onEnemy2 == true)
        {
            PhaseManagerEnemy2.instance.RunNextAttack();
        }
        else
        { 
            phase1ManagerEnemy1.instance.RunNextAttack(); 
        }
    }

    public void EnterPrepWindow()
    {
        currentState = CombatState.PrepWindow;
        Debug.Log("Preparation window");
        playerMovement.instance.canMove = true;
        playerMovement.instance.canDash = true;
        StartCoroutine(PrepWindow());

    }

    IEnumerator PrepWindow()
    {
        audioSyncManager.instance.OnBeat += beatCounter;

        while (beatCounterValue < prepWindowBeats)
        {
            //add UI element to show timer countdown if want
            yield return null;
        }
        audioSyncManager.instance.OnBeat -= beatCounter;
        beatCounterValue = 0;
        EnterEnemyTurn();
    }

    public void OnPlayerAttackComplete()
    {
        EnterPrepWindow();
    }
    public void OnEnemyTurnComplete()
    {
        playerMovement.instance.rb2d.linearVelocity = Vector2.zero; // Stop player movement at the start of their turn
        EnterPlayerTurn();
    }

    public void beatCounter()
    {
        beatCounterValue++;
    }
}
