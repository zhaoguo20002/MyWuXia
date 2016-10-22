using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class NpcData {
		/// <summary>
		/// Npc主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// Npc名字
		/// </summary>
		public string Name;
		/// <summary>
		/// npc类型
		/// </summary>
		public NpcType Type;
		/// <summary>
		/// 默认说的一句话
		/// 当NPC身上没任务时点击Npc头像弹出的气泡显示这句话
		/// 有任务或者已经做过任务的话直接弹出任务剧情对话界面
		/// </summary>
		public string DefaultDialogMsg;
		/// <summary>
		/// 头像Id
		/// </summary>
		public string IconId;
		/// <summary>
		/// 是否为动态Npc(不会默认出现在某一个场景,需要关联任务状态确定出现地)
		/// </summary>
		public bool IsActive;
		/// <summary>
		/// NPC身上的当前任务Id
		/// </summary>
		public string CurrentResourceTaskDataId;
		/// <summary>
		/// 当前任务数据
		/// </summary>
		public TaskData CurrentTask;
		/// <summary>
		/// 已经完成的任务对话列表
		/// </summary>
		public List<TaskDialogData> GotDialogs;
		/// <summary>
		/// 当前战斗id
		/// </summary>
		public string CurrentFightId;
        /// <summary>
        /// 现身所需要的特定任务id
        /// </summary>
        public string ShowAfterTaskId;

		public NpcData() {
			DefaultDialogMsg = "";
			IconId = "";
			IsActive = false;
			CurrentResourceTaskDataId = "";
			GotDialogs = new List<TaskDialogData>();
			CurrentFightId = "";
            ShowAfterTaskId = "";
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			CurrentTask = JsonManager.GetInstance().GetMapping<TaskData>("Tasks", CurrentResourceTaskDataId);
		}

		/// <summary>
		/// 设置Npc已经完成的任务对话[让某个Npc完成的所有任务对话能持久化保存起来,方便玩家回溯]
		/// </summary>
		/// <param name="dialogs">Dialogs.</param>
		public void SetGotDialogs(List<TaskDialogData> dialogs) {
			GotDialogs = dialogs;
		}
	}
}
