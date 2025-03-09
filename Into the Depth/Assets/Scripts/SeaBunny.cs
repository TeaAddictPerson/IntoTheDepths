using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaBunny : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    private int currentWaypointIndex = 0;
    private bool FacingRight = false;
    private float zPosition = 10f;
    public float pickUpRadius = 2f;  // Радиус поиска предметов
    public int seaweedGoal = 2; // Количество водорослей, которое нужно съесть
    private int seaweedEaten = 0; // Количество съеденных водорослей
    public Transform player; // Игрок, за которым заяц будет следовать
    private bool isFollowingPlayer = false; // Флаг, который указывает, следует ли заяц за игроком
    public float followingDistance = 3f; // Расстояние, на котором заяц будет следовать за игроком

    void Start()
    {
        if (waypoints.Length > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    void Update()
    {
        if (isFollowingPlayer)
        {
            FollowPlayer(); // Если заяц должен следовать за игроком
        }
        else
        {
            FindAndPickUpSeaweed(); // Ищем водоросли
        }
    }

    IEnumerator MoveAlongPath()
    {
        while (!isFollowingPlayer)
        {
            Vector2 currentWaypoint = new Vector2(waypoints[currentWaypointIndex].position.x, waypoints[currentWaypointIndex].position.y);
            Vector2 nextWaypoint = new Vector2(waypoints[(currentWaypointIndex + 1) % waypoints.Length].position.x, waypoints[(currentWaypointIndex + 1) % waypoints.Length].position.y);

            float journeyLength = Vector2.Distance(currentWaypoint, nextWaypoint);
            float startTime = Time.time;

            while (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), nextWaypoint) > 0.01f)
            {
                float distanceCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distanceCovered / journeyLength;

                Vector3 newPosition = Vector2.Lerp(currentWaypoint, nextWaypoint, fractionOfJourney);
                newPosition.z = zPosition;
                transform.position = newPosition;

                Vector2 direction = (nextWaypoint - new Vector2(transform.position.x, transform.position.y)).normalized;
                CheckDirection(direction);

                yield return null;
            }

            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void CheckDirection(Vector2 direction)
    {
        if (direction.x < 0 && FacingRight)
        {
            Flip();
        }
        else if (direction.x > 0 && !FacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void FindAndPickUpSeaweed()
    {
        // Ищем все объекты с компонентом Rigidbody2D (падающие предметы)
        Rigidbody2D[] droppedItems = FindObjectsOfType<Rigidbody2D>();

        foreach (Rigidbody2D itemRb in droppedItems)
        {
            GameObject item = itemRb.gameObject;

            // Проверяем, есть ли у предмета SpriteRenderer
            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) continue;

            // Проверяем, является ли этот предмет водорослями (по спрайту)
            if (IsSeaweed(spriteRenderer.sprite))
            {
                float distance = Vector2.Distance(transform.position, item.transform.position);

                if (distance <= pickUpRadius)
                {
                    StartCoroutine(MoveToAndPickUp(item));
                    break;
                }
            }
        }
    }

    private bool IsSeaweed(Sprite itemSprite)
    {
        // Тут можно проверить по конкретному спрайту водорослей
        return itemSprite != null && itemSprite.name.Contains("small_stuff_5"); // Подставь имя спрайта водорослей
    }

    private IEnumerator MoveToAndPickUp(GameObject item)
    {
        while (item != null && Vector2.Distance(transform.position, item.transform.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, item.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (item != null) // Проверяем, существует ли объект перед уничтожением
        {
            Destroy(item);
            seaweedEaten++; // Увеличиваем счетчик съеденных водорослей
            Debug.Log("Заяц съел водоросли! Всего съедено: " + seaweedEaten);

            // Если заяц съел достаточно водорослей, начинает следовать за игроком
            if (seaweedEaten >= seaweedGoal)
            {
                isFollowingPlayer = true;
                speed = 3f; // Увеличиваем скорость после того, как заяц приручен
                Debug.Log("Заяц съел достаточно водорослей, теперь он будет следовать за игроком.");
            }
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            // Рассчитываем расстояние между зайцем и игроком
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Если заяц слишком близко, отступаем
            if (distanceToPlayer > followingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }

            Vector2 direction = (player.position - transform.position).normalized;
            CheckDirection(direction);
        }
    }
}
