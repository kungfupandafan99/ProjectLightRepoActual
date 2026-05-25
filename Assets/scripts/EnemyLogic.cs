using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class EnemyLogic : MonoBehaviour
{
    public static EnemyLogic Instance;

    [Header("Enemy stats")]
    public int maxHealth = 200;
    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Animator")]
    public Animator animator;

    void Awake()
    {
        Instance = this;


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    void UpdateHealthBar()
    {
        healthSlider.value = (float)currentHealth / maxHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
        Debug.Log($"Enemy Health: {currentHealth} / {maxHealth}");
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");
        yield return new WaitForSeconds(2f); // Wait for the death animation to finish
        gameObject.SetActive(false); // Deactivate the enemy object
        Debug.Log("Enemy Defeated!");
    }
}
