using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    public float chaseSpeed = 2f;
    public float jumpforce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;

    public int damage = 1;

    [Header("Chase Settings")]
    public float detectionRange = 5f;      

    private bool isChasing = false;        

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
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

       
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer);

        if (isChasing)
        {
            
            ChasePlayer();
        }
        else
        {
            
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void ChasePlayer()
    {
        
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        
        
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        
        RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
        RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
        RaycastHit2D platformBeyond = Physics2D.Raycast(transform.position, Vector2.up, 5f, groundLayer);

        if (!groundInFront.collider && !gapAhead.collider)
        {
            shouldJump = true;
        }
        else if (platformBeyond.collider)
        {
            shouldJump = true;
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpforce;

            rb.AddForce(new Vector2(jumpDirection.x, jumpforce), ForceMode2D.Impulse);
        }
    }
}
