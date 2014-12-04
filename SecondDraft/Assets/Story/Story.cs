using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Story : MonoBehaviour {

    public GameObject Player;
    public GameObject Lucy;

    [Header("Tutorial")]
    public LucyExplainingState LucyExplains;
    public SimpleFollowLucyState InitialMove;
    public SimpleFollowLucyState SecondMove;

    [Header("Level 1")]
    public SimpleFollowLucyState SomeOtherState;

    // Current state that we are in
    private BaseState currentState;

    // Let's load the start state
    void Start()
    {
        LoadState(StartState);
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
