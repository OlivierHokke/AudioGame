using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Story : MonoBehaviour {

    [Header("Important objects")]
    public GameObject Player;
    public GameObject Lucy;
	public GameObject Mallum;
    [Header("Important sounds")]
    public AudioClip LucyBell;

    /// <summary>
    /// Lucy explains what happened to the player
    /// </summary>
    [Header("Tutorial")]
    public LucyExplainingState LucyExplains1 = new LucyExplainingState();
    /// <summary>
    /// Lucy shows moves a bit to teach the player movement controls
    /// </summary>
    public SimpleFollowLucyState InitialMove = new SimpleFollowLucyState();
    /// <summary>
    /// Lucy moves to the door, inviting the player to open it
    /// </summary>
    public SimpleFollowLucyState SecondMove = new SimpleFollowLucyState();
    public LucyExplainingState LucyExplains2 = new LucyExplainingState();
    /// <summary>
    /// The door is opened when the player moved to it
    /// </summary>
    public LucyRemoveObjectState RemoveDoor = new LucyRemoveObjectState();

    /// <summary>
    /// The player hears the parents talking (screaming) but still has to follow lucy, who is at the next door
    /// </summary>
    [Header("Level 1")]
    public FollowLucyWithTalkingParentsState ParentRoomState = new FollowLucyWithTalkingParentsState();
	public LucyRemoveObjectState RemoveFlatDoor = new LucyRemoveObjectState();

	/// <summary>
	/// Leave the flat and follow lucy to the elevator door
	/// </summary>
	public SimpleFollowLucyState FollowLucyToElevator = new SimpleFollowLucyState ();
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

    public StepwiseFollowLucyState WalkPastBuilding1 = new StepwiseFollowLucyState();
    public StepwiseFollowLucyState WalkPastBuilding2 = new StepwiseFollowLucyState();

    /// <summary>
    /// Lucy explains to the player that he/she has to cross some more roads
    /// </summary>
    public LucyExplainingState LucyExplainsToCrossOtherRoad = new LucyExplainingState();
    /// <summary>
    /// The player has to move over to the next position while evading cars
    /// </summary>
    public CarEvasionState EvadeFirstRoadCars = new CarEvasionState();
	public CarEvasionState EvadeSecondRoadCars = new CarEvasionState();

    public StepwiseFollowLucyState WalkToPortal = new StepwiseFollowLucyState();

    public LucyExplainingState LucyExplainsPortal = new LucyExplainingState();
    /// <summary>
    /// The player reached the end of the level and is transported to a magical forest.
    /// </summary>
    public PortalState PortalState = new PortalState();

    [Header("Level 2")]
    public LucyExplainingState ExplainsFortress = new LucyExplainingState();
    public SingPuzzleState SingPuzzle = new SingPuzzleState();
    public CharacterExplainingState RubyExplainsSomething = new CharacterExplainingState();
    public StepwiseFollowLucyState FollowLucyToMines1 = new StepwiseFollowLucyState();
    public StepwiseFollowLucyState FollowLucyToMines2 = new StepwiseFollowLucyState();
    public MinesPuzzleState MinesPuzzle = new MinesPuzzleState();
    public StepwiseFollowLucyState FollowLucyToBoss = new StepwiseFollowLucyState();
    public LucyExplainingState LucyExplainsBoss = new LucyExplainingState();
    public FinalBossState FinalBoss = new FinalBossState();
    public LucyExplainingState LucyExplainsYouWin = new LucyExplainingState();
    public EndState EndState = new EndState();
 
       
    // Current state that we are in
    private BaseState currentState;

    // Load the start state
    void Start()
    {
        LoadState(LucyExplains1);

        // Define for some states that require it what the next state is.
        LucyExplains1.NextState = InitialMove;
        InitialMove.NextState = SecondMove;
        SecondMove.NextState = RemoveDoor;
        // Level 1
        RemoveDoor.NextState = ParentRoomState;
        ParentRoomState.NextState = LucyExplains2;
        LucyExplains2.NextState = RemoveFlatDoor;
		RemoveFlatDoor.NextState = FollowLucyToElevator;
		FollowLucyToElevator.NextState = RemoveElevatorDoor;
		RemoveElevatorDoor.NextState = ElevatorState;
		ElevatorState.NextState = LucyExplainsCars;
        LucyExplainsCars.NextState = EvadeFirstRoad;
        EvadeFirstRoad.NextState = WalkPastBuilding1;
        WalkPastBuilding1.NextState = WalkPastBuilding2;
        WalkPastBuilding2.NextState = LucyExplainsToCrossOtherRoad;
        LucyExplainsToCrossOtherRoad.NextState = EvadeFirstRoadCars;
		EvadeFirstRoadCars.NextState = EvadeSecondRoadCars;
		EvadeSecondRoadCars.NextState=WalkToPortal;
        WalkToPortal.NextState = LucyExplainsPortal;
        LucyExplainsPortal.NextState = PortalState;
        PortalState.NextState = ExplainsFortress;
        // Level 2
        ExplainsFortress.NextState = SingPuzzle;
        SingPuzzle.NextState = RubyExplainsSomething;
        RubyExplainsSomething.NextState = FollowLucyToMines1;
        FollowLucyToMines1.NextState = FollowLucyToMines2;
        FollowLucyToMines2.NextState = MinesPuzzle;
        MinesPuzzle.NextState = FollowLucyToBoss;
        FollowLucyToBoss.NextState = LucyExplainsBoss;
        LucyExplainsBoss.NextState = FinalBoss;
        FinalBoss.NextState = LucyExplainsYouWin;
        LucyExplainsYouWin.NextState = EndState;

        Player.GetComponent<PlayerController>().TriggerEntered += OnPlayerEnteredTrigger;
	}

    void OnPlayerEnteredTrigger(object sender, TriggerEventArgs e)
    {
        currentState.PlayerEnteredTrigger(e.Trigger, this);
    }
	
	void Update () 
    {
        if (currentState != null)
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
