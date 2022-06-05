using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image clock;
    private void Start()
    {
        EventSystem.current.OnGettingBonus += ResetTimer;
        EventSystem.current.OnGrapplingHookDeactivation += ResetGrapplingHook;
        clock.fillOrigin = (int)Image.Origin360.Top;
        clock.fillMethod = Image.FillMethod.Radial360;
    }
    
    
    private void ResetGrapplingHook(float cooldownTime)
    {
        clock.fillAmount = 0;
        StartFilling(cooldownTime);
    }

    public void ResetTimer()
    {
        clock.fillAmount = 1;
    }

    public void StartFilling(float cooldownTime)
    {
        StartCoroutine(StartFillingRoutine(cooldownTime));
    }

    private IEnumerator StartFillingRoutine(float cooldownTime)
    {
        float t = 0;
        while (clock.fillAmount < 1)
        {
            t += Time.deltaTime;
            clock.fillAmount = t/cooldownTime;
            yield return null;
        }
    }
}