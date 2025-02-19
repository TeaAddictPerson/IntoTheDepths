using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    [Header("Здоровье")]
    public int maxHealth = 15;
    public int currentHealth;
    public bool isDead = false;

    public Image Bar;

    [Header("Атака")]
    public Transform attackPoint;
    public float AttackRange = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

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
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = defaultGravityScale;
    }

void Update()
{
    if (isDead) return;

direction = new Vector2(
    Input.GetAxisRaw("Horizontal"),
    Input.GetAxisRaw("Vertical")
).normalized;

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

    if (Input.GetMouseButtonDown(0))
    {
        Attack();
        nextAttackTime = Time.time + 1f / attackRate;
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt")) return;


  if (isInWater && !isJumping)
{
    bool hasVerticalInput = Mathf.Abs(direction.y) > 0.1f;
    bool hasHorizontalInput = Mathf.Abs(direction.x) > 0.1f;
    
    animator.SetBool("IsMoving", direction.magnitude > 0.1f);
    animator.SetFloat("velocityY", hasVerticalInput ? direction.y : 0);
    animator.SetFloat("velocityX", hasHorizontalInput ? Mathf.Abs(direction.x) : 0);
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
        if (!isDead && !isJumping)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        if (!isJumping)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    bool IsHurtAnimationPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsTag("Hurt");
    }

public void TakeDamage(int damage)
{
    Debug.Log($"Получен урон: {damage}. текущее хп: {currentHealth}");

    if (isDead)
    {
        Debug.Log("Дамаг не прошел, игрок умер или в инвизе");
        return;
    }

    currentHealth -= damage;
    Bar.fillAmount = (float)currentHealth / maxHealth;

    Debug.Log($"Хп после урона: {currentHealth}");

    animator.ResetTrigger("HurtSwimUp");
    animator.ResetTrigger("HurtSwimDown");
    animator.ResetTrigger("HurtSwim");
    animator.ResetTrigger("HurtIdle");

    if (direction.y > 0)
    {
        Debug.Log("Запуск анимации: HurtSwimUp");
        animator.SetTrigger("HurtSwimUp");
    }
    else if (direction.y < 0)
    {
        Debug.Log("Запуск анимации: HurtSwimDown");
        animator.SetTrigger("HurtSwimDown");
    }
    else if (direction.x != 0)
    {
        Debug.Log("Запуск анимации: HurtSwim");
        animator.SetTrigger("HurtSwim");
    }
    else
    {
        Debug.Log("Запуск анимации: HurtIdle");
        animator.SetTrigger("HurtIdle");
    }

    StartCoroutine(ResetHurtAnimation());

    if (currentHealth <= 0)
    {
        Die();
    }
}

    void Attack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + 1f / attackRate;
        animator.SetTrigger("Attack");

        StartCoroutine(PlayAttackAnimation());
    }

    private bool isAttacking = false; 
    private bool wasAttacking = false; 

    private IEnumerator PlayAttackAnimation()
    {
        if (isAttacking) yield break; 
        isAttacking = true; 
        wasAttacking = true; 

        yield return null;

        string attackAnimation;

        if (direction.y > 0)
        {
            attackAnimation = "PlayerAttackSwimUp";
        }
        else if (direction.y < 0)
        {
            attackAnimation = "PlayerAttackSwimDown";
        }
        else if (direction.x != 0)
        {
            attackAnimation = "PlayerAttackSwim";
        }
        else
        {
            attackAnimation = "PlayerAttackIdle";
        }

        animator.Play(attackAnimation);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, AttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }


        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false; 

        ResetToMovementState();
    }

    private void ResetToMovementState()
    {
        if (isAttacking) return; 

        if (direction.magnitude > 0.1f) 
        {
            wasAttacking = false; 
        }

        if (wasAttacking)
        {
            wasAttacking = false; 
            return;
        }

        if (direction.y > 0)
        {
            animator.Play("PlayerSwimUp");
        }
        else if (direction.y < 0)
        {
            animator.Play("PlayerSwimDown");
        }
        else if (direction.x != 0)
        {
            animator.Play("PlayerSwim");
        }
        else
        {
            animator.Play("PlayerIdle");
        }
    }

    private IEnumerator ResetHurtAnimation()
{
    yield return new WaitForSeconds(0.5f);
    animator.ResetTrigger("HurtSwimUp");
    animator.ResetTrigger("HurtSwimDown");
    animator.ResetTrigger("HurtSwim");
    animator.ResetTrigger("HurtIdle");
}

    public void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        rb.velocity = Vector2.zero;
        moveSpeed = 0; 
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
