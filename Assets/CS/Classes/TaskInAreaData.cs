using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class TaskInAreaData {
		/// <summary>
		/// 大地图场景名
		/// </summary>
		public string AreaName;
		/// <summary>
		/// 该区域所属的任务Id列表
		/// </summary>
		public List<string> ResourceTaskDataIds;
		/// <summary>
		/// 在该带地图下的所有的任务数据
		/// </summary>
		public List<TaskData> TasksInArea;

		public TaskInAreaData() {
			ResourceTaskDataIds = new List<string>();
			TasksInArea = new List<TaskData>();
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			TasksInArea.Clear();
			for (int i= 0; i< ResourceTaskDataIds.Count; i++) {
				TasksInArea.Add(JsonManager.GetInstance().GetMapping<TaskData>("Tasks", ResourceTaskDataIds[i]));
			}
		}
	}
}
