using UnityEngine;
using System.Collections;
using Game;

public class BindUIToObjectCore : MonoBehaviour {
	public Transform BindedTarget;
	public float TargetSkiHeight = 0.2f;
	float targetHeight;
	RectTransform uiTransform;

	Camera mainCamera;

	/// <summary>
	/// 初始化
	/// </summary>
	protected virtual void Init() {
		transform.SetParent(UIModel.FontCanvas.transform);
		uiTransform = GetComponent<RectTransform>();
		mainCamera = Camera.main;

		if (BindedTarget == null) {
			Debug.LogWarning("lost binded target!");
			enabled = false;
			return;
		}
		name = "bind_to_" + BindedTarget.name;
		BoxCollider collider = BindedTarget.GetComponent<BoxCollider>();
		targetHeight = collider.size.y + TargetSkiHeight;
	}

	void Start() {
		Init();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (BindedTarget != null) {
			Vector3 targetPosition = BindedTarget.transform.position;
			Vector3 worldPosition = new Vector3 (targetPosition.x , targetPosition.y + targetHeight, targetPosition.z);
			Vector3 position = mainCamera.WorldToScreenPoint(worldPosition);
			if (position.z > 0.0f) {
				uiTransform.anchoredPosition = new Vector2(position.x, position.y);
			}
		}
		else {
			Destroy(gameObject);
		}
	}
}
