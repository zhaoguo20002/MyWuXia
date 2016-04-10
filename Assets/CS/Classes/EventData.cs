using UnityEngine;
using System.Collections;

namespace Game {
	public class EventData {
		/// <summary>
		/// 事件主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 事件描述名
		/// </summary>
		public string Name;
		/// <summary>
		/// 事件类型
		/// </summary>
		public SceneEventType Type;
		/// <summary>
		/// 事件Id
		/// </summary>
		public string EventId;
		/// <summary>
		/// 所属场景文件名
		/// </summary>
		public string SceneId;
		/// <summary>
		/// 事件开启条件
		/// </summary>
		public SceneEventOpenType OpenType;
		/// <summary>
		/// 开启条件的值(战斗id、物品id)
		/// </summary>
		public string OpenKey;

		public EventData() {
			EventId = "0";
			OpenType = SceneEventOpenType.None;
			OpenKey = "";
		}
	}
}
