using UnityEngine;
using System.Collections;


[RequireComponent (typeof(PlayerScore))]
public class CallWind : MonoBehaviour {

	public GameObject windForce;
	public float force;
	public float windForceTTL;

	PlayerScore ps;


	void Awake() {
		ps = GetComponent<PlayerScore>();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (ps.WindOrbs > 0) {
			if (Input.GetButtonDown("Fire1")) {
				SummonWind(Quaternion.Euler(new Vector3(0f, 0f, 90f)), new Vector2(0, force));
			} else if (Input.GetButtonDown("Fire2")) {
				SummonWind(Quaternion.Euler(new Vector3(0f, 0f, 0f)), new Vector2(force, 0));
			} else if (Input.GetButtonDown("Fire3")) {
				SummonWind(Quaternion.Euler(new Vector3(0f, 0f, 180f)), new Vector2(-force, 0f));
			}
		}
	}

	void SummonWind(Quaternion rot, Vector2 force) {
		GameObject wf = Instantiate(windForce, transform.position, rot) as GameObject;
		wf.GetComponent<WindComponent> ().Force = force;
		Invoke("DestroyWF", wf.GetComponent<WindComponent> ().windTTL);
		ps.MinusWindOrb();
	}

	void DestroyWF() {
		ps.PlusWindOrb();
	}
}
