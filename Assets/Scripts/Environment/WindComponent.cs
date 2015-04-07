using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindComponent : MonoBehaviour 
{
	public bool windOn = true;
	public Vector2 Force = Vector2.zero;
	public float windTTL;
	
	// Internal list that tracks objects that enter this object's "zone"
	private List<Collider2D> objects = new List<Collider2D>();
	private ParticleSystem[] particleSystems;

	void Awake() {
		particleSystems = GetComponentsInChildren<ParticleSystem> ();
		if (!windOn) {
			stopParticles();
		}
	}

	void Start() {
		Destroy(this.gameObject, windTTL);
	}
	// This function is called every fixed framerate frame
	void FixedUpdate()
	{
		// For every object being tracked
		if (windOn) {
			for(int i = 0; i < objects.Count; i++)
			{
				// Get the rigid body for the object.
				Rigidbody2D body = objects[i].attachedRigidbody;

				// if object is wall, lower mass
				if (objects[i].tag == "Obstacle") {
					body.AddForce(Force*40000f);
				} else {
					// Apply the force
					body.AddForce(Force);
				}
				
				
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		objects.Add(other);
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		objects.Remove(other);
	}

	public void activateWind() {
		windOn = true;
		startParticles ();
	}

	void startParticles() {
		for (int i=0; i < particleSystems.Length; i++) {
			particleSystems[i].Play();
		}
	}

	void stopParticles() {
		for (int i=0; i < particleSystems.Length; i++) {
			particleSystems[i].Stop ();
		}
	}
}
