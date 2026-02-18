using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisappearingPlatform : MonoBehaviour
{
    [Header("Timing Settings")]
    public float timeToDisappear = 3f; 
    public float timeToReappear = 5f; 

    [Header("Warning Effect")]
    public bool enableWarningEffect = true; 
    public Color warningColor = Color.red; 
    public float blinkInterval = 0.2f; 

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private Color originalColor;
    private bool isPlayerOnPlatform = false;

    private void Start()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            if (!isPlayerOnPlatform)
            {
                isPlayerOnPlatform = true;
                StartCoroutine(HandlePlatformDisappearance());
            }
        }
    }

    private IEnumerator HandlePlatformDisappearance()
    {
        if (enableWarningEffect && spriteRenderer != null)
        {
            yield return StartCoroutine(WarningEffect()); 
        }

        
        SetPlatformActive(false);

        
        yield return new WaitForSeconds(timeToReappear);

        
        SetPlatformActive(true);
        isPlayerOnPlatform = false;
    }

    private IEnumerator WarningEffect()
    {
        float elapsed = 0f;

        while (elapsed < timeToDisappear)
        {
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = (elapsed % (blinkInterval * 2) < blinkInterval) ? warningColor : originalColor;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void SetPlatformActive(bool isActive)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isActive; 
        }

        if (platformCollider != null)
        {
            platformCollider.enabled = isActive; 
        }
    }
}
