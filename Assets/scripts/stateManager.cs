using System.Collections;
using UnityEngine;

public class stateManager : MonoBehaviour
{
    public static stateManager instance;
    public enum CombatState { PlayerTurn, PlayerAttack, PrepWindow, EnemyTurn, Victory, Defeat }
    public CombatState currentState;
    public float prepWindowDuration = 7f;

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
        phase1ManagerEnemy1.instance.RunNextAttack();
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
        float timer = prepWindowDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //add UI element to show timer countdown if want
            yield return null;
        }
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
}
