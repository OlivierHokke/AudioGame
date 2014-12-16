using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


[Serializable]
public class CarGeneratorScript : MonoBehaviour {

	public GameObject car;
	public GameObject spawnPosition;
	public GameObject carLiveArea;
	public float carSpeed;
	public GameObject street;
	public bool isActive;	
	private List<CarScript> listCarsActive;
	public CarScript[] ListCarsActive
	{
		get { return listCarsActive.ToArray (); } 
	}

	private const float MIN_RANDOM_DELAY = 1.0f;
	private const float MAX_RANDOM_DELAY = 7.0f;
	private float timeUntilNextCar;

	// Use this for initialization
	void Start () {
		timeUntilNextCar = 0;
		isActive = true;
		listCarsActive = new List<CarScript>();
	}
	
	// Update is called once per frame
	void Update () {
		timeUntilNextCar -= Time.deltaTime;

		if (timeUntilNextCar < 0 && isActive) {
			timeUntilNextCar = Randomg.Range(MIN_RANDOM_DELAY, MAX_RANDOM_DELAY);
            GameObject carGO = Instantiate(car, spawnPosition.transform.position, spawnPosition.transform.rotation) as GameObject;
            CarScript carCS = carGO.GetComponent<CarScript>();
            carCS.Init(this, carSpeed);
            carGO.transform.parent = street.transform;
            listCarsActive.Add(carCS);
		}
	}

	public void SetActive(bool state) {
		isActive = state;
	}

	public void removeCarAt(int i){
		listCarsActive.RemoveAt (i);
	}
	/*
	public void changeSpeedCarAt(int i, float speed){
		ListCarsActive.IndexOf (i).changeSpeed (0f);
	}
	*/
}
