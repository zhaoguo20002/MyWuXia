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
		Image toggle0NewFlag;
		Image toggleGroup0;
		Toggle toggle1;
		Image toggle1NewFlag;
		Image toggleGroup1;
		Toggle toggle2;
		Image toggleGroup2;
		Button closeBtn;
		Text workerNumText;
		Text timerText;
		Text resultDescText;
		GridLayoutGroup resourceGrid;
		GridLayoutGroup weaponBuildingGrid;
		GridLayoutGroup weaponBreakingGrid;
		Text weaponNunText;
        Button plusWorkerBtn;

		string cityId;
		int workshopId;
		List<ResourceData> resources;
		List<ResourceData> resultResources;
		long ticks;
		int workerNum;
		int maxWorkerNum;
		List<WorkshopResourceContainer> resourceContainers;
		int modifyTimeout;
		Object prefabObj;
		List<WeaponData> weaponBuildings;
		List<WorkshopWeaponBuildingContainer> weaponBuildingContainers;
		Object prefabWeaponObj;
		List<WeaponData> weaponBreakings;
		List<WorkshopWeaponBreakingContainer> weaponBreakingContainers;
		Object prefabBreakWeaponObj;
		protected override void Init () {
			bg = GetComponent<CanvasGroup>();
			bg.DOFade(0, 0);
			toggle0 = GetChildToggle("Toggle0");
			toggle0.onValueChanged.AddListener(onValChanged0);
			toggle0NewFlag = GetChildImage("Toggle0NewFlag");
			toggle1 = GetChildToggle("Toggle1");
			toggle1.onValueChanged.AddListener(onValChanged1);
			toggle1NewFlag = GetChildImage("Toggle1NewFlag");
			toggle2 = GetChildToggle("Toggle2");
			toggle2.onValueChanged.AddListener(onValChanged2);
			toggleGroup0 = GetChildImage("WorkshopResourceTable");
			toggleGroup1 = GetChildImage("WorkshopWeaponBuildingTable");
			toggleGroup2 = GetChildImage("WorkshopWeaponBreakingTable");
			closeBtn = GetChildButton("CloseBtn");
			EventTriggerListener.Get(closeBtn.gameObject).onClick = onClick;

			workerNumText = GetChildText("WorkerNumText");
			timerText = GetChildText("TimerText");
			resultDescText = GetChildText("ResultDescText");
			resourceGrid = GetChildGridLayoutGroup("ResourceGrid");
			weaponBuildingGrid = GetChildGridLayoutGroup("WeaponBuildingGrid");
			weaponBreakingGrid = GetChildGridLayoutGroup("WeaponBreakingGrid");

			weaponNunText = GetChildText("WeaponNunText");

            plusWorkerBtn = GetChildButton("plusWorkerBtn");
            EventTriggerListener.Get(plusWorkerBtn.gameObject).onClick = onClick;

			resourceContainers = new List<WorkshopResourceContainer>();
			weaponBuildingContainers = new List<WorkshopWeaponBuildingContainer>();
			weaponBreakingContainers = new List<WorkshopWeaponBreakingContainer>();
		}

		/// <summary>
		/// 判断新增提示标记
		/// </summary>
		void checkNewFlags() {
			string headStr = PlayerPrefs.GetString("CurrentRoleId") + "_";
			//判断工坊里资源是否有新增
			bool newFlagForResource = false;
			for (int i = CitySceneModel.ResourceTypeStrOfWorkShopNewFlagList.Count - 1; i >= 0; i--) {
				if (string.IsNullOrEmpty(PlayerPrefs.GetString(headStr + "ResourceTypeStrOfWorkShopNewFlagIsHide_" + CitySceneModel.ResourceTypeStrOfWorkShopNewFlagList[i]))) {
					newFlagForResource = true;
					break;
				}
			}
			toggle0NewFlag.gameObject.SetActive(newFlagForResource);

			//判断工坊里锻造兵器是否有新增
			bool newFlagForWeapon = false;
			for (int i = CitySceneModel.WeaponIdOfWorkShopNewFlagList.Count - 1; i >= 0; i--) {
				if (string.IsNullOrEmpty(PlayerPrefs.GetString(headStr + "WeaponIdOfWorkShopNewFlagIsHide_" + CitySceneModel.WeaponIdOfWorkShopNewFlagList[i]))) {
					newFlagForWeapon = true;
					break;
				}
			}
			toggle1NewFlag.gameObject.SetActive(newFlagForWeapon);
		}

		void onValChanged0(bool check) {
			if (!toggleGroup0.gameObject.activeSelf) {
				toggle0.graphic.color = new Color(1, 1, 0, 0.5f);
				toggle0.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 0, 1);
				toggle1.graphic.color = new Color(1, 1, 1, 1);
				toggle1.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
				toggle2.graphic.color = new Color(1, 1, 1, 1);
				toggle2.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
				toggleGroup0.gameObject.SetActive(check);
				toggleGroup1.gameObject.SetActive(!check);
				toggleGroup2.gameObject.SetActive(!check);
				weaponNunText.gameObject.SetActive(false);
				Messenger.Broadcast(NotifyTypes.GetWorkshopPanelData);
				checkNewFlags();
			}
		}

		void onValChanged1(bool check) {
			if (!toggleGroup1.gameObject.activeSelf) {
				toggle0.graphic.color = new Color(1, 1, 1, 1);
				toggle0.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
				toggle1.graphic.color = new Color(1, 1, 0, 0.5f);
				toggle1.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 0, 1);
				toggle2.graphic.color = new Color(1, 1, 1, 1);
				toggle2.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
				toggleGroup0.gameObject.SetActive(!check);
				toggleGroup1.gameObject.SetActive(check);
				toggleGroup2.gameObject.SetActive(!check);
				weaponNunText.gameObject.SetActive(false);
				Messenger.Broadcast(NotifyTypes.GetWorkshopWeaponBuildingTableData);
				checkNewFlags();
			}
		}

		void onValChanged2(bool check) {
			if (!toggleGroup2.gameObject.activeSelf) {
				toggle0.graphic.color = new Color(1, 1, 1, 1);
				toggle0.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
				toggle1.graphic.color = new Color(1, 1, 1, 1);
				toggle1.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 1);
				toggle2.graphic.color = new Color(1, 1, 0, 0.5f);
				toggle2.graphic.GetComponentInChildren<Text>().color = new Color(1, 1, 0, 1);
				toggleGroup0.gameObject.SetActive(!check);
				toggleGroup1.gameObject.SetActive(!check);
				toggleGroup2.gameObject.SetActive(check);
				weaponNunText.gameObject.SetActive(true);
				Messenger.Broadcast(NotifyTypes.GetWorkshopWeaponBreakingTableData);
				checkNewFlags();
			}
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "plusWorkerBtn":
                    if (DbManager.Instance.GetPlusWorkerNum() < DbManager.Instance.GetMaxPlusWorkerNum())
                    {
                        ConfirmCtrl.Show(string.Format("花费¥3 购买10个家丁({0}/{1})\n(家丁越多资源生产效率越高)\n确定购买？", DbManager.Instance.GetPlusWorkerNum(), DbManager.Instance.GetMaxPlusWorkerNum()), () => {
                            MaiHandler.PayForProduct("com.courage2017.worker_10");
                        }, null, "购买", "不了");
                    }
                    else
                    {
                        AlertCtrl.Show(string.Format("你已经买满了{0}个家丁", DbManager.Instance.GetMaxPlusWorkerNum()));
                    }
                    break;
                case "CloseBtn":
                    FadeOut();
                    break;
                default:
                    break;
            }
		}

		/// <summary>
		/// 切换标签
		/// </summary>
		/// <param name="index">Index.</param>
		public void ChangeTab(int index) {
			switch (index) {
			case 0:
			default:
				onValChanged0(true);
				break;
			case 1:
				onValChanged1(true);
				break;
			case 2:
				onValChanged2(true);
				break;
			}
		}

		public void UpdateData(string cityid) {
			cityId = cityid;
		}

		public override void UpdateData(object obj) {
			JArray data = (JArray)obj;
			workshopId = (int)data[0];
			resources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[1].ToString());
//			workerNum = (int)data[2];
//			maxWorkerNum = (int)data[3];
            UpdateData((int)data[2], (int)data[3]);
			resultResources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[4].ToString());
		}

		public override void RefreshView () {
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

        public void UpdateData(int wn, int mwn) {
            workerNum = wn;
            maxWorkerNum = mwn;
        }

        public void RefreshWorkerNumView() {
            workerNumText.text = string.Format("家丁: {0}/{1}", workerNum, maxWorkerNum);
        }

		/// <summary>
		/// 刷新工坊分配资源后的界面
		/// </summary>
		public void RefreshResultResourcesView() {
            RefreshWorkerNumView();
			string desc = "";
			if (resultResources.Count > 0) {
				for (int i = 0; i < resultResources.Count; i++) {
					desc += (Statics.GetResourceName(resultResources[i].Type) + (resultResources[i].Num > 0 ? "+" + resultResources[i].Num.ToString() : resultResources[i].Num.ToString())) + " ";
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
			List<ResourceData> _receiveResources = JsonManager.GetInstance().DeserializeObject<List<ResourceData>>(data[2].ToString());
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
			string msg = "";
			for (int i = 0; i < receiveResources.Count; i++) {
				receive = receiveResources[i];
				if (receive.Num != 0) {
					msg += string.Format("<color=\"{2}\">{0} {1}</color>\n", Statics.GetResourceName(receive.Type), (receive.Num > 0 ? ("+" + receive.Num.ToString()) : receive.Num.ToString()), receive.Num > 0 ? "#00FF00" : "#FF0000");
					if (toggleGroup0.gameObject.activeSelf) {
						findContainer = resourceContainers.Find(item => item.Type == receive.Type);
						//更新资源的工作家丁数
						if (findContainer != null) {
							findContainer.UpdateNum(receive.Num);
						}
					}
				}
			}
			if (msg != "") {
				Statics.CreatePopMsg(Vector3.zero, msg, Color.white, 30);
			}
			//刷新产出总量
			if (_receiveResources.Count > 0) {
				resultResources = _receiveResources;
				RefreshResultResourcesView();
			}
		}

		public void UpdateWeaponBuildingData(JArray data) {
			weaponBuildings = new List<WeaponData>();
			for (int i = 0; i < data.Count; i++) {
				weaponBuildings.Add(JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", data[i].ToString()));
			}
            weaponBuildings.Sort((a, b) => b.Quality.CompareTo(a.Quality));
		}

		public void RefreshWeaponBuildingView() {
			if (prefabWeaponObj == null) {
				prefabWeaponObj = Statics.GetPrefab("Prefabs/UI/GridItems/WorkshopWeaponBuildingContainer");
			}
			GameObject itemPrefab;
			WorkshopWeaponBuildingContainer container;
			WeaponData weapon;
			for (int i = 0; i < weaponBuildings.Count; i++) {
				weapon = weaponBuildings[i];
				if (weaponBuildingContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabWeaponObj);
					MakeToParent(weaponBuildingGrid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<WorkshopWeaponBuildingContainer>();
					weaponBuildingContainers.Add(container);
				}
				else {
					container = weaponBuildingContainers[i];
				}
				container.UpdateData(weapon);
				container.RefreshView();
			}
			RectTransform trans = weaponBuildingGrid.GetComponent<RectTransform>();
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, (weaponBuildingGrid.cellSize.y + weaponBuildingGrid.spacing.y) * Mathf.Ceil(weaponBuildingContainers.Count * 0.5f) - weaponBuildingGrid.spacing.y);
		}

		public void UpdateWeaponBreakingData(List<WeaponData> weapons) {
			weaponBreakings = weapons;
		}

		public void RefreshWeaponBreakingView() {
			weaponNunText.text = string.Format("兵器匣:{0}/{1}", weaponBreakings.Count, DbManager.Instance.MaxWeaponNum);
			if (prefabBreakWeaponObj == null) {
				prefabBreakWeaponObj = Statics.GetPrefab("Prefabs/UI/GridItems/WorkshopWeaponBreakingContainer");
			}
			GameObject itemPrefab;
			WorkshopWeaponBreakingContainer container;
			WeaponData weapon;
			for (int i = 0; i < weaponBreakings.Count; i++) {
				weapon = weaponBreakings[i];
				if (weaponBreakingContainers.Count <= i) {
					itemPrefab = Statics.GetPrefabClone(prefabBreakWeaponObj);
					MakeToParent(weaponBreakingGrid.transform, itemPrefab.transform);
					container = itemPrefab.GetComponent<WorkshopWeaponBreakingContainer>();
					weaponBreakingContainers.Add(container);
				}
				else {
					container = weaponBreakingContainers[i];
				}
				container.UpdateData(weapon);
				container.RefreshView();
			}
			RectTransform trans = weaponBreakingGrid.GetComponent<RectTransform>();
			trans.sizeDelta = new Vector2(trans.sizeDelta.x, (weaponBreakingGrid.cellSize.y + weaponBreakingGrid.spacing.y) * Mathf.Ceil(weaponBreakingContainers.Count * 0.5f) - weaponBreakingGrid.spacing.y);
		}

		/// <summary>
		/// 熔解兵器成功回调
		/// </summary>
		/// <param name="primaryKeyId">Primary key identifier.</param>
		public void BreakWeaponEcho(int primaryKeyId) {
			int index = weaponBreakings.FindIndex(item => item.PrimaryKeyId == primaryKeyId);
			if (index >= 0) {
				weaponBreakings.RemoveAt(index);
				if (weaponBreakingContainers.Count > index) {
					Destroy(weaponBreakingContainers[index].gameObject);
					weaponBreakingContainers.RemoveAt(index);
				}
			}
		}

		public void FadeIn() {
			bg.DOFade(1, 0.5f);
		}

		public void FadeOut() {
			bg.DOFade(0, 0.5f).OnComplete(() => {
				Close();
			});
			Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
			Messenger.Broadcast(NotifyTypes.MakeCheckNewFlags); //判断城镇界面的新增提示
		}

		public static void Show(string cityid) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Workshop/WorkshopPanelView", "WorkshopPanelCtrl");
				Ctrl.FadeIn();
			}
			Ctrl.UpdateData(cityid);
			Ctrl.ChangeTab(0);
        }

        public static void Hide() {
            if (Ctrl != null) {
                Ctrl.Close();
            }
        }

		/// <summary>
		/// 请求原材料数据回调
		/// </summary>
		/// <param name="data">Data.</param>
		public static void MakeGetWorkshopPanelDataEcho(JArray data) {
			if (Ctrl != null) {
				Ctrl.UpdateData(data);
				Ctrl.RefreshView();
			}
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

		/// <summary>
		/// 请求工坊中的武器打造标签页数据回调
		/// </summary>
		/// <param name="weapons">Weapons.</param>
		public static void MakeGetWorkshopWeaponBuildingTableDataEcho(JArray data) {
			if (Ctrl != null) {
				Ctrl.UpdateWeaponBuildingData(data);
				Ctrl.RefreshWeaponBuildingView();
			}
		}

		/// <summary>
		/// 请求工坊中的武器熔解标签页数据回调
		/// </summary>
		/// <param name="data">Data.</param>
		public static void MakeGetWorkshopWeaponBreakingTableDataEcho(List<WeaponData> weapons) {
			if (Ctrl != null) {
				Ctrl.UpdateWeaponBreakingData(weapons);
				Ctrl.RefreshWeaponBreakingView();
			}
		}

		/// <summary>
		/// 熔解兵器成功回调
		/// </summary>
		/// <param name="primaryKeyId">Primary key identifier.</param>
		public static void MakeBreakWeaponEcho(int primaryKeyId) {
			if (Ctrl != null) {
				Ctrl.BreakWeaponEcho(primaryKeyId);
			}
		}

        /// <summary>
        /// 刷新家丁数量
        /// </summary>
        /// <param name="wn">Wn.</param>
        /// <param name="mwn">Mwn.</param>
        public static void MakeWorkerNumChange(int wn, int mwn) {
            if (Ctrl != null)
            {
                Ctrl.UpdateData(wn, mwn);
                Ctrl.RefreshWorkerNumView();
            }
        }

		void OnDestroy() {
			Timer.RemoveTimer("WorkshopModifyResourceTimer");
		}
	}
}
