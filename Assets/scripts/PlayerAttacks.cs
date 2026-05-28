using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAttacks : MonoBehaviour
{
    public static PlayerAttacks instance;

    [System.Serializable]
    public class Attack
    {
        public string attackName;
        public KeyCode[] inputSequence;
        public float timeLimit;
        public int damage;
        public float recoilDamage;
    }

    public Attack[] attacks;

    [Header("UI")]
    public GameObject attackPromptUI;
    public GameObject damageUI;
    public TextMeshProUGUI SequenceText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI recoilText;


    private int currentAttackIndex;
    private float timer;
    private int currentInputIndex;
    private bool isAttacking;



    void Awake()
    {
        instance = this;
    }

    public void startAttack(int attackIndex)
    {

        currentAttackIndex = attackIndex;
        currentInputIndex = 0;
        timer = attacks[currentAttackIndex].timeLimit;
        attackPromptUI.SetActive(true);
        isAttacking = true;
        UpdateSequenceDisplay();
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        while (timer > 0 && isAttacking)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();

            Attack currentAttack = attacks[currentAttackIndex];
            if (Input.GetKeyDown(currentAttack.inputSequence[currentInputIndex]))
            {
                currentInputIndex++;
                UpdateSequenceDisplay();
                if (currentInputIndex >= currentAttack.inputSequence.Length)
                {
                    // Attack successful
                    damageUI.SetActive(true);
                    attackPromptUI.SetActive(false);
                    Debug.Log("Attack Successful!");
                    damageText.text = $"Damage: {currentAttack.damage}";
                    EnemyLogic.Instance.TakeDamage(currentAttack.damage);
                    isAttacking = false;
                   
                    StartCoroutine(FinishSequence());
                    yield break;
                }

            }
            else if (Input.anyKeyDown && !Input.GetKeyDown(currentAttack.inputSequence[currentInputIndex]))
            {
                damageUI.SetActive(true);
                
                Debug.Log("Wrong Input!");
                currentInputIndex = 0;
                UpdateSequenceDisplay();
            }
            yield return null;


        }
        if (isAttacking)
        {
            Debug.Log("Attack Failed!");
            isAttacking = false;
            damageUI.SetActive(true);
            attackPromptUI.SetActive(false);
            damageText.text = $"Attack Failed! dealt {attacks[currentAttackIndex].damage/2}";
            EnemyLogic.Instance.TakeDamage((attacks[currentAttackIndex].damage) / 2);
            playerHealth.Instance.TakeDamage((int)attacks[currentAttackIndex].recoilDamage);
            recoilText.text = $"Recoil Damage: {attacks[currentAttackIndex].recoilDamage}";
            StartCoroutine(FinishSequence());
        }
    }

    IEnumerator FinishSequence()
    {

        yield return new WaitForSeconds(2f);
        Debug.Log("Sequence Complete");
        damageUI.SetActive(false);
        damageText.text = "";
        recoilText.text = "";
        stateManager.instance.OnPlayerAttackComplete();
    }

    public void UpdateSequenceDisplay()
    {
        Attack currentAttack = attacks[currentAttackIndex];
        string display = "";
        for (int i = currentInputIndex; i < currentAttack.inputSequence.Length; i++)
        {
            display += currentAttack.inputSequence[i].ToString() + " ";
        }
        SequenceText.text = display;
    }
}
