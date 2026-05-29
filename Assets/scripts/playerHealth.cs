using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class playerHealth : MonoBehaviour
{
    public static playerHealth Instance;
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    private Animator anim;

    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Update is called once per frame

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("beenHit");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void UpdateHealthBar()
    {
        Debug.Log(currentHealth);
        Debug.Log(maxHealth);
        float healthSliderValue = (float) currentHealth / maxHealth;
        Debug.Log(healthSliderValue);
        healthText.text = $"{currentHealth}";
        healthSlider.value = healthSliderValue;
    }

    IEnumerator Die()
    {
        // You can add a death animation or effect here
        yield return new WaitForSeconds(2f);
        Debug.Log("Player Died!");
        stateManager.instance.currentState = stateManager.CombatState.Defeat;
    }


}
