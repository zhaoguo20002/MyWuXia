using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using DG;
using DG.Tweening;

namespace Game {
	public class RecordListPanelCtrl : WindowCore<RecordListPanelCtrl, JArray> {
		Image bg;
		Button block;
		GridLayoutGroup grid;
		Button closeBtn;

		List<JArray> recordsData;
		List<RecordItemContainer> recordContainers;
		Object prefabObj;
		int maxRecords = 5; //最多5个存档位子
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			grid = GetChildGridLayoutGroup("Grid");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;
			recordContainers = new List<RecordItemContainer>();
		}

		void onClick(GameObject e) {
			Back();
		}

		public void UpdateData(List<JArray> records) {
			//如果最近一次进入游戏的主角id不存在在当前存档列表中则使用第一个存档作为默认进入游戏的主角id
			string currentRoleId = PlayerPrefs.GetString("CurrentRoleId");
			if (currentRoleId == "") {
				if (records.Count > 0) {
					currentRoleId = records[0][1].ToString();
				}
				else {
					currentRoleId = "role_0";
				}
				PlayerPrefs.SetString("CurrentRoleId", currentRoleId);
				DbManager.Instance.SetCurrentRoleId(currentRoleId);
			}
			int len = records.Count;
			string roleId;
			recordsData = new List<JArray>();
			JArray find;
			for (int i = 0; i < maxRecords; i++) {
				roleId = "role_" + i;
				find = records.Find(d => d[1].ToString() == roleId);
				if (find != null) {
					recordsData.Add(find);
				}
				else {
					recordsData.Add(new JArray(0, roleId));
				}
			}
		}

		public override void RefreshView () {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/RecordItemContainer");
			}
			if (recordsData.Count > 0) {
				JArray record;
				GameObject itemPrefab;
				RecordItemContainer container;
				for (int i = 0; i < recordsData.Count; i++) {
					record = recordsData[i];
					if (recordContainers.Count <= i) {
						itemPrefab = Statics.GetPrefabClone(prefabObj);
						MakeToParent(grid.transform, itemPrefab.transform);
						container = itemPrefab.GetComponent<RecordItemContainer>();
						recordContainers.Add(container);
					}
					else {
						container = recordContainers[i];
					}
					container.UpdateData(record);
					container.RefreshView();
				}
				RectTransform trans = grid.GetComponent<RectTransform>();
				trans.sizeDelta = new Vector2(trans.sizeDelta.x, (grid.cellSize.y + grid.spacing.y) * recordContainers.Count - grid.spacing.y);
			}
		}

		public void Pop() {
			bg.transform.DOScale(0, 0);
			bg.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

		public static void Show(List<JArray> records) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/MainTool/RecordListPanelView", "RecordListPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(records);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
