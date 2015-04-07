using UnityEngine;
using System.Collections;

[RequireComponent (typeof(SpriteRenderer))]


public class Tiling : MonoBehaviour {

	public int offsetX = 2;
	public bool hasARightBuddy = false;
	public bool hasALeftBuddy = false;

	public bool reverseScale = false;
	private float spriteWidth = 0f;

	private Camera cam;
	private Transform myTransform;
	Tiling leftBuddy;
	Tiling rightBuddy;

	void Awake() {
		cam = Camera.main;
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		SpriteRenderer sRenderer = GetComponent<SpriteRenderer> ();
		spriteWidth = sRenderer.sprite.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
		// calculate the camera extend (half the width) of what the camera can see in world coordinates
		float camHorizontalExtend = cam.orthographicSize * (Screen.width/Screen.height);
		if (!hasALeftBuddy || !hasARightBuddy) {

			// calculate the x position where the camera can see the edge of the sprite (element)
			float edgeVisiblePositionRight = ((myTransform.position.x + spriteWidth) / 2) - camHorizontalExtend;
			float edgeVisiblePositionLeft = ((myTransform.position.x - spriteWidth) / 2) + camHorizontalExtend;

			if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasARightBuddy) {
				MakeNewBuddy(1);
				hasARightBuddy = true;

			} else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasALeftBuddy) {
				MakeNewBuddy(-1);
				hasALeftBuddy = true;
			}
		}
	}

	void MakeNewBuddy(int rightOrLeft) {
		Vector3 newPosition = new Vector3 (myTransform.position.x + spriteWidth * rightOrLeft, myTransform.position.y, myTransform.position.z);
		Transform newBuddy = Instantiate (myTransform, newPosition, myTransform.rotation) as Transform;

		if (reverseScale) {
			newBuddy.localScale = new Vector3 (newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
		}

		newBuddy.parent = myTransform.parent;
		if (rightOrLeft > 0) {
			rightBuddy = newBuddy.GetComponent<Tiling> ();
			rightBuddy.hasALeftBuddy = true;
			rightBuddy.leftBuddy = GetComponent<Tiling>();
		} else {
			leftBuddy = newBuddy.GetComponent<Tiling> ();
			leftBuddy.hasARightBuddy = true;
			leftBuddy.rightBuddy = GetComponent<Tiling>();
		}
	}
}
