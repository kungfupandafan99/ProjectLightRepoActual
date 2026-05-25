using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    public static playerMovement instance;
    public Rigidbody2D rb2d;
    BoxCollider2D collider2d;
    public float playerSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    float playerDirectionX;
    float playerDirectionY;
    public bool canDash = true;
    bool isDashing = false;
    public bool canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<BoxCollider2D>();
        rb2d.gravityScale = 0f;
    }

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing && canMove)
        {
            walking();
        }
       
    }

    void walking()
    {
        rb2d.linearVelocity = new Vector2(playerSpeed * playerDirectionX, playerSpeed * playerDirectionY);
    }

    void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        playerDirectionX = inputVector.x;
        playerDirectionY = inputVector.y;
        
    }

    void OnDash(InputValue value)
    {
        if (canDash)
        {
            StartCoroutine(Dash());

        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            Debug.Log("dashed");
            rb2d.linearVelocity = new Vector2(dashSpeed * playerDirectionX, dashSpeed * playerDirectionY);
            yield return null;
        }
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        Debug.Log("Ended dash");
        canDash = true;
        
    }
}
