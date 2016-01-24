using UnityEngine;
using System.Collections;

namespace Game {
	public class MsgQueueData {
		/// <summary>
		/// 主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 消息名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 消息文本
		/// </summary>
		public string Msg;
		/// <summary>
		/// 文本颜色
		/// </summary>
		public Color Color;
		/// <summary>
		/// 显示此条消息前是否清空之前的消息
		/// </summary>
		public bool WillClearMsg;

		public MsgQueueData(string id, string name, string msg, Color color, bool willClearMsg) {
			Id = id;
			Name = name;
			Msg = msg;
			Color = color;
			WillClearMsg = willClearMsg;
		}
	}
}
