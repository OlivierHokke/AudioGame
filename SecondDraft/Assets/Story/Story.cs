using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Story : MonoBehaviour {

    public GameObject Player;
    public GameObject Lucy;

    /// <summary>
    /// Lucy explains what happened to the player
    /// </summary>
    [Header("Tutorial")]
    public LucyExplainingState LucyExplains = new LucyExplainingState();
    /// <summary>
    /// Lucy shows moves a bit to teach the player movement controls
    /// </summary>
    public SimpleFollowLucyState InitialMove = new SimpleFollowLucyState();
    /// <summary>
    /// Lucy moves to the door, inviting the player to open it
    /// </summary>
    public SimpleFollowLucyState SecondMove = new SimpleFollowLucyState();
    /// <summary>
    /// The door is opened when the player moved to it
    /// </summary>
    public LucyRemoveObjectState RemoveDoor = new LucyRemoveObjectState();

    /// <summary>
    /// The player hears the parents talking (screaming) but still has to follow lucy, who is at the next door
    /// </summary>
    [Header("Level 1")]
    public FollowLucyWithTalkingParentsState ParentRoomState = new FollowLucyWithTalkingParentsState();
    public LucyRemoveObjectState RemoveElevatorDoor = new LucyRemoveObjectState();
    /// <summary>
    /// The player is in the elevator, waiting to get to the street
    /// </summary>
    public ElevatorState ElevatorState = new ElevatorState();

    /// <summary>
    /// Lucy explains the cars
    /// </summary>
    public LucyExplainingState LucyExplainsCars = new LucyExplainingState();

    /// <summary>
    /// The player has to cross the first road with cars (or something) moving over it.
    /// </summary>
    public CarEvasionState EvadeFirstRoad = new CarEvasionState();

    /// <summary>
    /// Lucy explains to the player that he/she has to cross some more roads
    /// </summary>
    public LucyExplainingState LucyExplainsToCrossOtherRoad = new LucyExplainingState();
    /// <summary>
    /// The player has to move over to the next position while evading cars
    /// </summary>
    public CarRoundaboutEvasionState EvadeRoundaboutCars = new CarRoundaboutEvasionState();

    /// <summary>
    /// The player reached the end of the level and is transported to a magical forest.
    /// </summary>
    public PortalState PortalState = new PortalState();
 
       
    // Current state that we are in
    private BaseState currentState;

    // Load the start state
    void Start()
    {
        LoadState(LucyExplains);

        // Define for some states that require it what the next state is.
        InitialMove.NextState = SecondMove;
        SecondMove.NextState = RemoveDoor;
        // Level 1
        RemoveDoor.NextState = ParentRoomState;

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
