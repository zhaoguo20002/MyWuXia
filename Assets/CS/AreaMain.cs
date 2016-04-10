using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class AreaMain : MonoBehaviour {
	/// <summary>
	/// 静态区域大地图拥有的缓存事件
	/// </summary>
	public static Dictionary<string, EventData> StaticAreaEventsMapping = null;
	/// <summary>
	/// 动态区域大地图拥有的缓存事件
	/// </summary>
	public static Dictionary<string, EventData> ActiveAreaEventsMapping = null;

	public string BgmId = "";
	string areaName;
	int startX;
	int startY;
	tk2dTileMapDemoFollowCam follow;
	tk2dTileMap map;
	public tk2dTileMap Map {
		get {
			return map;
		}
	}
	AreaTarget areaTarget;

	// Use this for initialization
	void Awake () {
		areaName = Application.loadedLevelName;
		Debug.LogWarning("当前场景:" + Application.loadedLevelName);
		//每次切换新的区域大地图都需要把静态事件和动态事件合并
		if (StaticAreaEventsMapping == null) {
			StaticAreaEventsMapping = new System.Collections.Generic.Dictionary<string, EventData>();
			JObject allEvents = JsonManager.GetInstance().GetJson("AreaEventDatas");
			foreach(var obj in allEvents) {
				if (!StaticAreaEventsMapping.ContainsKey(obj.Value["Id"].ToString())) {
					StaticAreaEventsMapping.Add(obj.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<EventData>(obj.Value.ToString()));
				}
			}
		}
		//加载动态事件
		if (ActiveAreaEventsMapping == null) {
			ActiveAreaEventsMapping = new Dictionary<string, EventData>();
			//待做...
		}

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
