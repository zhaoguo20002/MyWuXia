﻿using UnityEngine;
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
	string sceneId;
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
		sceneId = Application.loadedLevelName;
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
	/// <summary>
	/// 更新动态事件数据
	/// </summary>
	/// <param name="events">Events.</param>
	public void UpdateActiveAreaEventsData(List<EventData> events) {
		ClearActiveAreaEvents();
		for (int i = 0; i < events.Count; i++) {
			if (!ActiveAreaEventsMapping.ContainsKey(events[i].Id)) {
				ActiveAreaEventsMapping.Add(events[i].Id, events[i]);
			}
		}
	}
	/// <summary>
	/// 刷新动态事件
	/// </summary>
	public void RefreshActiveAreaEventsView() {
		int eventIconIndex = 0;
		foreach(EventData eventData in ActiveAreaEventsMapping.Values) {
			switch(eventData.Type) {
			case SceneEventType.Battle:
				eventIconIndex = 1;
				break;
			case SceneEventType.Task:
				eventIconIndex = 26;
				break;
			default:
				eventIconIndex = 18;
				break;
			}
			Map.Layers[1].SetTile(eventData.X, eventData.Y, eventIconIndex);
		}
		Map.Build(tk2dTileMap.BuildFlags.Default);
		Statics.ChangeLayers(GameObject.Find("TileMap Render Data").transform, "Ground");
	}
	/// <summary>
	/// 清空原有的动态事件
	/// </summary>
	public void ClearActiveAreaEvents() {
		foreach(EventData eventData in ActiveAreaEventsMapping.Values) {
			Map.Layers[1].SetTile(eventData.X, eventData.Y, 0);
		}
		Map.Build(tk2dTileMap.BuildFlags.Default);
		ActiveAreaEventsMapping.Clear();
	}
}
