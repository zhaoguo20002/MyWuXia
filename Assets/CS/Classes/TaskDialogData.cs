using UnityEngine;
using System.Collections;

namespace Game {
	/// <summary>
	/// 任务对话数据
	/// 每一个任务步骤包括对话,给东西,战斗等都系要在这里集中管理,并且记录结果
	/// </summary>
	public class TaskDialogData {
		/// <summary>
		/// 任务对话类型
		/// </summary>
		public TaskDialogType Type;

		/// <summary>
		/// 字符串类型值
		/// </summary>
		public string StringValue;
		/// <summary>
		/// 整型值
		/// </summary>
		public int IntValue;
		/// <summary>
		/// 浮点数型值
		/// </summary>
		public float FloatValue;
		/// <summary>
		/// 整型值区间最小值
		/// </summary>
		public int MinIntValue;
		/// <summary>
		/// 整型值区间最大值
		/// </summary>
		public int MaxIntValue;

		/// <summary>
		/// 如果是纯谈话类型的对话则需要显示人物头像
		/// </summary>
		public string IconId;

		/// <summary>
		/// 显示信息
		/// </summary>
		public string Msg;

		public TaskDialogData() {
			
		}

		/// <summary>
		/// 判断是否达成条件,判断结果为真时通知上一级任务对象将任务Id和任务对话索引存入数据库,以便持久化
		/// </summary>
		public bool Check() {
			return false;
		}
	}
}
