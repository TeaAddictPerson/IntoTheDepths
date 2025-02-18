using UnityEngine;

public class SwordFish : MonoBehaviour, IDamageable
{
    public Animator animator;
    public int maxHealth = 10;
    private int currentHealth;
    private Collider2D fishCollider;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("Преследование")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    private Transform player;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        fishCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure player has 'Player' tag");
        }

        lastAttackTime = -attackCooldown;

        if (isDead)
        {
            SetupDeathState();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (currentHealth <= 0 || player == null || animator == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        var playerScript = player.GetComponent<PlayerScript>();
        if (playerScript != null && playerScript.isDead)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
            return;
        }

        if (distanceToPlayer < detectionRange)
        {
            animator.SetBool("IsRunning", true);
            ChasePlayer();

            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }


    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        var playerScript = player.GetComponent<PlayerScript>();

        if (playerScript != null)
        {
            playerScript.TakeDamage(1);
            animator.SetTrigger("Attack");
        }
        else
        {
            Debug.LogError("PlayerScript component not found on player object!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("IsDied", true);
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies");

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        if (fishCollider != null)
            fishCollider.enabled = false;

        PlayerScript playerScript = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerScript>();

    }

    void SetupDeathState()
    {
        animator.SetBool("IsDied", true);
        gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        if (fishCollider != null)
            fishCollider.enabled = false;
    }

    void OnBecameInvisible()
    {
        if (isDead && spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            if (animator != null)
                animator.enabled = false;
        }
    }

    void OnBecameVisible()
    {
        if (isDead && spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            if (animator != null)
                animator.enabled = true;
        }
    }
}
