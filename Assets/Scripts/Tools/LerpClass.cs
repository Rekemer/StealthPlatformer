using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpClass : MonoBehaviour
{
    public enum InterType
    {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep,
        Parabola,
        Hyperbolic
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static float Lerp(float t, InterType interpolation)
    {
        
        switch (interpolation)
        {
            case InterType.Linear:
                break;
            case InterType.EaseOut:
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                break;
            case InterType.EaseIn:
                t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                break;
            case InterType.SmoothStep:
                t = t * t * (3 - 2 * t);
                break;
            case InterType.SmootherStep:
                t = t * t * t * (t * (t * 6 - 15) + 10);
                break;
            case InterType.Parabola:
                t = t * t*t *4;
                break;
            case InterType.Hyperbolic:
                t = (Mathf.Pow(2.7f,t) - Mathf.Pow(2.7f,-t))/2 * 5;
                break;
        }

        return t;
    }
    
}
