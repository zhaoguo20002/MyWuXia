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
		/// 特定任务的后续任务集合字典
		/// </summary>
		Dictionary<string, JArray> childrenTasksMapping = null;

		/// <summary>
		/// 初始化任务相关数据
		/// </summary>
		void initTasks() {
			validTaskListData();
			if (childrenTasksMapping == null) {
				childrenTasksMapping = new Dictionary<string, JArray>();
				JObject obj = JsonManager.GetInstance().GetJson("Tasks");
				TaskData taskData;
				foreach(var item in obj) {
					if (item.Key != "0") {
						taskData = JsonManager.GetInstance().DeserializeObject<TaskData>(item.Value.ToString());
						if (!childrenTasksMapping.ContainsKey(taskData.FrontTaskDataId)) {
							childrenTasksMapping[taskData.FrontTaskDataId] = new JArray();
						}
						childrenTasksMapping[taskData.FrontTaskDataId].Add(taskData.Id);
					}
				}
			}
			//每次游戏启动时初始化一次起始任务
			addChildrenTasks("0");
		}

		/// <summary>
		/// 开启前置任务id对应的后续任务
		/// </summary>
		/// <param name="frontTaskDataId">Front task data identifier.</param>
		void addChildrenTasks(string frontTaskDataId) {
			if (childrenTasksMapping.ContainsKey(frontTaskDataId)) {
				JArray childrenTasks = childrenTasksMapping["0"];
				for (int i = 0; i < childrenTasks.Count; i++) {
					AddNewTask(childrenTasks[i].ToString());
				}
			}
			//检测任务状态
			checkAddedTasksStatus();
		}

		/// <summary>
		/// 适时判断当前已开启还没接取的任务是否可以接取
		/// </summary>
		void checkAddedTasksStatus() {
			validTaskListData();
			//查询处条件还处于不可接取的所有任务
			List<TaskData> addedTasks = taskListData.FindAll(item => item.State == TaskStateType.CanNotAccept);
			TaskData task;
			db = OpenDb();
			bool canAccept;
			for (int i = 0; i < addedTasks.Count; i++) {
				task = addedTasks[i];
				canAccept = false;
				switch (task.Type) {
				case TaskType.Gender:

					break;
				case TaskType.ItemInHand:

					break;
				case TaskType.MoralRange:

					break;
				case TaskType.Occupation:

					break;
				case TaskType.TheHour:

					break;
				case TaskType.None:
				default:
					canAccept = true;
					break;

				}
				if (canAccept) {
					//讲符合接取条件的任务状态改变为可以接取任务
					db.ExecuteQuery("update TasksTable set State = " + (int)TaskStateType.CanAccept + 
						" where TaskId ='" + task.Id + "' and BelongToRoleId = '" + currentRoleId + "'");
					task.State = TaskStateType.CanAccept;
				}
			}
			db.CloseSqlConnection();
		}

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
		/// 获取一个任务的缓存数据
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="taskId">Task identifier.</param>
		TaskData getTask(string taskId) {
			if (taskListData != null) {
				return taskListData.Find(item => item.Id == taskId);
			} else {
				return null;
			}
		}

		/// <summary>
		/// 添加一条新任务到可接取任务数据列表
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		public void AddNewTask(string taskId) {
			validTaskListData();
			if (getTask(taskId) == null) {
				db = OpenDb();
				TaskData taskData = JsonManager.GetInstance().GetMapping<TaskData>("Tasks", taskId);
				if (taskData.Id == taskId) {
					//添加任务数据时把任务步骤存档字段也进行初始化
					JArray progressDataList = new JArray();
					for(int i = 0; i < taskData.Dialogs.Count; i++) {
						progressDataList.Add((short)(i == 0 ? TaskDialogStatusType.Initial : TaskDialogStatusType.HoldOn));
					}
					db.ExecuteQuery("insert into TasksTable (TaskId, ProgressData, CurrentDialogIndex, State, BelongToRoleId) values('" + taskId + "', '" + progressDataList.ToString() + "', 0, 0, '" + currentRoleId + "')");
					//顺手把数据写入缓存
					taskData.State = TaskStateType.CanNotAccept;
					taskData.SetCurrentDialogIndex(0);
					taskData.ProgressData = progressDataList;
					taskData.MakeJsonToModel();
					taskListData.Add(taskData);
				}
				db.CloseSqlConnection();
			}
		}

		/// <summary>
		/// 检测任务对话状态(任务对话的进度在这里来更新, 每次验证任务对话类型，然后判断是否可以完成，如果可以完成则CurrentDialogIndex+1)
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		/// <param name="auto">If set to <c>true</c> auto.</param>
		/// <param name="selectedNo">If set to <c>true</c> selected no.</param>
		public void CheckTaskDialog(string taskId, bool auto = false, bool selectedNo = false) {
			db = OpenDb();
			TaskData data = getTask(taskId);
			string triggerNewBackTaskDataId = "";
			if (data != null) {
				if (data.CheckCompleted()) {
					db.CloseSqlConnection();
					return;
				}
				bool canModify = false;
				switch (data.GetCurrentDialog().Type) {
				case TaskDialogType.Choice:
					if (!auto) {
						triggerNewBackTaskDataId = selectedNo ? data.GetCurrentDialog().BackNoTaskDataId : data.GetCurrentDialog().BackYesTaskDataId;
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
				} else {
					data.State = TaskStateType.Accepted;
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
					Messenger.Broadcast<TaskData>(NotifyTypes.CheckTaskDialogEcho, data);
				}
			}
			db.CloseSqlConnection();
			//触发新任务
			if (triggerNewBackTaskDataId != "") {
				AddNewTask(triggerNewBackTaskDataId);
				//检测任务状态
				checkAddedTasksStatus();
			}
			if (data.CheckCompleted()) {
				//添加任务奖励物品
				PushItemToBag(data.Rewards);
				Debug.LogWarning("任务奖励");
				//任务完成后出发后续任务
				addChildrenTasks(data.Id);
			}
		}

		/// <summary>
		/// 请求任务界面数据
		/// </summary>
		public void GetTaskListPanelData() {
			validTaskListData();
		}

		/// <summary>
		/// 请求特定场景下可以接取的任务列表
		/// </summary>
		/// <param name="cityId">City identifier.</param>
		public void GetTaskListDataInCityScene(string cityId) {
			validTaskListData();
			List<TaskData> taskData = taskListData.FindAll(item => item.BelongToSceneId == cityId && item.State != TaskStateType.CanNotAccept && item.State != TaskStateType.Completed);
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
			TaskData data = getTask(taskId);
			if (data != null && (data.State != TaskStateType.CanNotAccept && data.State != TaskStateType.Completed)) {
				Messenger.Broadcast<TaskData>(NotifyTypes.ShowTaskDetailInfoPanel, data);
			}
		}
	}
}
