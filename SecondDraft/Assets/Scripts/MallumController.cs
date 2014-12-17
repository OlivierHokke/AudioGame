using UnityEngine;
using System.Collections;

public class MallumController : MonoBehaviour {

	public GameObject target;
	public GameObject fireball;
	public GameObject powerUp;
	public GameObject spawnPosition;
	public GameObject fireballLiveArea;
	public GameObject MallumRoom;
	public float spellSpeed;
	public bool isAwaken;

	private int fireballsThrown;

	private const float MIN_RANDOM_DELAY = 3.0f;
	private const float MAX_RANDOM_DELAY = 10.0f;
	private float timeUntilNextFireball;

	// Use this for initialization
	void Start () {
		isAwaken = true;
		fireballsThrown = 0;
		timeUntilNextFireball = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timeUntilNextFireball -= Time.deltaTime;
		
		if (timeUntilNextFireball < 0 && isAwaken) {
			timeUntilNextFireball = Randomg.Range(MIN_RANDOM_DELAY, MAX_RANDOM_DELAY);

			GameObject spellGO = null;

			if (fireballsThrown < 3) {
				spellGO = Instantiate(fireball, spawnPosition.transform.position, spawnPosition.transform.rotation) as GameObject;
			}

			spellGO.transform.LookAt(target.transform);
			SpellController spellCS = spellGO.GetComponent<FireballController>();
			spellCS.Init(this, spellSpeed);
			spellGO.transform.parent = MallumRoom.transform;

		}
	}
	
	public void setAwaken(bool state) {
		isAwaken = state;
	}
}
