using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    // transitions of each state
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type,List<Transition>>();
    // transitions of current state
    private List<Transition> _currentTransitions = new List<Transition>();
    // transitions that are relevant to all states
    private List<Transition> _anyTransitions = new List<Transition>();
    private List<Transition> _emptyTransitions = new List<Transition>();

    public IState GetCurrentState()
    {
        return currentState;
    }
    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);
      
        currentState?.Tick();
    }

    public void AddTransition(IState from,IState t0,Func<bool> predicate)
    {
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }
        transitions.Add(new Transition(t0,predicate));
    }
    public void SetState(IState state)
    {
        if (state == currentState ) return; // ==, equals, referenceEquals and MonoBehaviour?
        currentState?.OnExit();
        currentState = state;
        _transitions.TryGetValue(currentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
        {
            _currentTransitions = _emptyTransitions;
        }
    }

    private Transition GetTransition()
    {
        foreach(var transition in _anyTransitions)
            if (transition.Condition())
                return transition;
      
        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
    public class Transition
    {
        public Func<bool> Condition {get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }
}





