using UnityEngine;

public interface IState 
{
    public void Tick();
    public void OnExit();
}