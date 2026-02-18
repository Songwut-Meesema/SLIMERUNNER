using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.forward; 
    public float rotationSpeed = 50f;             
    public bool rotateClockwise = true;           

    [Header("Player Interaction")]
    public LayerMask playerLayer;                
    private Collider2D platformCollider;         
    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        RotatePlatform();
    }

    private void RotatePlatform()
    {
        float direction = rotateClockwise ? -1f : 1f; 
        transform.Rotate(rotationAxis * rotationSpeed * direction * Time.deltaTime);

        
        PushPlayers();
    }

    private void PushPlayers()
    {
        
        Collider2D[] playersOnPlatform = Physics2D.OverlapBoxAll(platformCollider.bounds.center, platformCollider.bounds.size, 0f, playerLayer);

        foreach (Collider2D player in playersOnPlatform)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 tangentialVelocity = CalculateTangentialVelocity(player.transform.position);
                playerRb.velocity += tangentialVelocity * Time.deltaTime; 
            }
        }
    }

    private Vector2 CalculateTangentialVelocity(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = playerPosition - transform.position; 
        Vector3 tangentialDirection = Vector3.Cross(directionToPlayer.normalized, rotationAxis); 
        float speed = rotationSpeed * Mathf.Deg2Rad * directionToPlayer.magnitude; 
        return tangentialDirection * speed * (rotateClockwise ? 1 : -1); 
    }

    private void OnDrawGizmosSelected()
    {
        if (platformCollider == null) platformCollider = GetComponent<Collider2D>();

        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(platformCollider.bounds.center, platformCollider.bounds.size);
    }
}
