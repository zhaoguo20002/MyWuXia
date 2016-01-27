using UnityEngine;
using System.Collections;

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
		SetPosition(1, 1);
		Map.Layers[1].SetTile(3, 3, 3);
		Map.Build(tk2dTileMap.BuildFlags.Default);
		tk2dRuntime.TileMap.TileInfo tile;
		for (int i = 0; i < Map.width; i++) {
			for (int j = 0; j <  Map.height; j++) {
				tile = Map.GetTileInfoForTileId(Map.GetTile(i, j, 1));
				if (tile != null) {
					Debug.LogWarning(i+ "," + j + "," + tile.stringVal);
				}
			}
		}
	}

	/// <summary>
	/// 获取地砖信息
	/// </summary>
	/// <returns>The tile info.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="layer">Layer.</param>
	tk2dRuntime.TileMap.TileInfo getTileInfo(int x, int y, int layer) {
		return Map.GetTileInfoForTileId(Map.GetTile(x, y, layer));
	} 

	/// <summary>
	/// 根据地砖编号转换成坐标
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void SetPosition(int x, int y) {
		tk2dRuntime.TileMap.TileInfo groundTile = getTileInfo(x, y, 0);
		//判断禁止通过的碰撞区域
		if (groundTile == null || groundTile.stringVal == "obstacle") {
			return;
		}
		tk2dRuntime.TileMap.TileInfo eventTile = getTileInfo(x, y, 1);
		if (eventTile != null) {
			Debug.LogWarning(eventTile.stringVal);
		}
		_x = x;
		_y = y;
		Vector3 position = Map.GetTilePosition(_x, _y);
		float rx = Map.partitionSizeX * 0.005f;
		transform.position = new Vector3(position.x + rx, position.y, position.z);
		Map.ColorChannel.SetColor(_x, _y, new Color(1, 1, 1, 0));
		Map.Build(tk2dTileMap.BuildFlags.Default);
	}

	public void Move(string direction) {
		switch (direction) {
		case "up":
			SetPosition(_x, _y + 1);
			break;
		case "down":
			SetPosition(_x, _y - 1);
			break;
		case "left":
			SetPosition(_x - 1, _y);
			break;
		case "right":
			SetPosition(_x + 1, _y);
			break;
		default:
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (GUI.Button(new Rect(100, 50, 50, 50), "Up")) {
			Move(Up);
		}
		if (GUI.Button(new Rect(100, 150, 50, 50), "Down")) {
			Move(Down);
		}
		if (GUI.Button(new Rect(50, 100, 50, 50), "Left")) {
			Move(Left);
		}
		if (GUI.Button(new Rect(150, 100, 50, 50), "Right")) {
			Move(Right);
		}
	}
}
