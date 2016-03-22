using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
	public class WorkshopPanelCtrl : WindowCore<WorkshopPanelCtrl, JArray> {
		CanvasGroup bg;
		Toggle toggle0;
		Image toggleGroup0;
		Toggle toggle1;
		Image toggleGroup1;
		Button closeBtn;
		Text workerNumText;
		Text timerText;
		Text resultDescText;
		GridLayoutGroup resourceGrid;

		int workshopId;
		List<ResourceData> resources;
		List<ResourceData> resultResources;
		long ticks;
		int workerNum;
		int maxWorkerNum;
		List<WorkshopResourceContainer> resourceContainers;
		int modifyTimeout;
		Object prefabObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			toggle0 = GetChildToggle("Toggle0");
			toggle0.onValueChanged.AddListener(onValChanged0);
			toggle1 = GetChildToggle("Toggle1");
			toggle1.onValueChanged.AddListener(onValChanged1);
			toggleGroup0 = GetChildImage("WorkshopResourceTable");
			toggleGroup1 = GetChildImage("WorkshopWeaponBuildingTable");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;

			workerNumText = GetChildText("WorkerNumText");
			timerText = GetChildText("TimerText");
			resultDescText = GetChildText("ResultDescText");
			resourceGrid = GetChildGridLayoutGroup("ResourceGrid");

			resourceContainers = new List<WorkshopResourceContainer>();
		}

		void onValChanged0(bool check) {
			toggle0.graphic.color = new Color(1, 1, 0, 0.5f);
			toggle0.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 0, 1);
			toggle1.graphic.color = new Color(1, 1, 1, 1);
			toggle1.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
			toggleGroup0.gameObject.SetActive(check);
			toggleGroup1.gameObject.SetActive(!check);
		}

		void onValChanged1(bool check) {
			toggle0.graphic.color = new Color(1, 1, 1, 1);
			toggle0.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
			toggle1.graphic.color = new Color(1, 1, 0, 0.5f);
			toggle1.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 0, 1);
			toggleGroup0.gameObject.SetActive(!check);
			toggleGroup1.gameObject.SetActive(check);
		}

		void onClick(GameObject e) {
			FadeOut();
		}

		public override void UpdateData(object obj) {
			JArray data = (JArray)obj;
			workshopId = (int)data[0];
			resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[1].ToString());
			workerNum = (int)data[2];
			maxWorkerNum = (int)data[3];
			resultResources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[4].ToString());
		}

		public override void RefreshView () {
			onValChanged0(true);
			RefreshResultResourcesView();
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/WorkshopResourceContainer");
			}
			GameObject itemPrefab;
			WorkshopResourceContainer container;
			ResourceData resource;
			for (int i = 0; i < resources.Count; i++) {
				resource = resources[i];
				if (resourceContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabObj);
					MakeToParent(resourceGrid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<WorkshopResourceContainer>();
					resourceContainers.Add(container);
				}
				else {
					container = resourceContainers[i];
				}
				container.UpdateData(resource);
				container.RefreshView();
			}
			RectTransform trans = resourceGrid.GetComponent<RectTransform>();
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, (resourceGrid.cellSize.y + resourceGrid.spacing.y) * Mathf.Ceil(resourceContainers.Count * 0.5f) - resourceGrid.spacing.y);
			Messenger.Broadcast(NotifyTypes.ModifyResources);
		}

		/// <summary>
		/// 刷新工坊分配资源后的界面
		/// </summary>
		public void RefreshResultResourcesView() {
			workerNumText.text = string.Format("家丁: {0}/{1}", workerNum, maxWorkerNum);
			string desc = "";
			if (resultResources.Count > 0) {
				for (int i = 0; i < resultResources.Count; i++) {
					desc += (Statics.GetResourceName(resultResources[i].Type) + (resultResources[i].Num > 0 ? "+" + resultResources[i].Num.ToString() : resultResources[i].Num.ToString()));
				}
			}
			else {
				desc = "没有任何生产材料产出";
			}
			resultDescText.text = desc;
		}

		/// <summary>
		/// 增减资源的工作家丁数回调
		/// </summary>
		/// <param name="data">Data.</param>
		public void ChangeResourceWorkerNumEcho(JArray data) {
			ResourceType type = (ResourceType)((short)data[0]);
			int busyWorkerNum = (int)data[1];
			workerNum = (int)data[2];
			maxWorkerNum = (int)data[3];
			resultResources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[4].ToString());
			ResourceData findResource = resources.Find(item => item.Type == type);
			if (findResource != null) {
				findResource.WorkersNum = busyWorkerNum;
				WorkshopResourceContainer findContainer = resourceContainers.Find(item => item.Type == findResource.Type);
				//更新资源的工作家丁数
				if (findContainer != null) {
					findContainer.UpdateData(busyWorkerNum);
					findContainer.RefreshView();
				}
			}
			//更新产量信息
			RefreshResultResourcesView();
		}

		/// <summary>
		/// 请求资源累加数据
		/// </summary>
		/// <param name="data">Data.</param>
		public void ModifyResourcesEcho(JArray data) {
			modifyTimeout = (int)data[0] + 1;
			List<ResourceData> receiveResources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[1].ToString());
			Timer.RemoveTimer("WorkshopModifyResourceTimer");
			timerText.text = string.Format("下次刷新: {0}", Statics.GetTime(modifyTimeout));
			Timer.AddTimer("WorkshopModifyResourceTimer", modifyTimeout, (timer) => {
				timerText.text = string.Format("下次刷新: {0}", Statics.GetTime(timer.Second));
			}, (timer) => {
				timerText.text = string.Format("下次刷新: {0}", Statics.GetTime(timer.Second));
				Messenger.Broadcast(NotifyTypes.ModifyResources);
			});
			ResourceData receive;
			WorkshopResourceContainer findContainer;
			for (int i = 0; i < receiveResources.Count; i++) {
				receive = receiveResources[i];
				Statics.CreatePopMsg(new Vector3(0, i * 0.3f, 0), string.Format("{0}+{1}", Statics.GetResourceName(receive.Type), receive.Num), receive.Num > 0 ? Color.green : Color.red, 30);
				findContainer = resourceContainers.Find(item => item.Type == receive.Type);
				//更新资源的工作家丁数
				if (findContainer != null) {
					findContainer.UpdateNum(receive.Num);
				}
			}
		}

		public void UpdateWeaponBuildingData() {
			
		}

		public void RefreshWeaponBuildingView() {
			
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
			Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
		}

		public static void Show(JArray data) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Workshop/WorkshopPanelView", "WorkshopPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
		}

		/// <summary>
		/// 增减资源的工作家丁数回调
		/// </summary>
		/// <param name="data">Data.</param>
		public static void MakeChangeResourceWorkerNumEcho(JArray data) {
			if (Ctrl != null) {
				Ctrl.ChangeResourceWorkerNumEcho(data);
			}
		}

		/// <summary>
		/// 请求资源累加数据
		/// </summary>
		/// <param name="data">Data.</param>
		public static void MakeModifyResourcesEcho(JArray data) {
			if (Ctrl != null) {
				Ctrl.ModifyResourcesEcho(data);
			}
		}

		void OnDestroy() {
			Timer.RemoveTimer("WorkshopModifyResourceTimer");
		}
	}
}
