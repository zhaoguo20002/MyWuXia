﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


		public TaskData() {
			Dialogs = new List<TaskDialogData>();
			BelongToNpcId = "";
			FrontTaskDataId = "0";
			BelongToSceneId = "";
			Rewards = new List<DropData>();
			CanRepeat = false;
		}

		/// <summary>
		/// 判断当前对话是否满足条件,满足则将对话索引加1
		/// </summary>
		/// <returns><c>true</c>, if dialog was checked, <c>false</c> otherwise.</returns>
		public bool CheckDialog() {
			if (Dialogs.Count <= _currentDialogIndex) {
				return false;
			}
			if (Dialogs[_currentDialogIndex].Check()) {
				_currentDialogIndex++;
			}
			return CheckCompleted();
		}

		/// <summary>
		/// 判断是否满足任务开启条件,满足后将任务Id存入数据库,标记为已经可以接取的任务
		/// </summary>
		public bool CheckOpen() {
			return false;
		}

		/// <summary>
		/// 标记指向下一个任务步骤
		/// </summary>
		public void NextDialogIndex() {
			if (_currentDialogIndex < Dialogs.Count - 1) {
				_currentDialogIndex++;
			}
		}

		/// <summary>
		/// 判断任务对话是否已经全部满足条件,表示任务任务完成,将任务Id存入数据库,标记任务已经完成
		/// </summary>
		/// <returns><c>true</c>, if completed was checked, <c>false</c> otherwise.</returns>
		public bool CheckCompleted() {
			return CurrentDialogIndex >= 0 && CurrentDialogIndex >= Dialogs.Count - 1;
		}

	}
}