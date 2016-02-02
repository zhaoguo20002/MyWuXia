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
		follow = GetComponent<tk2dTileMapDemoFollowCam>();
		follow.followSpeed = 30;
		map = GameObject.Find("TileMap").GetComponent<tk2dTileMap>();
		areaTarget = Statics.GetPrefabClone("Prefabs/AreaTarget").GetComponent<AreaTarget>();
		areaTarget.Map = map;
		follow.target = areaTarget.transform;
		Messenger.Broadcast<AreaTarget, AreaMain>(NotifyTypes.AreaInit, areaTarget, this);
	}

	/// <summary>
	/// 设置坐标
	/// </summary>
	/// <param name="pos">Position.</param>
	public void SetPosition(Vector2 pos) {
		MoveTo(pos);
		transform.position = new Vector3(areaTarget.transform.position.x, areaTarget.transform.position.y, transform.position.z);
	}

	/// <summary>
	/// 移动
	/// </summary>
	/// <param name="pos">Position.</param>
	public void MoveTo(Vector2 pos) {
		//出生点需要更传送机制关联
		startX = (int)pos.x;
		startY = (int)pos.y;
		areaTarget.SetPosition(startX, startY);
	}
}
