using UnityEngine;
using System.Collections;
using Game;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using BeautifyEffect;

[RequireComponent(typeof(Beautify),typeof(Camera))]
public class AreaMain : MonoBehaviour {
	/// <summary>
	/// 静态区域大地图拥有的缓存事件
	/// </summary>
	public static Dictionary<string, EventData> StaticAreaEventsMapping = null;
	/// <summary>
	/// 动态区域大地图拥有的缓存事件
	/// </summary>
	public static Dictionary<string, EventData> ActiveAreaEventsMapping = null;
	/// <summary>
	/// 临时禁用事件集合
	/// </summary>
	public static Dictionary<string, EventData> DisableEventIdMapping = null;

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
	EventData handleDisableEvent = null;

	Camera myCamera;
	Beautify myBeatuify;

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

		//初始化临时禁用事件集合
		if (DisableEventIdMapping == null) {
			DisableEventIdMapping = new Dictionary<string, EventData>();
		}

		follow = GetComponent<tk2dTileMapDemoFollowCam>();
		follow.followSpeed = 30;
		map = GameObject.Find("TileMap").GetComponent<tk2dTileMap>();
		areaTarget = Statics.GetPrefabClone("Prefabs/AreaTarget").GetComponent<AreaTarget>();
		areaTarget.Map = map;
		follow.target = areaTarget.transform;
		myCamera = GetComponent<Camera>();
		myCamera.cullingMask = 1 << LayerMask.NameToLayer("Role") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Target");
		myBeatuify = GetComponent<Beautify>();
		if (myBeatuify == null) {
			myBeatuify = gameObject.AddComponent<Beautify>();
		}
		myBeatuify.quality = BEAUTIFY_QUALITY.Mobile;
		myBeatuify.preset = BEAUTIFY_PRESET.Medium;
		myBeatuify.sharpenRelaxation = 0;
		myBeatuify.brightness = 1f;
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
			Map.Layers[1].ClearTile(eventData.X, eventData.Y);
		}
		Map.Build(tk2dTileMap.BuildFlags.Default);
		Statics.ChangeLayers(GameObject.Find("TileMap Render Data").transform, "Ground");
		ActiveAreaEventsMapping.Clear();
	}

	/// <summary>
	/// 缓存将要禁用的事件
	/// </summary>
	/// <param name="ev">Ev.</param>
	public void HandleDisableEvent(EventData ev) {
		handleDisableEvent = ev;
	}

	/// <summary>
	/// 判断战斗胜负决定是否禁用战斗对应的事件
	/// </summary>
	/// <param name="win">If set to <c>true</c> window.</param>
	public void ReleaseDisableEvent(bool win) {
		if (handleDisableEvent == null) {
			return;
		}
		if (win) {
			PushDisableEvent(handleDisableEvent.Id, handleDisableEvent);
		}
		handleDisableEvent = null;
	}

	/// <summary>
	/// 添加临时禁用事件
	/// </summary>
	/// <param name="eventId">Event identifier.</param>
	/// <param name="disableEvent">Disable event.</param>
	public void PushDisableEvent(string eventId, EventData disableEvent) {
		if (!DisableEventIdMapping.ContainsKey(eventId)) {
			DisableEventIdMapping.Add(eventId, disableEvent);
			Map.Layers[2].SetTile(disableEvent.X, disableEvent.Y, 15);
			Map.Build(tk2dTileMap.BuildFlags.Default);
			Statics.ChangeLayers(GameObject.Find("TileMap Render Data").transform, "Ground");
		}
	}

	/// <summary>
	/// 清空临时禁用事件
	/// </summary>
	public void ClearDisableEventIdMapping() {
		foreach(EventData eventData in DisableEventIdMapping.Values) {
			Map.Layers[2].ClearTile(eventData.X, eventData.Y);
		}
		Map.Build(tk2dTileMap.BuildFlags.Default);
		Statics.ChangeLayers(GameObject.Find("TileMap Render Data").transform, "Ground");
		DisableEventIdMapping.Clear();
	}
}
