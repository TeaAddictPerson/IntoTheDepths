using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    public Transform waterSurfacePoint; 
    public Transform jumpCheckPoint; 
    public LayerMask solidLayer; 

    private bool isOnSurface;
    private bool isJumping;
    private bool isOnRaft; 
    private float defaultGravityScale;

    public float jumpForce = 10f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale; 
    }

    void Update()
    {

        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

      
        isOnSurface = transform.position.y >= waterSurfacePoint.position.y;

       
        if (isOnSurface && direction.y > 0)
        {
            direction.y = 0;
        }

      
        if (isOnRaft)
        {
            rb.gravityScale = defaultGravityScale; 
            direction.y = 0; 
        }
        else
        {
            rb.gravityScale = isOnSurface ? defaultGravityScale : 0;
        }

        
        if (Input.GetKeyDown(KeyCode.Space) && (isOnRaft || isOnSurface))
        {
            StartCoroutine(Jump());
        }

     
        animator.SetBool("isOnSurface", isOnSurface);
    }

    void FixedUpdate()
    {
        if (!isJumping)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        animator.Play("PlayerJump");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
        yield return new WaitForSeconds(0.5f); 
        isJumping = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Raft")) 
        {
            isOnRaft = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Raft")) 
        {
            isOnRaft = false;
        }
    }
}
