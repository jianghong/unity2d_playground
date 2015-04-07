using UnityEngine;
using System.Collections;

public class FinishLevel : MonoBehaviour {

    int maxSproutlings;

    public string nextLevel;

    void Awake() {
        maxSproutlings = GameObject.FindGameObjectsWithTag("Sproutling").Length;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (other.GetComponent<PlayerScore>().Sproutlings == maxSproutlings) {
                Application.LoadLevel(nextLevel);
            }
        }
    }
}
