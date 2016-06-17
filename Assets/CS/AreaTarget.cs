using UnityEngine;
using System.Collections;
using Game;
using DG;
using DG.Tweening;
using System.Collections.Generic;

public class AreaTarget : MonoBehaviour {
	/// <summary>
	/// 向上移动
	/// </summary>
	/// <value>The U.</value>
	public static string Up {
		get {
			return "up";
		}
	}
	/// <summary>
	/// 向下移动
	/// </summary>
	/// <value>Down.</value>
	public static string Down {
		get {
			return "down";
		}
	}
	/// <summary>
	/// 向左移动
	/// </summary>
	/// <value>The left.</value>
	public static string Left {
		get {
			return "left";
		}
	}
	/// <summary>
	/// 向右移动
	/// </summary>
	/// <value>The right.</value>
	public static string Right {
		get {
			return "right";
		}
	}
	public tk2dTileMap Map;
	int _x = 0;
	int _y = 0;
	// Use this for initialization
	void Start () {
//		SetPosition(1, 1);
//		Map.Layers[1].SetTile(0, 0, 10);
//		Map.Build(tk2dTileMap.BuildFlags.Default);
//		Debug.LogWarning("动态生成地砖");
//		tk2dRuntime.TileMap.TileInfo tile;
//		for (int i = 0; i < Map.width; i++) {
//			for (int j = 0; j <  Map.height; j++) {
//				tile = Map.GetTileInfoForTileId(Map.GetTile(i, j, 1));
//				if (tile != null) {
//					Debug.LogWarning(i+ "," + j + "," + tile.stringVal);
//				}
//			}
//		}
	}

	/// <summary>
	/// 获取当前地砖信息
	/// </summary>
	/// <returns>The current tile info.</returns>
	/// <param name="layer">Layer.</param>
	public tk2dRuntime.TileMap.TileInfo GetCurrentTileInfo(int layer) {
		return GetTileInfo(_x, _y, layer);
	}

	/// <summary>
	/// 获取地砖信息
	/// </summary>
	/// <returns>The tile info.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="layer">Layer.</param>
	public tk2dRuntime.TileMap.TileInfo GetTileInfo(int x, int y, int layer) {
		return Map.GetTileInfoForTileId(Map.GetTile(x, y, layer));
	}

	/// <summary>
	/// 根据地砖编号转换成坐标
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="doEvent">If set to <c>true</c> do event.</param>
	/// <param name="duringMove">If set to <c>true</c> during move.</param>
	public void SetPosition(int x, int y, bool doEvent = true, bool duringMove = false) {
		tk2dRuntime.TileMap.TileInfo groundTile = GetTileInfo(x, y, 0);
		//判断禁止通过的碰撞区域
		if (groundTile == null || groundTile.stringVal == "obstacle") {
			return;
		}
		if (doEvent) {
			//记录当前坐标
			Messenger.Broadcast<string, Vector2, System.Action<UserData>>(NotifyTypes.UpdateUserDataAreaPos, UserModel.CurrentUserData.CurrentAreaSceneName, new Vector2(x, y), null);
			Invoke("doEventDelay", duringMove ? 0.24f : 0);
		}
		_x = x;
		_y = y;
		Vector3 position = Map.GetTilePosition(_x, _y);
		float rx = Map.partitionSizeX * 0.005f;
		if (duringMove) {
			transform.DOMove(new Vector3(position.x + rx, position.y, position.z), 0.3f);
		}
		else {
			transform.position = new Vector3(position.x + rx, position.y, position.z);
		}
//		Map.ColorChannel.SetColor(_x, _y, new Color(1, 1, 1, 0));
//		Map.Build(tk2dTileMap.BuildFlags.Default);

	}

	void doEventDelay() {
		tk2dRuntime.TileMap.TileInfo eventTile = GetTileInfo(_x, _y, 1);
		if (eventTile != null) {
			//处理区域图上的事件
			if (eventTile.stringVal == "Event") {
				string id = Application.loadedLevelName + "_" + _x + "_" + _y;
				Messenger.Broadcast<string>(NotifyTypes.DealSceneEvent, id);
			}
		}
		else {
			//之前没有触发任何事件则在这里处理随机遇敌
			List<RateData> ratesData = Statics.GetMeetEnemyRates(UserModel.CurrentUserData.CurrentAreaSceneName);
			RateData rateData;
			for (int i = 0; i < ratesData.Count; i++) {
				rateData = ratesData[i];
				if (rateData.IsTrigger()) {
					Messenger.Broadcast<string>(NotifyTypes.CreateBattle, rateData.Id); //遇敌
					break;
				}
			}
		}
	}

	/// <summary>
	/// 让角色移动
	/// </summary>
	/// <param name="direction">Direction.</param>
	/// <param name="doEvent">If set to <c>true</c> do event.</param>
	/// <param name="duringMove">If set to <c>true</c> during move.</param>
	public Vector2 Move(string direction, bool doEvent = true, bool duringMove = false) {
		switch (direction) {
		case "up":
			SetPosition(_x, _y + 1, doEvent, duringMove);
			break;
		case "down":
			SetPosition(_x, _y - 1, doEvent, duringMove);
			break;
		case "left":
			SetPosition(_x - 1, _y, doEvent, duringMove);
			break;
		case "right":
			SetPosition(_x + 1, _y, doEvent, duringMove);
			break;
		default:
			break;
		}
		return new Vector2(_x, _y);
	}

	/// <summary>
	/// 获取下个移动方向对应的坐标
	/// </summary>
	/// <returns>The next move position.</returns>
	/// <param name="direction">Direction.</param>
	public Vector2 GetNextMovePosition(string direction) {
		switch (direction) {
		case "up":
			return new Vector2(_x, _y + 1);
		case "down":
			return new Vector2(_x, _y - 1);
		case "left":
			return new Vector2(_x - 1, _y);
		case "right":
			return new Vector2(_x + 1, _y);
		default:
			return new Vector2(_x, _y);
		}
	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}

//	void OnGUI() {
//		if (GUI.Button(new Rect(100, 50, 50, 50), "Up")) {
//			Move(Up);
//		}
//		if (GUI.Button(new Rect(100, 150, 50, 50), "Down")) {
//			Move(Down);
//		}
//		if (GUI.Button(new Rect(50, 100, 50, 50), "Left")) {
//			Move(Left);
//		}
//		if (GUI.Button(new Rect(150, 100, 50, 50), "Right")) {
//			Move(Right);
//		}
//	}
}
