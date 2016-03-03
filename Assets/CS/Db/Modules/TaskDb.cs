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
	    List<TaskData> taskListData = null;

		/// <summary>
		/// 判断是否需要初始化缓存数据
		/// </summary>
		void validTaskListData() {
			if (taskListData == null) {
				taskListData = new List<TaskData>();
				db = OpenDb();
				//正序查询处于战斗队伍中的角色
				SqliteDataReader sqReader = db.ExecuteQuery("select * from TasksTable where BelongToRoleId = '" + currentRoleId + "' and State >= 0 order by State");
				TaskData taskData;
				while (sqReader.Read()) {
					taskData = JsonManager.GetInstance().GetMapping<TaskData>("Tasks", sqReader.GetString(sqReader.GetOrdinal("TaskId")));
					taskData.State = (TaskStateType)sqReader.GetInt32(sqReader.GetOrdinal("State"));
					taskData.SetCurrentDialogIndex(sqReader.GetInt32(sqReader.GetOrdinal("CurrentDialogIndex")));
					taskData.ProgressData = JsonManager.GetInstance().DeserializeObject<JArray>(sqReader.GetString(sqReader.GetOrdinal("ProgressData")));
					taskData.MakeJsonToModel();
					taskListData.Add(taskData);
				}
				db.CloseSqlConnection();
			}
		}

		/// <summary>
		/// 添加一条新任务到可接取任务数据列表
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		public void AddNewTask(string taskId) {
			db = OpenDb();
			SqliteDataReader sqReader = db.ExecuteQuery("select TaskId from TasksTable where TaskId = '" + taskId + "'");
			if (!sqReader.HasRows) {
				TaskData taskData = JsonManager.GetInstance().GetMapping<TaskData>("Tasks", taskId);
				if (taskData.Id == taskId) {
					//添加任务数据时把任务步骤存档字段也进行初始化
					JArray progressDataList = new JArray();
					for(int i = 0; i < taskData.Dialogs.Count; i++) {
						progressDataList.Add((short)(i == 0 ? TaskDialogStatusType.Initial : TaskDialogStatusType.HoldOn));
					}
					db.ExecuteQuery("insert into TasksTable (TaskId, ProgressData, CurrentDialogIndex, State, BelongToRoleId) values('" + taskId + "', '" + progressDataList.ToString() + "', 0, 0, '" + currentRoleId + "');");
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 检测任务对话状态(任务对话的进度在这里来更新, 每次验证任务对话类型，然后判断是否可以完成，如果可以完成则CurrentDialogIndex+1)
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		/// <param name="auto">If set to <c>true</c> auto.</param>
		/// <param name="selectedNo">If set to <c>true</c> selected no.</param>
		public void CheckTaskDialog(string taskId, bool auto = false, bool selectedNo = false) {
			db = OpenDb();
			TaskData data = taskListData.Find(item => item.Id == taskId);
			if (data != null) {
				if (data.CheckCompleted()) {
					db.CloseSqlConnection();
					return;
				}
				bool canModify = false;
				switch (data.GetCurrentDialog().Type) {
				case TaskDialogType.Choice:
					if (!auto) {
						data.NextDialogIndex(selectedNo);
						canModify = true;
					}
					break;
				case TaskDialogType.ConvoyNpc:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.FightWined:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.JustTalk:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.RecruitedThePartner:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.SendItem:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.UsedTheBook:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.UsedTheSkillOneTime:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.UsedTheWeapon:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				case TaskDialogType.WeaponPowerPlusSuccessed:
					data.NextDialogIndex(selectedNo);
					canModify = true;
					break;
				default:
					break;
				}
				if (data.CheckCompleted()) {
					data.State = TaskStateType.Completed;
					data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
				}
				if (canModify) {
					//update data
					db.ExecuteQuery("update TasksTable set ProgressData = '" + data.ProgressData.ToString() + 
					                "', CurrentDialogIndex = " + data.CurrentDialogIndex + 
					                ", State = " + (int)data.State + 
					                " where TaskId ='" + taskId + "' and BelongToRoleId = '" + currentRoleId + "'");
					int index = taskListData.FindIndex(item => item.Id == taskId);
					//update cache
					if (taskListData.Count > index) {
						taskListData[index] = data;
					}
					Debug.LogWarning(data.GetCurrentDialog().Type + "," + data.CurrentDialogIndex + "," + auto + "," + data.GetCurrentDialogStatus());
					Messenger.Broadcast<TaskData>(NotifyTypes.CheckTaskDialogEcho, data);
				}
			}
			db.CloseSqlConnection();
		}

		/// <summary>
		/// 请求任务界面数据
		/// </summary>
		public void GetTaskListPanelData() {
			validTaskListData();
			Debug.LogWarning(taskListData.Count);
		}

		/// <summary>
		/// 请求特定场景下可以接取的任务列表
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetTaskListDataInCityScene(string cityId) {
			validTaskListData();
			List<TaskData> taskData = taskListData.FindAll(item => item.BelongToSceneId == cityId);
			Messenger.Broadcast<List<TaskData>>(NotifyTypes.GetTaskListDataInCitySceneEcho, taskData);
		}

		/// <summary>
		/// 获取当前任务列表数据
		/// </summary>
		public void GetTaskListData() {
			validTaskListData();
			Messenger.Broadcast<List<TaskData>>(NotifyTypes.GetTaskListDataEcho, taskListData);
		}

		public void GetTaskDetailInfoData(string taskId) {
			validTaskListData();
			TaskData data = taskListData.Find(item => item.Id == taskId);
			if (data != null) {
				Messenger.Broadcast<TaskData>(NotifyTypes.ShowTaskDetailInfoPanel, data);
			}
		}
	}
}
