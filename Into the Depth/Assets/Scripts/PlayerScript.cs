using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed=5f;
    private Rigidbody2D rb;
    private Vector2 direction;
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position+direction*moveSpeed*Time.fixedDeltaTime);
    }
}
