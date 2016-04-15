using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class CityScenePanelCtrl : WindowCore<CityScenePanelCtrl, JArray> {
		CanvasGroup bg;
		Text sceneNameText;
		Button enterAreaBtn;
		Button enterWorkshopBtn;
		Button enterStoreBtn;
		Button enterHospitalBtn;
		Button enterInnBtn;
		Button enterYamenBtn;
		Button enterWinshopBtn;
		Button enterForbiddenAreaBtn;
		GridLayoutGroup npcsGrid;
		Dictionary<string, NpcContainer> npcContainersMapping;
		Image npcDialogImage;
		Text npcDialogText;

		SceneData sceneData;
		List<string> cityIds;
		List<TaskData> taskList;
		Object prefabObj;

		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			sceneNameText = GetChildText("sceneNameText");
			enterAreaBtn = GetChildButton("enterAreaBtn");
			EventTriggerListener.Get(enterAreaBtn.gameObject).onClick = onClick;
			enterWorkshopBtn = GetChildButton("enterWorkshopBtn");
			EventTriggerListener.Get(enterWorkshopBtn.gameObject).onClick = onClick;
			enterStoreBtn = GetChildButton("enterStoreBtn");
			EventTriggerListener.Get(enterStoreBtn.gameObject).onClick = onClick;
			enterHospitalBtn = GetChildButton("enterHospitalBtn");
			EventTriggerListener.Get(enterHospitalBtn.gameObject).onClick = onClick;
			enterInnBtn = GetChildButton("enterInnBtn");
			EventTriggerListener.Get(enterInnBtn.gameObject).onClick = onClick;
			enterYamenBtn = GetChildButton("enterYamenBtn");
			EventTriggerListener.Get(enterYamenBtn.gameObject).onClick = onClick;
			enterWinshopBtn = GetChildButton("enterWinshopBtn");
			EventTriggerListener.Get(enterWinshopBtn.gameObject).onClick = onClick;
			enterForbiddenAreaBtn = GetChildButton("enterForbiddenAreaBtn");
			EventTriggerListener.Get(enterForbiddenAreaBtn.gameObject).onClick = onClick;
			npcsGrid = GetChildGridLayoutGroup("npcsGrid");
			npcContainersMapping = new Dictionary<string, NpcContainer>();
			npcDialogImage = GetChildImage("NpcDialogImage");
			npcDialogText = GetChildText("NpcDialogText");
			cityIds = new List<string>();
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch (e.name) {
			case "enterAreaBtn":
//				Hide();
//				Messenger.Broadcast(NotifyTypes.FromCitySceneBackToArea);
				Messenger.Broadcast(NotifyTypes.GetReadyToTravelPanelData);
				break;
			case "enterWinshopBtn":
				Messenger.Broadcast<string>(NotifyTypes.GetRolesOfWinShopPanelData, sceneData.Id);
				break;
			case "enterWorkshopBtn":
				Messenger.Broadcast<string>(NotifyTypes.ShowWorkshopPanel, sceneData.Id);
				break;
			case "enterForbiddenAreaBtn":
				Messenger.Broadcast<string>(NotifyTypes.GetBooksOfForbiddenAreaPanelData, sceneData.Id);
				break;
			case "enterHospitalBtn":
				Messenger.Broadcast(NotifyTypes.GetHospitalPanelData);
				break;
			case "enterStoreBtn":
				Messenger.Broadcast<string>(NotifyTypes.GetStorePanelData, sceneData.Id);
				break;
			case "enterInnBtn":
				Messenger.Broadcast<string>(NotifyTypes.GetInnInCityData, sceneData.Id);
				break;
			default:
				break;
			}
		}

		public void UpdateData(SceneData data, List<string> ids) {
			sceneData = data;
			cityIds = ids;
			PlayBgm();
		}

		public override void RefreshView () {
			sceneNameText.text = sceneData.Name;
			foreach (NpcContainer container in npcContainersMapping.Values) {
				Destroy(container.gameObject);
			}
			npcContainersMapping.Clear();
			for (int i = 0; i < sceneData.Npcs.Count; i++) {
				createNpcContainer(sceneData.Npcs[i]);
			}
			//根据城镇传送开启情况控制功能按钮开启
			enterStoreBtn.gameObject.SetActive(false);
			enterHospitalBtn.gameObject.SetActive(false);
			enterInnBtn.gameObject.SetActive(false);
			enterYamenBtn.gameObject.SetActive(false);
			enterWinshopBtn.gameObject.SetActive(false);
			enterForbiddenAreaBtn.gameObject.SetActive(false);
			if (cityIds.FindIndex(id => id == "2") >= 0) {
				enterStoreBtn.gameObject.SetActive(true);
				enterHospitalBtn.gameObject.SetActive(true);
				enterInnBtn.gameObject.SetActive(true);
			}
			if (cityIds.FindIndex(id => id == "3") >= 0) {
				enterWinshopBtn.gameObject.SetActive(true);
				enterForbiddenAreaBtn.gameObject.SetActive(true);
			}
		}

		void createNpcContainer(NpcData npc) {
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/NpcItemContainer");
			}
			GameObject itemPrefab = Statics.GetPrefabClone(prefabObj);
			itemPrefab.name = npc.Id;
			MakeToParent(npcsGrid.transform, itemPrefab.transform);
			NpcContainer container = itemPrefab.GetComponent<NpcContainer>();
			container.SetNpcData(npc);
			npcContainersMapping.Add(npc.Id, container);
		}

		/// <summary>
		/// 添加任务到Npc身上
		/// </summary>
		/// <param name="list">List.</param>
		public void UpdateTaskToNpcData(List<TaskData> list) {
			taskList = list;
		}

		/// <summary>
		/// 刷新任务
		/// </summary>
		public void RefreshTaskToNpc() {
			TaskData taskData;
			for (int i = 0; i < taskList.Count; i++) {
				taskData = taskList[i];
				if (!npcContainersMapping.ContainsKey(taskData.BelongToNpcId)) {
					createNpcContainer(JsonManager.GetInstance().GetMapping<NpcData>("Npcs", taskData.BelongToNpcId));
				}
				npcContainersMapping[taskData.BelongToNpcId].UpdateTaskData(taskData.Id, taskData.State);
				npcContainersMapping[taskData.BelongToNpcId].RefreshTaskView();
			}
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f).SetDelay(0.3f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
		}

		/// <summary>
		/// 播放背景音乐
		/// </summary>
		public void PlayBgm() {
			SoundManager.GetInstance().PlayBGM(sceneData.BgmSoundId);
		}

		/// <summary>
		/// 获取城镇内任务列表
		/// </summary>
		public void GetTasks() {
			Messenger.Broadcast<string>(NotifyTypes.GetTaskListDataInCityScene, sceneData.Id);
		}

		public static void Show(SceneData data, List<string> ids) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/MainTool/CityScenePanelView", "CityScenePanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(data, ids);
			Ctrl.RefreshView();
		}

		public static void ShowTask(List<TaskData> list) {
			if (Ctrl != null) {
				Ctrl.UpdateTaskToNpcData(list);
				Ctrl.RefreshView();
				Ctrl.RefreshTaskToNpc();
			}
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
		}

		public static void MakeClose() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}

		/// <summary>
		/// 播放城镇的背景音乐
		/// </summary>
		public static void MakePlayBgm() {
			if (Ctrl != null) {
				Ctrl.PlayBgm();
			}
		}

		/// <summary>
		/// 获取城镇内任务列表
		/// </summary>
		public static void GetTasksInCityScene() {
			if (Ctrl != null) {
				Ctrl.GetTasks();
			}
		}
	}
}
