using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnBasedUI : MonoBehaviour
{
    public static TurnBasedUI Instance;
    public GameObject playerTurnUI;
    public Button[] buttons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttons[0].onClick.AddListener(() => actionSelected(0));
        buttons[1].onClick.AddListener(() => actionSelected(1));
        buttons[2].onClick.AddListener(() => actionSelected(2));
        buttons[3].onClick.AddListener(() => actionSelected(3));
    }

    private void Awake()
    {
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void showTBCUI()
    {
        playerTurnUI.SetActive(true);
    }

    public void hideTBCUI()
    {
        playerTurnUI.SetActive(false);
    }

    public void actionSelected(int actionIndex)
    {
        hideTBCUI();
        stateManager.instance.currentState = stateManager.CombatState.PlayerAttack;
        //Handle the selected action based on the actionIndex
    }
}
