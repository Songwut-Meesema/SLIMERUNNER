using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PushableObject : MonoBehaviour
{
    [Header("Physics Settings")]
    public float pushForce = 5f; 
    public float maxSpeed = 3f; 
    public float friction = 0.8f; 

    [Header("Axis Constraints")]
    public bool lockHorizontal = false; 
    public bool lockVertical = true; 

    private Rigidbody2D rb;
    private bool isPlayerNearby = false;
    private Transform player;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; 
        rb.freezeRotation = true; 
    }

    private void Update()
    {
        if (isPlayerNearby && player != null)
        {
            HandlePush();
        }
        ApplyFriction();
    }

    private void HandlePush()
    {
        Vector2 playerDirection = (Vector2)player.position - rb.position;
        playerDirection.Normalize();

        
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector2 pushDirection = new Vector2(inputHorizontal, lockVertical ? 0 : inputVertical);

        
        if (pushDirection.magnitude > 0)
        {
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Force);
        }

        
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void ApplyFriction()
    {
        
        Vector2 velocity = rb.velocity;
        velocity *= (1 - friction * Time.deltaTime);
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }
}
