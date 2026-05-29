using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    public static playerMovement instance;
    public Rigidbody2D rb2d;
    BoxCollider2D collider2d;
    public Animator anim;
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
        anim = GetComponent<Animator>();
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
            Debug.Log("Player is walking");
            walking();
        }
        if (canMove == false && isDashing == false)
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunningUp", false);
        }




    }

    void walking()
    {

        rb2d.linearVelocity = new Vector2(playerSpeed * playerDirectionX, playerSpeed * playerDirectionY);
        transform.localScale = new Vector3(Mathf.Sign(playerDirectionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (playerDirectionX == 0 && playerDirectionY == 0)
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunningUp", false);
        }
        else if (playerDirectionX != 0 && playerDirectionY == 0)
        {
            anim.SetBool("isRunning", true);
        }
        else if (playerDirectionX == 0 && playerDirectionY != 0)
        {
            anim.SetBool("isRunningUp", true);
        }
       
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
        anim.SetTrigger("isDashing");
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
