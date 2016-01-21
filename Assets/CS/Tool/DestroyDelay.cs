using UnityEngine;
using System.Collections;

public class DestroyDelay : MonoBehaviour {
	
	public float Timeout = 1.0f;
	float date;
	// Use this for initialization
	void Start () {
		date = Time.fixedTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.fixedTime - date > Timeout) {
			Destroy(gameObject);
		}
	}
}
