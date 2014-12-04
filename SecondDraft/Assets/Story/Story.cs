using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Story : MonoBehaviour {

    public GameObject Player;
    public GameObject Lucy;

    [Header("Tutorial")]
    public LucyExplainingState LucyExplains = new LucyExplainingState();
    public SimpleFollowLucyState InitialMove = new SimpleFollowLucyState();
    public SimpleFollowLucyState SecondMove = new SimpleFollowLucyState();

    [Header("Level 1")]
    public SimpleFollowLucyState SomeOtherState = new SimpleFollowLucyState();

    // Current state that we are in
    private BaseState currentState;

    // Let's load the start state
    void Start()
    {
        LoadState(LucyExplains);
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
