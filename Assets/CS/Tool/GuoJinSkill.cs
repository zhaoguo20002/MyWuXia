using UnityEngine;
using System.Collections;

public class GuoJinSkill : MonoBehaviour {
	public Camera MainCamera;
	Camera camera;
	// Use this for initialization
	void Start () {
		if (MainCamera == null) {
			if (Camera.main == null) {
				enabled = false;
				return;
			}
			MainCamera = Camera.main;
		}
		GameObject cameraObj = new GameObject();
		cameraObj.name = "GuoJinSkillCamera";
		camera = cameraObj.AddComponent<Camera>();
		camera.cullingMask = 1 << LayerMask.NameToLayer("Role");
		camera.clearFlags = CameraClearFlags.Depth;
		camera.depth = 10;
		camera.fieldOfView = MainCamera.fieldOfView;
		camera.nearClipPlane = MainCamera.nearClipPlane;
		camera.farClipPlane = MainCamera.farClipPlane;
		cameraObj.transform.SetParent(MainCamera.transform);
		cameraObj.transform.localPosition = Vector3.zero;
		cameraObj.transform.localEulerAngles = Vector3.zero;
	}

	void OnDestroy() {
		if (camera != null) {
			Destroy(camera.gameObject);
		}
	}
}
