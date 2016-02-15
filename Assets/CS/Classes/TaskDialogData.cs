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
		/// 对话内容
		/// </summary>
		public string TalkMsg;
		/// <summary>
		/// 字符串类型值
		/// </summary>
		public string StringValue;
		/// <summary>
		/// 整型值
		/// </summary>
		public int IntValue;
		/// <summary>
		/// 布尔型值
		/// </summary>
		public bool BoolValue;
		/// <summary>
		/// 布尔是后置任务Id(正常的流程用)
		/// 当抉择型的对话步骤出现时,任何一个选项都会立即接取一个新任务
		/// </summary>
		public string BackYesTaskDataId;
		/// <summary>
		/// 布尔非后置任务Id(只有抉择型任务对话为非时才会有用)
		/// 当抉择型的对话步骤出现时,任何一个选项都会立即接取一个新任务
		/// </summary>
		public string BackNoTaskDataId;
		/// <summary>
		/// 如果是纯谈话类型的对话则需要显示人物头像
		/// </summary>
		public string IconId;
		/// <summary>
		/// 布尔是显示信息(正常的流程用)
		/// </summary>
		public string YesMsg;
		/// <summary>
		/// 布尔非显示信息(只有抉择型任务对话为非时才会有用)
		/// </summary>
		public string NoMsg;
		/// <summary>
		/// 标记对话是否已经完成
		/// </summary>
		public bool Completed;

		public TaskDialogData() {
			BackYesTaskDataId = "";
			BackNoTaskDataId = "";
		}

		/// <summary>
		/// 判断是否达成条件,判断结果为真时通知上一级任务对象将任务Id和任务对话索引存入数据库,以便持久化
		/// </summary>
		public bool Check() {
			return false;
		}
	}
}
