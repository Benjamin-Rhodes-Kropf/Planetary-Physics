using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //control the time scale of the game
    public float timeScale = 1.0f;
    
    void Update()
    {
        Time.timeScale = timeScale;
    }
}
