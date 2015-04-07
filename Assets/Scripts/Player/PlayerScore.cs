using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScore : MonoBehaviour {

	private int sproutlings = 0;
	private int windOrbs = 0;

	public GameObject windOrbCount;
	public GameObject WorbCompanion;


	public int WindOrbs {
		get {
			return windOrbs;
		}
	}

	public int Sproutlings {
		get {
			return sproutlings;
		}
		set {
			sproutlings = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlusWindOrb() {
		windOrbs += 1;
		WorbCompanion.SetActive (true);
		windOrbCount.GetComponent<Text> ().text = "x" + windOrbs.ToString ();
	}

	public void MinusWindOrb() {
		windOrbs -= 1;
		windOrbCount.GetComponent<Text> ().text = "x" + windOrbs.ToString ();
		if (windOrbs == 0) {
			WorbCompanion.SetActive (false);
		}
	}
}
