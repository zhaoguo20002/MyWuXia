using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using DG;
using DG.Tweening;

[RequireComponent(typeof(Vortex))]
public class CameraVortex : MonoBehaviour {
	Vortex vortexScript;
	bool isPlaying;
	float _frame = 30;
	System.Action _callback;
	float maxAngle = 3600;
	float currentAngle;
	float stepAngle;
	float stepASpeedAngle;
	bool endHalf;

	void Awake() {
		vortexScript = GetComponent<Vortex>();
		vortexScript.radius = Vector2.one;
		vortexScript.center = new Vector2(0.5f, 0.5f);
		vortexScript.angle = 0;
		isPlaying = false;
	}

	public void StartPlay(System.Action callback) {
		_callback = callback;
		currentAngle = 0;
		stepASpeedAngle = maxAngle / (_frame * 0.5f);
		stepAngle = 0;
		endHalf = false;
		isPlaying = true;
		vortexScript.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (isPlaying) {
			vortexScript.angle = currentAngle;
			if (!endHalf) {
				stepAngle = Mathf.Lerp(stepAngle, stepASpeedAngle, Time.deltaTime);
				currentAngle += stepAngle;
				if (currentAngle >= maxAngle - 50) {
					endHalf = true;
					stepAngle = 0;
				}
			}
			else {
				currentAngle = Mathf.Lerp(currentAngle, 0, Time.deltaTime * 10);
				if (currentAngle <= 0.1f) {
					isPlaying = false;
					vortexScript.enabled = false;
					if (_callback != null) {
						_callback();	
					}
				}
			}
		}
	}
}
