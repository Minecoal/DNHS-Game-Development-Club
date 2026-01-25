using System;
using System.Collections;
using UnityEngine;

public class TimeController : PersistentGenericSingleton<TimeController>
{   
    private static bool isFading = false;
    
    static public IEnumerator TimeStop(float realtimeDuration, float stopPercent = 0f)
    {
        if (isFading) yield break;
        isFading = true;
        Time.timeScale = stopPercent;
        yield return new WaitForSecondsRealtime(realtimeDuration);
        Time.timeScale = 1f;
        isFading = false;
    }
    
    private const float e = (float)Math.E;
    /// <summary>
    /// input limit [0, 1]
    /// intensity = 0, linear
    /// intensity < 0, concave-up / fade in
    /// intensity > 0, concave-down / fade out
    /// </summary>
    static private float PowerCurve(float input, float intensity)
    {
        input = Mathf.Clamp01(input);
        return Mathf.Pow(input, Mathf.Pow(e, - intensity));
    }
    
    /// <summary>
    /// intensity = 0, linear
    /// intensity < 0, time stops fast then slow
    /// intensity > 0, time stop slow then fast
    /// </summary>
    static public IEnumerator TimeStopFadeOut(float realTimeDuration, float intensity, float startScale)
    {
        if (isFading) yield break;
        isFading = true;
        
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < realTimeDuration)
        {
            float t = (Time.unscaledTime - startTime) / realTimeDuration;
            float easedT = 1f - PowerCurve(1f - t, intensity); // just graph it on desmos
            Time.timeScale = Mathf.Lerp(startScale, 1f, easedT);
            yield return null;
        }
        Time.timeScale = 1f;
        isFading = false;
    }

    /// <summary>
    /// intensity = 0, linear
    /// intensity < 0, time stops fast then slow
    /// intensity > 0, time stop slow then fast
    /// </summary>
    static public IEnumerator TimeStopFadeIn(float realTimeDuration, float intensity, float endScale)
    {
        if (isFading) yield break;
        isFading = true;
        
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < realTimeDuration)
        {
            float t = (Time.unscaledTime - startTime) / realTimeDuration;
            float easedT = PowerCurve(1f - t, intensity); // same here
            Time.timeScale = Mathf.Lerp(1f, endScale, easedT);
            yield return null;
        }
        Time.timeScale = endScale;
        isFading = false;
    }
}
