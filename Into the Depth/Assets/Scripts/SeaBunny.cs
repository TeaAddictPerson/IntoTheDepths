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
    public float pickUpRadius = 2f;  
    public int seaweedGoal = 2; 
    private int seaweedEaten = 0; 
    public Transform player; 
    private bool isFollowingPlayer = false; 
    public float followingDistance = 1f;
    public Transform waterSurfacePoint;
    private float waterSurfaceY;

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
            FollowPlayer();
        }
        else
        {
            FindAndPickUpSeaweed(); 
        }

        if (transform.position.y > waterSurfaceY)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = waterSurfaceY;
            transform.position = newPosition;
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

        Rigidbody2D[] droppedItems = FindObjectsOfType<Rigidbody2D>();

        foreach (Rigidbody2D itemRb in droppedItems)
        {
            GameObject item = itemRb.gameObject;

       
            SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) continue;

           
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
       
        return itemSprite != null && itemSprite.name.Contains("small_stuff_5"); 
    }

    private HashSet<GameObject> eatenSeaweed = new HashSet<GameObject>();

    private IEnumerator MoveToAndPickUp(GameObject item)
    {
        if (eatenSeaweed.Contains(item))
        {
            Debug.Log("Этот объект уже съеден, пропускаем его.");
            yield break;
        }

        Debug.Log("Заяц двигается к объекту: " + item.name + " на позицию: " + item.transform.position);

        while (item != null && Vector2.Distance(transform.position, item.transform.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, item.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (item != null && !eatenSeaweed.Contains(item))
        {
            Debug.Log("Заяц достигает объекта и уничтожает его: " + item.name); 
            eatenSeaweed.Add(item);
            Destroy(item);
            seaweedEaten++; 
            Debug.Log("Заяц съел водоросли! Всего съедено: " + seaweedEaten); 

            if (seaweedEaten >= seaweedGoal)
            {
                isFollowingPlayer = true;
                speed = 3f; 
                Debug.Log("Заяц съел достаточно водорослей, теперь он будет следовать за игроком.");
            }
        }
        else
        {
            Debug.Log("Объект водорослей был уничтожен или не найден в процессе."); 
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
           
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

     
            if (distanceToPlayer > followingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }

            Vector2 direction = (player.position - transform.position).normalized;
            CheckDirection(direction);
        }
    }
}
