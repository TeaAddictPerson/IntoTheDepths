using System.Collections;
using UnityEngine;

public class Ray : MonoBehaviour
{
    public Transform[] waypoints; 
    public float speed = 2f;
    private int currentWaypointIndex = 0;
    private bool FacingRight = true;
    private float zPosition = 10f;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        while (true)
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
}
