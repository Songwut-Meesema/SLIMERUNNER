using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    private Transform player;
    public float chaseSpeed = 2f;
    public float jumpforce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isChasing = false;

    public int damage = 1;

    [Header("Chase Settings")]
    public float detectionRange = 5f;      

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            
            ChasePlayer();
        }
        else
        {
            
            rb.velocity = Vector2.zero;  
        }
    }

    private void ChasePlayer()
    {
        
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        
        
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        
        if (transform.position.y < player.position.y)
        {
            rb.velocity = new Vector2(rb.velocity.x, chaseSpeed); 
        }
        else if (transform.position.y > player.position.y)
        {
            rb.velocity = new Vector2(rb.velocity.x, -chaseSpeed); 
        }

        
        RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
        RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);

        if (!groundInFront.collider && !gapAhead.collider)
        {
            
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }
    }
}
