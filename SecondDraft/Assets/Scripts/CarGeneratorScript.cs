using UnityEngine;
using System.Collections.Generic;

public class CarGeneratorScript : MonoBehaviour {

	public CarScript car;
	public GameObject spawnPosition;
	public GameObject carLiveArea;
	public float carSpeed;
	public GameObject street;
	public bool isActive;	

	private const float MIN_RANDOM_DELAY = 1.0f;
	private const float MAX_RANDOM_DELAY = 7.0f;
	private float timeUntilNextCar;

	// Use this for initialization
	void Start () {
		timeUntilNextCar = 0;
		isActive = true;
	}
	
	// Update is called once per frame
	void Update () {
		timeUntilNextCar -= Time.deltaTime;

		if (timeUntilNextCar < 0 && isActive) {
			timeUntilNextCar = Randomg.Range(MIN_RANDOM_DELAY, MAX_RANDOM_DELAY);
			CarScript newCar = Instantiate(car, spawnPosition.transform.position, spawnPosition.transform.rotation) as CarScript;
			newCar.Init(this, carSpeed);
			newCar.transform.parent = street.transform;
		}
	}

	public void SetActive(bool state) {
		isActive = state;
	}
}
