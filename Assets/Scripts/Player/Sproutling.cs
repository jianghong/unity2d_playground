using UnityEngine;
using System.Collections;

public class Sproutling : MonoBehaviour {

	public float smoothTime = 0.3f;
	public float distanceOffset;

	Transform target;
	Transform m_thisTransform;
	BoxCollider2D m_boxCollider;
	bool m_FacingRight = true;
	float xOffset = 0f;
	float yOffset = 0f;

	void Awake() {
		m_boxCollider = GetComponent<BoxCollider2D>();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		m_thisTransform = transform;
		if (target) {
			Follow ();
		}
	}

	void Follow() {
		float distance = Vector2.Distance (target.position, m_thisTransform.position);
		if (TargetIsRight() && !m_FacingRight) {
			Flip ();
		} else if (!TargetIsRight() && m_FacingRight) {
			Flip ();
		}
		if (distance > distanceOffset) {
			float newX = Mathf.Lerp( m_thisTransform.position.x, target.position.x + xOffset, Time.deltaTime * smoothTime);
			float newY = Mathf.Lerp( m_thisTransform.position.y, target.position.y + yOffset, Time.deltaTime * smoothTime);

			m_thisTransform.position = new Vector3(newX, newY, 0f);
		}
	}

	void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	bool TargetIsRight() {
		return (target.position.x - m_thisTransform.position.x) > 0;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			target = other.transform;
			int sproutlingsAcquired = target.gameObject.GetComponent<PlayerScore>().Sproutlings;
			xOffset = sproutlingsAcquired * 0.7f;
			target.gameObject.GetComponent<PlayerScore>().Sproutlings += 1;
			m_boxCollider.isTrigger = true;
		}
	}
}
