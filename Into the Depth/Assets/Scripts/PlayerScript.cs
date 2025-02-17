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
    public LayerMask raftLayer;

    private bool isOnSurface;
    private bool isJumping;
    private bool isOnRaft;
    private float defaultGravityScale = 2f;
    private float underwaterGravityScale = 0f;

    public float jumpForce = 10f;

    private bool FacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = defaultGravityScale;
    }

    void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        isOnSurface = transform.position.y >= waterSurfacePoint.position.y;
        bool isInWater = transform.position.y <= waterSurfacePoint.position.y;

        if (isOnSurface && direction.y > 0 && !isJumping)
        {
            direction.y = 0;
        }

        if (transform.position.y > waterSurfacePoint.position.y)
        {
            rb.gravityScale = defaultGravityScale;
        }
        else
        {
            rb.gravityScale = underwaterGravityScale;
        }

        bool canJump = Physics2D.OverlapCircle(jumpCheckPoint.position, 0.1f, raftLayer);

        if (Input.GetKeyDown(KeyCode.Space) && (isOnRaft || isOnSurface) && canJump)
        {
            StartCoroutine(Jump());
        }

        if (isInWater && !isJumping)
        {
            if (direction.magnitude > 0)
            {
                if (direction.y > 0)
                {
                    animator.Play("PlayerSwimUp");
                }
                else if (direction.y < 0)
                {
                    animator.Play("PlayerSwimDown");
                }
                else
                {
                    animator.Play("PlayerSwim");
                }
            }
            else
            {
                animator.Play("PlayerIdle");
            }
        }

        if (direction.x < 0 && FacingRight)
        {
            Flip();
        }
        else if (direction.x > 0 && !FacingRight)
        {
            Flip();
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

            if (!isJumping)
            {
                animator.Play("PlayerIdle");
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Raft"))
        {
            isOnRaft = false;
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
