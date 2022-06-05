using System;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public delegate void MyDelegate(float coolDownTime);
    public static EventSystem current;
    public event Action  OnGettingBonus;
    public MyDelegate OnGrapplingHookDeactivation;
    public static  bool isPlayerHiding; // can be substituted with event

    private void Awake()
    {
        current = this;
    }

    public void ActivateBonus()
    {
        OnGettingBonus?.Invoke();
    }

    public void OnResetGrapplingHook(float cooldownTime)
    {
        OnGrapplingHookDeactivation?.Invoke(cooldownTime); 
    }
}