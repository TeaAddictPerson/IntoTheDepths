﻿using UnityEngine;
using System.Collections;

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
    public float knockbackForce = 2f;
    public Transform waterSurfacePoint;
    private float waterSurfaceY;

    private float lastAttackTime;
    private Transform player;
    private bool isDead = false;
    private bool isKnockedBack = false;
    private Vector2 lastMoveDirection;

    [Header("Дроп предметов")]
    public GameObject meatPrefab;
    public GameObject bonePrefab;
    public int meatDropAmount = 2;
    public int boneDropAmount = 3;

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

        if (waterSurfacePoint != null)
        {
            waterSurfaceY = waterSurfacePoint.position.y;
        }
        else
        {
            Debug.LogError("WaterSurfacePoint is not assigned! Assign it in the Inspector.");
        }

        lastAttackTime = -attackCooldown;

        if (isDead)
        {
            SetupDeathState();
        }
    }

    void Update()
    {
        if (isDead || isKnockedBack) return;
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
                StartCoroutine(Attack());
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRunning", false);
        }

        if (transform.position.y > waterSurfaceY)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = waterSurfaceY;
            transform.position = newPosition;
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        lastMoveDirection = direction;
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

    IEnumerator Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) yield break;

        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Attack");

        var playerScript = player.GetComponent<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(1);
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(KnockbackAfterAttack());
    }

    IEnumerator KnockbackAfterAttack()
    {
        isKnockedBack = true;
        rb.velocity = lastMoveDirection * knockbackForce;
        yield return new WaitForSeconds(0.5f);
        isKnockedBack = false;
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

        DropLoot();
        Destroy(gameObject, 1f);
    }

    void DropLoot()
    {
        SpawnDroppedItems(meatPrefab, meatDropAmount);
        SpawnDroppedItems(bonePrefab, boneDropAmount);
    }

    void SpawnDroppedItems(GameObject itemPrefab, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
            GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);

            DroppedItem droppedItemScript = droppedItem.GetComponent<DroppedItem>();
            if (droppedItemScript == null)
            {
                Debug.LogError($"DroppedItem отсутствует на {itemPrefab.name}!");
            }
        }
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
