using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D  rb;
    public Animator animator;
    bool isFacingRight = true;
    public ParticleSystem smokeFX;
    BoxCollider2D playerCollider;
    
    [Header ("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header ("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

     [Header ("Jumping")]
     public float JumpForce = 10f;
     public int maxJump = 2;
     int jumpRemaining;

     [Header ("GroundCheck")]
     public Transform groundCheck;
     public Vector2 groundCheckSize = new Vector2(0.49f,0.03f);
     public LayerMask groundLayer;
     bool isGrounded;
     bool isOnPlatform;

     [Header ("Gravity")]
     public float baseGravity = 2;
     public float maxFallSpeed = 18f;
     public float fallSpeedMultiplier = 2f;

     [Header ("WallCheck")]
     public Transform wallCheck;
     public Vector2 wallCheckSize = new Vector2(0.5f,0.05f);
     public LayerMask wallLayer;

     [Header ("WallMovement")]
     public float wallSlideSpeed = 2;
     bool isWallSlide;

    [Header ("Wall Jump")]
     bool isWalljumping;
     float wallJumpDirection;
     float wallJumpTime = 0.5f;
     float wallJumpTimer;
     public Vector2 wallJumpPower = new Vector2(5f,10f);

     private void Start() 
     {
        trailRenderer = GetComponent<TrailRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
     }

    void Update()
    {
        animator.SetFloat("yVelocity",rb.velocity.y);
        animator.SetFloat("magnitude",rb.velocity.magnitude);
        animator.SetBool("isWallSlide",isWallSlide);

        if(isDashing)
        {
            return;
        }
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        if(!isWalljumping)
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            Flip();
        }    
    }

    public void Move (InputAction.CallbackContext context) 
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash (InputAction.CallbackContext context) 
    {
        if(context.performed && canDash)
        {
            StartCoroutine(DashCaroutine());
        }
    }

    public void Drop (InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded && isOnPlatform && playerCollider.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.25f));
        }
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }

    private IEnumerator DashCaroutine()
    {
        Physics2D.IgnoreLayerCollision(7,8,true);
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;

        float dashDirection = isFacingRight ? 1f : -1f;

        rb.velocity = new Vector2(dashDirection * dashSpeed,rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = new Vector2(0f,rb.velocity.y);

        isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(7,8,false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Jump (InputAction.CallbackContext context)
    {
        if (jumpRemaining > 0) 
      {
        if(context.performed) 
        {
            rb.velocity = new Vector2(rb.velocity.x,JumpForce);
            jumpRemaining--;
            JumpFX();
        }
        else if (context.canceled)
        {
            rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y * 0.5f);
            jumpRemaining--;
            JumpFX();
        }
      }

      if(context.performed && wallJumpTimer >0f) 
      {
        isWalljumping = true;
        rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); 
        wallJumpTimer = 0;
        JumpFX();

        //จับหัน
        if(transform.localScale.x != wallJumpDirection)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

        Invoke(nameof(CancelWallJump),wallJumpTime + 0.1f); 
      }
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();
    }
    private void GroundCheck ()
    {
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0 ,groundLayer))
        {
            jumpRemaining = maxJump;
            isGrounded = true;
        }  
        else
        {
            isGrounded = false;
        }       
    }

    private bool WallCheck() 
    {
        return Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0 ,wallLayer);
    }

    private void ProcessGravity()
    {
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; 
            rb.velocity = new Vector2(rb.velocity.x,Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if(!isGrounded & WallCheck() & horizontalMovement !=0)
        {
            isWallSlide = true;
            rb.velocity = new Vector2 (rb.velocity.x,Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSlide = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSlide)
        {
            isWalljumping = false;
            wallJumpDirection = - transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump ()
    {
        isWalljumping = false;
    }

    private void Flip ()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0 )
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if(rb.velocity.y == 0)
            {
                smokeFX.Play();
            }        
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position,groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheck.position,wallCheckSize);
    }
}