using UnityEngine;
using System.Collections;

public class MallumController : MonoBehaviour {

	public GameObject target;
	public GameObject fireball;
	public GameObject powerUp;
	public GameObject spawnPosition;
	public GameObject spellLiveArea;
	public GameObject MallumRoom;
	public float spellSpeed;
	public bool isAwaken;

	private int spellsThrown;

	private const float MIN_RANDOM_DELAY = 1.0f;
	private const float MAX_RANDOM_DELAY = 1.5f;
	private float timeUntilNextFireball;

	// Use this for initialization
	void Start () {
		isAwaken = true;
		spellsThrown = 0;
		timeUntilNextFireball = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timeUntilNextFireball -= Time.deltaTime;
		
		if (timeUntilNextFireball < 0 && isAwaken) {
			timeUntilNextFireball = Randomg.Range(MIN_RANDOM_DELAY, MAX_RANDOM_DELAY);

			GameObject spellGO = null;
			SpellController spellCS = null;

			if (spellsThrown < 3) {
				spellGO = Instantiate(fireball, spawnPosition.transform.position, spawnPosition.transform.rotation) as GameObject;
				spellCS = spellGO.GetComponent<FireballController>();
				spellsThrown++;
			} else {
				spellGO = Instantiate(powerUp, spawnPosition.transform.position, spawnPosition.transform.rotation) as GameObject;
				spellCS = spellGO.GetComponent<PowerUpController>();
				spellsThrown = 0;
			}

			spellGO.transform.parent = MallumRoom.transform;
			spellGO.transform.LookAt(target.transform);
			spellCS.Init(this, spellSpeed);
		}
	}
	
	public void setAwaken(bool state) {
		isAwaken = state;
	}
}
