using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1f;
    public Image fillcircle;

    private float holdtimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete;
    void Update()
    {
        if (isHolding)
        {
            holdtimer += Time.deltaTime;
            fillcircle.fillAmount = holdtimer / holdDuration;
            if (holdtimer >= holdDuration)
            {
                OnHoldComplete.Invoke();
                ResetHold();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false;
        holdtimer = 0;
        fillcircle.fillAmount = 0;
    }
}
