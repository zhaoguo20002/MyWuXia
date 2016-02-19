using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 任务相关数据模块
	/// </summary>
	public partial class DbManager {
		/// <summary>
		/// 任务缓存数据
		/// </summary>
		static Dictionary<string, TaskData> tasksMapping = null;
		public void AddNewTask(string taskId) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select TaskId from TasksTable where TaskId = '" + taskId + "'");
			if (!sqReader.HasRows) {
				db.ExecuteQuery("insert into TasksTable (TaskId, CurrentDialogIndex, State, BelongToRoleId) values('" + taskId + "', 0, 1, '" + currentRoleId + "');");
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求任务界面数据
		/// </summary>
		public void GetTaskListPanelData() {
			if (tasksMapping == null) {
				tasksMapping = new Dictionary<string, TaskData>();
				db = OpenDb();
				//正序查询处于战斗队伍中的角色
				SqliteDataReader sqReader = db.ExecuteQuery("select * from TasksTable where BelongToRoleId = '" + currentRoleId + "' and State > 0 order by Id");
				TaskData taskData;
				while (sqReader.Read()) {
					taskData = JsonManager.GetInstance().GetMapping<TaskData>("Tasks", sqReader.GetString(sqReader.GetOrdinal("TaskId")));
					taskData.State = (TaskStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
					taskData.SetCurrentDialogIndex(sqReader.GetInt32(sqReader.GetOrdinal("CurrentDialogIndex")));
					tasksMapping.Add(taskData.Id, taskData);
				}
				db.CloseSqlConnection();
			}
			Debug.LogWarning(tasksMapping.Count);
		}
	}
}
