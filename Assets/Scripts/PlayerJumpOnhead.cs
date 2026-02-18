using UnityEngine;

public class PlayerJumpOnHead : MonoBehaviour
{
    public float bounceForce = 10f; 
    public GameObject itemDropPrefab; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            float playerY = transform.position.y;
            float enemyY = collision.transform.position.y;

            
            if (playerY > enemyY)
            {
                
                if (itemDropPrefab != null)
                {
                    
                    Instantiate(itemDropPrefab, collision.transform.position, Quaternion.identity);
                }

                
                Destroy(collision.gameObject);

                
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(rb.velocity.x, bounceForce);
                }
            }
        }
    }
}
