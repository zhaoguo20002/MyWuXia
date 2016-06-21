using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

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
		Dictionary<string, List<string>> childrenTasksMapping = null;

		/// <summary>
		/// 初始化任务相关数据
		/// </summary>
		void initTasks() {
			if (childrenTasksMapping == null) {
				childrenTasksMapping = new Dictionary<string, List<string>>();
				JObject obj = JsonManager.GetInstance().GetJson("Tasks");
				TaskData taskData;
				foreach(var item in obj) {
					if (item.Key != "0") {
						taskData = JsonManager.GetInstance().DeserializeObject<TaskData>(item.Value.ToString());
						//初始化后置任务
						if (!childrenTasksMapping.ContainsKey(taskData.Id)) {
							childrenTasksMapping[taskData.Id] = new List<string>();
						}
						if (!childrenTasksMapping.ContainsKey(taskData.FrontTaskDataId)) {
							childrenTasksMapping[taskData.FrontTaskDataId] = new List<string>();
						}
						//处理后置任务
						if (taskData.BackTaskDataId != "0" && 
							!string.IsNullOrEmpty(taskData.BackTaskDataId) && 
							childrenTasksMapping[taskData.Id].FindIndex(id => id == taskData.BackTaskDataId) < 0) {
							childrenTasksMapping[taskData.Id].Add(taskData.BackTaskDataId);
						}
						//处理前置任务
						if (childrenTasksMapping[taskData.FrontTaskDataId].FindIndex(id => id == taskData.Id) < 0) {
							childrenTasksMapping[taskData.FrontTaskDataId].Add(taskData.Id);
						}
					}
				}
			}
		}

		/// <summary>
		/// 初试化任务
		/// </summary>
		public void ResetTasks() {
			taskListData = null;
			validTaskListData();
			//每次游戏启动时初始化一次起始任务
			addChildrenTasks("0");
		}

		/// <summary>
		/// 开启前置任务id对应的后续任务
		/// </summary>
		/// <param name="frontTaskDataId">Front task data identifier.</param>
		void addChildrenTasks(string frontTaskDataId) {
			if (childrenTasksMapping.ContainsKey(frontTaskDataId)) {
				List<string> childrenTasks = childrenTasksMapping[frontTaskDataId];
				for (int i = 0; i < childrenTasks.Count; i++) {
					AddNewTaskExceptType(childrenTasks[i].ToString(), TaskType.IsBindedWithEvent);
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
			RoleData hostRole = GetHostRoleData();
			bool canAccept;
			for (int i = 0; i < addedTasks.Count; i++) {
				task = addedTasks[i];
				canAccept = false;
				switch (task.Type) {
				case TaskType.Gender:
					if (hostRole != null && hostRole.Gender == (GenderType)task.IntValue) {
						canAccept = true;
					}
					break;
				case TaskType.ItemInHand:
					if (GetItemNumByItemId(task.StringValue) > 0) {
						canAccept = true;
					}
					break;
				case TaskType.MoralRange:
					if (hostRole != null && hostRole.Moral >= task.MinIntValue && hostRole.Moral < task.MaxIntValue) {
						canAccept = true;
					}
					break;
				case TaskType.Occupation:
					if (hostRole != null && hostRole.Occupation == (OccupationType)task.IntValue) {
						canAccept = true;
					}
					break;
				case TaskType.TheHour:
					if (FramePanelCtrl.CurrentTimeIndex == task.IntValue) {
						canAccept = true;
					}
					break;
				case TaskType.IsInArea:
					if (UserModel.CurrentUserData != null && UserModel.CurrentUserData.CurrentAreaSceneName == task.StringValue) {
						canAccept = true;
					}
					break;
				case TaskType.None:
				default:
					canAccept = true;
					break;

				}
				if (canAccept) {
					db = OpenDb();
					//讲符合接取条件的任务状态改变为可以接取任务
					task.State = TaskStateType.CanAccept;
					db.ExecuteQuery("update TasksTable set State = " + (int)task.State + 
						" where TaskId ='" + task.Id + "' and BelongToRoleId = '" + currentRoleId + "'");
					db.CloseSqlConnection();
					PlayerPrefs.SetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "NewTask", "true"); //标记新任务提示
				}
			}
		}

		/// <summary>
		/// 清空任务缓存数据
		/// </summary>
		public void ClearTaskCaches() {
			taskListData = null;
		}

		/// <summary>
		/// 判断是否需要初始化缓存数据
		/// </summary>
		void validTaskListData() {
			if (taskListData == null) {
				taskListData = new List<TaskData>();
				db = OpenDb();
				//正序查询处于战斗队伍中的角色
				SqliteDataReader sqReader = db.ExecuteQuery("select * from TasksTable where BelongToRoleId = '" + currentRoleId + "' and State >= 0 order by State desc");
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
						progressDataList.Add((short)TaskDialogStatusType.Initial);
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
		/// 添加一条新任务到可截取任务数据列表，但是不包括某一个特定类型的任务
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		/// <param name="exceptType">Except type.</param>
		public void AddNewTaskExceptType(string taskId, TaskType exceptType) {
			validTaskListData();
			if (getTask(taskId) == null) {
				db = OpenDb();
				TaskData taskData = JsonManager.GetInstance().GetMapping<TaskData>("Tasks", taskId);
				if (taskData.Type != exceptType) {
					if (taskData.Id == taskId) {
						//添加任务数据时把任务步骤存档字段也进行初始化
						JArray progressDataList = new JArray();
						for(int i = 0; i < taskData.Dialogs.Count; i++) {
							progressDataList.Add((short)TaskDialogStatusType.Initial);
						}
						db.ExecuteQuery("insert into TasksTable (TaskId, ProgressData, CurrentDialogIndex, State, BelongToRoleId) values('" + taskId + "', '" + progressDataList.ToString() + "', 0, 0, '" + currentRoleId + "')");
						//顺手把数据写入缓存
						taskData.State = TaskStateType.CanNotAccept;
						taskData.SetCurrentDialogIndex(0);
						taskData.ProgressData = progressDataList;
						taskData.MakeJsonToModel();
						taskListData.Add(taskData);
					}
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
			TaskData data = getTask(taskId);
			if (data != null) {
				if (data.State == TaskStateType.Completed) {
//					db.CloseSqlConnection();
					return;
				}
				string triggerNewBackTaskDataId = "";
				TaskDialogType dialogType = data.GetCurrentDialog().Type;
				bool canModify = false;
				JArray pushData = new JArray();
				TaskDialogData dialog = data.GetCurrentDialog();
				data.State = TaskStateType.Accepted;
				if (data.GetCurrentDialogStatus() == TaskDialogStatusType.Initial) {
					if (dialogType == TaskDialogType.JustTalk || dialogType == TaskDialogType.Notice) {
						//交谈步骤和信息显示步骤直接变为已读状态
						data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
					} 
					else {
						//其他状态的话需要等待下一个步骤提交时检测是否可以完成，所以先HoldOn
						data.SetCurrentDialogStatus(TaskDialogStatusType.HoldOn);
						bool loadEvents = false;
						//如果是动态战斗事件步骤需要在这里创建动态战斗事件
						if (dialogType == TaskDialogType.EventFightWined) {
							Debug.LogWarning("如果是动态战斗事件步骤需要在这里创建动态战斗事件");
							//创建一个区域大地图的战斗事件
							CreateNewEvent(SceneEventType.Battle, dialog.StringValue, UserModel.CurrentUserData.CurrentAreaSceneName);
							loadEvents = true;
						}
						//如果是区域大地图野外任务事件步骤需要在这里创建动态任务事件
						if (dialogType == TaskDialogType.CreateTaskIsBindedWithEvent) {
							Debug.LogWarning("如果是区域大地图野外任务事件步骤需要在这里创建动态任务事件");
							//创建一个区域大地图的战斗事件
							CreateNewEvent(SceneEventType.Task, dialog.StringValue, UserModel.CurrentUserData.CurrentAreaSceneName);
							loadEvents = true;
						}
						if (loadEvents) {
							//加载动态事件列表
							Messenger.Broadcast<string>(NotifyTypes.GetActiveEventsInArea, UserModel.CurrentUserData.CurrentAreaSceneName);
						}
					}
					dialog = data.GetCurrentDialog();
					pushData.Add(new JArray(dialog.Index.ToString(), dialog.Type, dialog.TalkMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
					canModify = true;
				}
				else {
					RoleData hostRole;
					switch(dialogType) {
					case TaskDialogType.Choice:
						if (!auto) {
							triggerNewBackTaskDataId = selectedNo ? data.GetCurrentDialog().BackNoTaskDataId : data.GetCurrentDialog().BackYesTaskDataId;
							data.SetCurrentDialogStatus(selectedNo ? TaskDialogStatusType.ReadNo : TaskDialogStatusType.ReadYes);
							//输出步骤执行结果信息
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, data.GetCurrentDialogStatus() == TaskDialogStatusType.ReadYes ? dialog.YesMsg : dialog.NoMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.ConvoyNpc: //暂时没考虑好怎么做护送npc任务
						data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
						pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
						canModify = true;
						break;
					case TaskDialogType.FightWined:
					case TaskDialogType.EventFightWined:
						if (IsFightWined(dialog.StringValue)) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.JustTalk:
					case TaskDialogType.Notice:
						canModify = true;
						break;
					case TaskDialogType.RecruitedThePartner:
						if (GetRoleDataByRoleId(dialog.StringValue) != null) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.SendItem:
						if (CostItemFromBag(dialog.StringValue, dialog.IntValue)) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.UsedTheBook:
						hostRole = GetHostRoleData();
						if (hostRole != null) {
							for (int i = 0; i < hostRole.ResourceBookDataIds.Count; i++) {
								if (hostRole.ResourceBookDataIds[i] == dialog.StringValue) {
									data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
									pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
									canModify = true;
									break;
								}
							}
						}
						break;
					case TaskDialogType.UsedTheSkillOneTime:
						if (GetUsedTheSkillTimes(dialog.StringValue) > 0) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.UsedTheWeapon:
						hostRole = GetHostRoleData();
						if (hostRole.ResourceWeaponDataId == dialog.StringValue) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.WeaponPowerPlusSuccessed:
						if (GetWeaponPowerPlusSuccessedTimes(dialog.IntValue) > 0) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.SendResource:
						if (CostResource((ResourceType)Enum.Parse(typeof(ResourceType), dialog.StringValue), dialog.IntValue)) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.TheHour:
						if (FramePanelCtrl.CurrentTimeIndex == dialog.IntValue) {
							data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
							pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
							canModify = true;
						}
						break;
					case TaskDialogType.PushRoleToWinshop:
						PushNewRoleToWinShop(dialog.StringValue);
						data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
						pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
						canModify = true;
						break;
					case TaskDialogType.CreateTaskIsBindedWithEvent:
						AddNewTask(dialog.StringValue);
						data.SetCurrentDialogStatus(TaskDialogStatusType.ReadYes);
						pushData.Add(new JArray(dialog.Index.ToString() + "_0", TaskDialogType.Notice, dialog.YesMsg, (short)data.GetCurrentDialogStatus(), dialog.IconId, dialog.StringValue));
						canModify = true;
						break;
					default:
						break;
					}
					if (canModify && data.CheckCompleted()) {
						data.State = TaskStateType.Completed;
					}
				}
				TaskStateType currentState = data.State;
				if (canModify) {
					db = OpenDb();
					//如果是可循环的任务这里需要置为可接受，否则置为已完成
					if (data.State == TaskStateType.Completed && data.CanRepeat) {
						data.State = TaskStateType.CanAccept;
						for (int a = 0; a < data.ProgressData.Count; a++) {
							data.ProgressData[a] = (short)TaskDialogStatusType.Initial;
						}
						data.SetCurrentDialogIndex(0);
					}
					//update data
					db.ExecuteQuery("update TasksTable set ProgressData = '" + data.ProgressData.ToString() + 
					                "', CurrentDialogIndex = " + data.CurrentDialogIndex + 
									", State = " + ((int)data.State) + 
					                " where TaskId ='" + taskId + "' and BelongToRoleId = '" + currentRoleId + "'");
					db.CloseSqlConnection();
					int index = taskListData.FindIndex(item => item.Id == taskId);
					//update cache
					if (taskListData.Count > index) {
						taskListData[index] = data;
					}
				}
				//触发新任务
				if (triggerNewBackTaskDataId != "" && triggerNewBackTaskDataId != "0") {
					AddNewTaskExceptType(triggerNewBackTaskDataId, TaskType.IsBindedWithEvent);
					//检测任务状态
					checkAddedTasksStatus();
				}
				if (currentState == TaskStateType.Completed) {
					//添加任务奖励物品
					//查询背包是否已满
					bool enoughBagSeat = false;
					int hasNum = GetItemNumLeftInBag(); //还剩余的背包位子
					if (hasNum > 0) {
						enoughBagSeat = true;
					}
					List<DropData> drops = PushItemToBag(data.Rewards);

					if (drops.Count > 0) {
						Messenger.Broadcast<List<DropData>>(NotifyTypes.ShowDropsListPanel, drops);
					}
					//任务完成后出发后续任务
					addChildrenTasks(data.Id);
					//任务完成后移除区域大地图上的任务事件
					RemoveTaskEvent(data.Id);
					//如果是就职任务则提示就职成功
					if (data.IsInaugurationTask) {
						RoleData hostRoleData = GetHostRoleData();
						if (hostRoleData.Occupation == OccupationType.None) {
							//加入门派
							hostRoleData.Occupation = data.InaugurationOccupation;
							hostRoleData.Disposed();
							db = OpenDb();
							db.ExecuteQuery("update RolesTable set RoleData = '" + JsonManager.GetInstance().SerializeObject(hostRoleData) + "' where Id = " + hostRoleData.PrimaryKeyId);
							//判断兵器是否属于本门派
							WeaponData currentWeapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", hostRoleData.ResourceWeaponDataId);
							if (currentWeapon.Occupation != OccupationType.None && currentWeapon.Occupation != hostRoleData.Occupation) {
								SqliteDataReader sqReader = db.ExecuteQuery("select * from WeaponsTable where BeUsingByRoleId = '" + currentRoleId + "' and BelongToRoleId ='" + currentRoleId + "'");
								while (sqReader.Read()) {
									//将兵器先卸下
									int dataId = sqReader.GetInt32(sqReader.GetOrdinal("Id"));
									db.ExecuteQuery("update WeaponsTable set BeUsingByRoleId = '' where Id = " + dataId);
								}
							}
							db.CloseSqlConnection();
							AlertCtrl.Show(string.Format("你已成功加入{0}!", Statics.GetOccupationName(hostRoleData.Occupation)));
						}
						else {
							AlertCtrl.Show("你已是有门有派之人, 不可在此另行拜师!");
						}
					}
				}
				Messenger.Broadcast<JArray>(NotifyTypes.CheckTaskDialogEcho, pushData);
				if (data.GetCurrentDialogStatus() == TaskDialogStatusType.ReadNo || data.GetCurrentDialogStatus() == TaskDialogStatusType.ReadYes) {
					data.NextDialogIndex();
				}
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
//			validTaskListData();
			checkAddedTasksStatus();
			List<TaskData> taskData = taskListData.FindAll(item => item.BelongToSceneId == cityId && item.State != TaskStateType.CanNotAccept && item.State != TaskStateType.Completed);
			Messenger.Broadcast<List<TaskData>>(NotifyTypes.GetTaskListDataInCitySceneEcho, taskData);
		}

		/// <summary>
		/// 获取当前任务列表数据
		/// </summary>
		public void GetTaskListData() {
			checkAddedTasksStatus();
			List<TaskData> taskData = taskListData.FindAll(item => item.State != TaskStateType.Completed);
			//判断任务的完成情况
			TaskData task;
			TaskDialogData dialog;
			RoleData hostRole;
			for (int i = taskData.Count - 1; i >= 0; i--) {
				//判断任务是否为区域专属类型，如果是需要根据当前所处区域判定是否需要显示出来
				task = taskData[i];
				if (task.Type == TaskType.IsInArea) {
					if (UserModel.CurrentUserData.CurrentAreaSceneName != task.StringValue) {
						taskData.RemoveAt(i);
						continue;
					}
				}
				dialog = task.GetCurrentDialog();
				switch (dialog.Type) {
				case TaskDialogType.ConvoyNpc: //暂时没考虑好怎么做护送npc任务
					
					break;
				case TaskDialogType.FightWined:
				case TaskDialogType.EventFightWined:
					if (IsFightWined(dialog.StringValue)) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.RecruitedThePartner:
					if (GetRoleDataByRoleId(dialog.StringValue) != null) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.SendItem:
					if (GetItemNumByItemId(dialog.StringValue) >= dialog.IntValue) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.UsedTheBook:
					hostRole = GetHostRoleData();
					if (hostRole != null) {
						for (int j = 0; j < hostRole.ResourceBookDataIds.Count; j++) {
							if (hostRole.ResourceBookDataIds[j] == dialog.StringValue) {
								dialog.Completed = true;
								break;
							}
						}
					}
					break;
				case TaskDialogType.UsedTheSkillOneTime:
					if (GetUsedTheSkillTimes(dialog.StringValue) > 0) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.UsedTheWeapon:
					hostRole = GetHostRoleData();
					if (hostRole.ResourceWeaponDataId == dialog.StringValue) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.WeaponPowerPlusSuccessed:
					if (GetWeaponPowerPlusSuccessedTimes(dialog.IntValue) > 0) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.SendResource:
					if (GetResourceNum((ResourceType)Enum.Parse(typeof(ResourceType), dialog.StringValue)) >= dialog.IntValue) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.TheHour:
					if (FramePanelCtrl.CurrentTimeIndex == dialog.IntValue) {
						dialog.Completed = true;
					}
					break;
				case TaskDialogType.PushRoleToWinshop:
				case TaskDialogType.CreateTaskIsBindedWithEvent:
					dialog.Completed = true;
					break;
				default:
					break;
				}
			}
			Messenger.Broadcast<List<TaskData>>(NotifyTypes.GetTaskListDataEcho, taskData);
		}

		/// <summary>
		/// 获取任务详情数据
		/// </summary>
		/// <param name="taskId">Task identifier.</param>
		public void GetTaskDetailInfoData(string taskId) {
			validTaskListData();
			TaskData data = getTask(taskId);
			if (data != null && (data.State != TaskStateType.CanNotAccept && data.State != TaskStateType.Completed)) {
				//就职任务执行之前判断性别
				if (data.IsInaugurationTask) {
					RoleData hostRoleData = GetHostRoleData();
					if (hostRoleData.Occupation != OccupationType.None) {
						AlertCtrl.Show("你已是有门有派之人, 不可在此另行拜师!");
						return;
					}
					if (hostRoleData.Gender == GenderType.Male) {
						if (data.InaugurationOccupation == OccupationType.EMei) {
							AlertCtrl.Show(string.Format("{0}派向来只收女弟子，这位公子请自重", Statics.GetOccupationName(OccupationType.EMei)));
							return;
						}
					}
					else {
						if (data.InaugurationOccupation == OccupationType.ShaoLin) {
							AlertCtrl.Show("阿弥陀佛，佛门乃清净之地，这位女施主请回");
							return;
						}
					}
				}
				//将任务数据转化成步骤信息发送给前端
				JArray dialogDataList = new JArray();
				TaskDialogData dialog;
				TaskDialogStatusType dialogStatus;
				for (int i = 0; i < data.Dialogs.Count; i++) {
					dialog = data.Dialogs[i];
					dialogStatus = data.GetDialogStatus(i);
					switch (dialogStatus) {
					case TaskDialogStatusType.HoldOn:
						dialogDataList.Add(new JArray(i.ToString(), dialog.Type, dialog.TalkMsg, (short)dialogStatus, dialog.IconId, dialog.StringValue));
						break;
					case TaskDialogStatusType.ReadYes:
					case TaskDialogStatusType.ReadNo:
						dialogDataList.Add(new JArray(i.ToString(), dialog.Type, dialog.TalkMsg, (short)dialogStatus, dialog.IconId, dialog.StringValue));
						//除了谈话和提示信息类的步骤外，将完成结果返回给前端
						if (dialog.Type != TaskDialogType.JustTalk && dialog.Type != TaskDialogType.Notice) {
							dialogDataList.Add(new JArray(i.ToString() + "_0", TaskDialogType.Notice, dialogStatus == TaskDialogStatusType.ReadYes ? dialog.YesMsg : dialog.NoMsg, (short)dialogStatus, dialog.IconId, dialog.StringValue));
						}
						break;
					default:
						break;
					}
				}
				Messenger.Broadcast<JArray>(NotifyTypes.ShowTaskDetailInfoPanel, new JArray(data.Id, dialogDataList));
			}
		}

		/// <summary>
		/// 判断是否借取或者完成了特定任务
		/// </summary>
		/// <returns><c>true</c> if this instance has any task the specified taskIds; otherwise, <c>false</c>.</returns>
		/// <param name="taskIds">Task identifiers.</param>
		public bool HasAnyTask(string[] taskIds) {
			bool result = false;
			for (int i = 0; i < taskIds.Length; i++) {
				if (getTask(taskIds[i]) != null) {
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
