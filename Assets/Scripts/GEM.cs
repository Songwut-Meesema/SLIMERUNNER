using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GEM : MonoBehaviour,UItems
{
    public static event Action<int> OnGemCollect;
    public int worth = 5;
    public void Collect()
    {
       OnGemCollect.Invoke(worth);
       SoundEffecManager.Play("Gem");
       Destroy(gameObject);
    }
}
