using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtility
{
    private static float timer;

    public static void StartTimer(float duration)
    {
        timer = duration;
    }

    public static bool TimerIsRunning()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return true;
        }
        return false;
    }
}
