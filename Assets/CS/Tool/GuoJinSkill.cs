using UnityEngine;
using System.Collections;
using Game;

public class GuoJinSkill : MonoBehaviour {
	public Camera MainCamera;
	Camera camera;
	Animator ani;
	// Use this for initialization
	void Start () {
		if (MainCamera == null) {
			if (Camera.main == null) {
				enabled = false;
				return;
			}
			MainCamera = Camera.main;
		}
		ani = GetComponent<Animator>();
//		ani.speed = 1.5f;
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
		Messenger.Broadcast<bool>(NotifyTypes.DisplayCameraDepthOfField, true);
		SoundManager.GetInstance().PushSound("qg0001");
	}

	void OnDestroy() {
		if (camera != null) {
			Messenger.Broadcast<bool>(NotifyTypes.DisplayCameraDepthOfField, false);
			Destroy(camera.gameObject);
		}
	}
}
