using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Story : MonoBehaviour {

    public GameObject player;

    [Header("Tutorial")]
    public StartState startState;
    public StartState someState;

    [Header("Level 1")]
    public StartState someOtherState;

    // Current state that we are in
    private BaseState currentState;

    // Let's load the start state
    void Start()
    {
        LoadState(startState);
	}
	
	void Update () 
    {
        currentState.Update(this);
	}

    public void LoadState(BaseState state)
    {
        if (currentState != null)
            currentState.End(this);

        currentState = state;
        currentState.Start(this);
    }
}
