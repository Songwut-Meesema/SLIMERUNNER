using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenArea : MonoBehaviour
{
    public float fadeduration = 1f;

    SpriteRenderer spriteRenderer;
    Color hiddenColor;
    Coroutine currentCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hiddenColor = spriteRenderer.color;
    }

   private void OnTriggerEnter2D(Collider2D collision) 
   {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeSprite(true));
        }
   }

   private void OnTriggerExit2D(Collider2D collision) 
   {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeSprite(false));
        }
   }

   private IEnumerator FadeSprite(bool fadeOut)
   {
    Color startColor = spriteRenderer.color;
    Color targetColor = fadeOut ? new Color(hiddenColor.r,hiddenColor.g,hiddenColor.b,0f) : hiddenColor;
    float timefading = 0f;

    while(timefading < fadeduration)
    {
        spriteRenderer.color = Color.Lerp(startColor,targetColor,timefading / fadeduration);
        timefading += Time.deltaTime;
        yield return null;
    }

    spriteRenderer.color = targetColor;
   }
}
