using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {

	public Sprite leverOn;
	public GameObject targetActivate;
	public GameObject windZoneTarget;
	public float timeToKillWind;
	WindZone wz;
	bool killWind = false;
	float initialWindMain;

	// Use this for initialization
	void Start () {
		wz = windZoneTarget.GetComponent<WindZone> ();
		initialWindMain = wz.windMain;
	}
	
	// Update is called once per frame
	void Update () {
		if (killWind) {
			wz.windMain = Mathf.Lerp(initialWindMain, 0f, timeToKillWind);
		}
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			GetComponent<SpriteRenderer>().sprite = leverOn;
			targetActivate.GetComponent<WindComponent>().activateWind();
			killWind = true;
		}
	}
}
