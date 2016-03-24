using UnityEngine;
using System.Collections;
using Game;

public class AreaMain : MonoBehaviour {
	public string BgmId = "";
	string areaName;
	int startX;
	int startY;
	tk2dTileMapDemoFollowCam follow;
	tk2dTileMap map;
	AreaTarget areaTarget;

	// Use this for initialization
	void Awake () {
		areaName = Application.loadedLevelName;
		Debug.LogWarning("当前场景:" + Application.loadedLevelName);
		follow = GetComponent<tk2dTileMapDemoFollowCam>();
		follow.followSpeed = 30;
		map = GameObject.Find("TileMap").GetComponent<tk2dTileMap>();
		areaTarget = Statics.GetPrefabClone("Prefabs/AreaTarget").GetComponent<AreaTarget>();
		areaTarget.Map = map;
		follow.target = areaTarget.transform;

	}

	void Start() {
		PlayBgm();
		Messenger.Broadcast<AreaTarget, AreaMain>(NotifyTypes.AreaInit, areaTarget, this);
	}

	/// <summary>
	/// 设置坐标
	/// </summary>
	/// <param name="pos">Position.</param>
	public void SetPosition(Vector2 pos, bool doEvent = false) {
		//出生点需要更传送机制关联
		startX = (int)pos.x;
		startY = (int)pos.y;
		areaTarget.SetPosition(startX, startY, doEvent);
		transform.position = new Vector3(areaTarget.transform.position.x, areaTarget.transform.position.y, transform.position.z);
	}

	public void PlayBgm() {
		if (!string.IsNullOrEmpty(BgmId)) {
			SoundManager.GetInstance().PlayBGM(BgmId);
		}
	}
}
