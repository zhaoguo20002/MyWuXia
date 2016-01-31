using UnityEngine;
using System.Collections;
using Game;

public class AreaMain : MonoBehaviour {
	int startX;
	int startY;
	tk2dTileMapDemoFollowCam follow;
	tk2dTileMap map;
	AreaTarget areaTarget;

	// Use this for initialization
	void Start () {
		Debug.LogWarning("当前场景:" + Application.loadedLevelName);
		//出生点需要更传送机制关联
		startX = 2;
		startY = 2;
		follow = GetComponent<tk2dTileMapDemoFollowCam>();
		follow.followSpeed = 30;
		map = GameObject.Find("TileMap").GetComponent<tk2dTileMap>();
		areaTarget = Statics.GetPrefabClone("Prefabs/AreaTarget").GetComponent<AreaTarget>();
		areaTarget.Map = map;
		follow.target = areaTarget.transform;
		areaTarget.SetPosition(startX, startY);

		Messenger.Broadcast<AreaTarget>(NotifyTypes.AreaInit, areaTarget);
	}
}
