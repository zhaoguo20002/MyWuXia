using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Game {
	/// <summary>
	/// 任务数据
	/// </summary>
	public class TaskData {
		/// <summary>
		/// 任务主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 任务名称
		/// </summary>
		public string Name;

		/// <summary>
		/// 任务描述
		/// </summary>
		public string Desc;

		/// <summary>
		/// 任务对话列表
		/// </summary>
		public List<TaskDialogData> Dialogs;

		int _currentDialogIndex = 0;
		/// <summary>
		/// 当前任务进行到的对话索引
		/// </summary>
		/// <value>The index of the current dialog.</value>
		public int CurrentDialogIndex {
			get {
				return _currentDialogIndex;
			}
		}

		/// <summary>
		/// 任务绑定的Npc Id
		/// </summary>
		public string BelongToNpcId;

		/// <summary>
		/// 前置任务Id[用于判断是否有前置任务从而开启新任务]
		/// 如果有前置任务的话,前置任务是否完成,前置任务为0则无前置任务
		/// </summary>
		public string FrontTaskDataId;

		/// <summary>
		/// 任务接取类型
		/// </summary>
		public TaskType Type;

		/// <summary>
		/// 所属区域名[用于判断是否开启区域大地图传送从而开启新任务]
		/// </summary>
		public string BelongToSceneId;
		/// <summary>
		/// 字符串类型值
		/// </summary>
		public string StringValue;
		/// <summary>
		/// 整型值
		/// </summary>
		public int IntValue;
		/// <summary>
		/// 整型值区间最小值
		/// </summary>
		public int MinIntValue;
		/// <summary>
		/// 整型值区间最大值
		/// </summary>
		public int MaxIntValue;
		/// <summary>
		/// 任务奖励
		/// </summary>
		public List<DropData> Rewards;
		/// <summary>
		/// 是否能重复接取任务
		/// </summary>
		public bool CanRepeat;
		/// <summary>
		/// 当前任务状态
		/// </summary>
		public TaskStateType State;
		/// <summary>
		/// 任务进度缓存数据表
		/// 这个进度记录获取的时候需要用CurrentDialogIndex+1来获取，因为一开始初始化的时候往进度里多加了一个记录
		/// </summary>
		public JArray ProgressData;


		public TaskData() {
			Desc = "";
			Dialogs = new List<TaskDialogData>();
			BelongToNpcId = "";
			FrontTaskDataId = "0";
			BelongToSceneId = "";
			Rewards = new List<DropData>();
			ProgressData = new JArray();
			CanRepeat = false;
		}

//		/// <summary>
//		/// 判断当前对话是否满足条件,满足则将对话索引加1
//		/// </summary>
//		/// <returns><c>true</c>, if dialog was checked, <c>false</c> otherwise.</returns>
//		public bool CheckDialog() {
//			if (Dialogs.Count <= _currentDialogIndex) {
//				return false;
//			}
//			if (Dialogs[_currentDialogIndex].Check()) {
//				_currentDialogIndex++;
//			}
//			return CheckCompleted();
//		}
//
//		/// <summary>
//		/// 判断是否满足任务开启条件,满足后将任务Id存入数据库,标记为已经可以接取的任务
//		/// </summary>
//		public bool CheckOpen() {
//			return false;
//		}

		/// <summary>
		/// 标记指向下一个任务步骤
		/// </summary>
		/// <param name="selectedNo">If set to <c>true</c> selected no.</param>
		public void NextDialogIndex(bool selectedNo = false) {
			if (_currentDialogIndex <= Dialogs.Count - 1) {
				if (ProgressData.Count > _currentDialogIndex) {
					TaskDialogStatusType dialogStatus = (TaskDialogStatusType)((short)ProgressData[_currentDialogIndex]);
					switch(dialogStatus) {
					case TaskDialogStatusType.Initial:
						ProgressData[_currentDialogIndex] = (short)TaskDialogStatusType.HoldOn;
						break;
					case TaskDialogStatusType.HoldOn:
						ProgressData[_currentDialogIndex] = (short)(selectedNo ? TaskDialogStatusType.ReadNo : TaskDialogStatusType.ReadYes);
						if (_currentDialogIndex < Dialogs.Count - 1) {
							_currentDialogIndex++;
						}
						break;
					default:
						break;
					}
				}
			}
		}

		/// <summary>
		/// 判断任务对话是否已经全部满足条件,表示任务任务完成,将任务Id存入数据库,标记任务已经完成
		/// </summary>
		/// <returns><c>true</c>, if completed was checked, <c>false</c> otherwise.</returns>
		public bool CheckCompleted() {
			return CurrentDialogIndex >= 0 && CurrentDialogIndex >= Dialogs.Count - 1;
		}

		/// <summary>
		/// 设置任务步骤进度
		/// </summary>
		/// <param name="index">Index.</param>
		public void SetCurrentDialogIndex(int index) {
			if (Dialogs.Count > index) {
				_currentDialogIndex = index;
			}
		}

		/// <summary>
		/// 获取上一个任务步骤
		/// </summary>
		/// <returns>The preview dialog.</returns>
		public TaskDialogData GetPreviewDialog() {
			if (CurrentDialogIndex > 0 && Dialogs.Count > CurrentDialogIndex - 1) {
				return Dialogs[CurrentDialogIndex - 1];
			}
			return null;
		}

		/// <summary>
		/// 获取当前任务步骤
		/// </summary>
		/// <returns>The current dialog.</returns>
		public TaskDialogData GetCurrentDialog() {
			if (Dialogs.Count > CurrentDialogIndex) {
				return Dialogs[CurrentDialogIndex];
			}
			return null;
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			for (int i = 0; i < Dialogs.Count; i++) {
				Dialogs [i].Index = i;
			}
			for (int i = 0; i < Rewards.Count; i++) {
				Rewards[i].MakeJsonToModel();
			}
		}

		/// <summary>
		/// 获取当前任务步骤的状态
		/// </summary>
		/// <returns>The current dialog status.</returns>
		public TaskDialogStatusType GetCurrentDialogStatus() {
			if (ProgressData.Count > CurrentDialogIndex) {
				return (TaskDialogStatusType)((short)ProgressData[CurrentDialogIndex]);
			}
			return TaskDialogStatusType.ReadYes;
		}

		/// <summary>
		/// 设置当前任务步骤的状态
		/// </summary>
		/// <param name="status">Status.</param>
		public void SetCurrentDialogStatus(TaskDialogStatusType status) {
			ProgressData[CurrentDialogIndex] = (short)status;
		}

	}
}
