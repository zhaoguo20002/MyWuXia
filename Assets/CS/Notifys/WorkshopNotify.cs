using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 打开工坊主界面
		/// </summary>
		public static string ShowWorkshopPanel;
		/// <summary>
		/// 获取工坊主界面数据
		/// </summary>
		public static string GetWorkshopPanelData;
		/// <summary>
		/// 获取工坊主界面数据回调
		/// </summary>
		public static string GetWorkshopPanelDataEcho;
		/// <summary>
		/// 增减资源的工作家丁数
		/// </summary>
		public static string ChangeResourceWorkerNum;
		/// <summary>
		/// 增减资源的工作家丁数回调
		/// </summary>
		public static string ChangeResourceWorkerNumEcho;
		/// <summary>
		/// 刷新资源
		/// </summary>
		public static string ModifyResources;
		/// <summary>
		/// 刷新资源回调
		/// </summary>
		public static string ModifyResourcesEcho;
		/// <summary>
		/// 请求工坊中的武器打造标签页数据
		/// </summary>
		public static string GetWorkshopWeaponBuildingTableData;
		/// <summary>
		/// 请求工坊中的武器打造标签页数据回调
		/// </summary>
		public static string GetWorkshopWeaponBuildingTableDataEcho;
		/// <summary>
		/// 打造兵器
		/// </summary>
		public static string CreateNewWeaponOfWorkshop;
		/// <summary>
		/// 请求工坊中兵器分解标签页数据
		/// </summary>
		public static string GetWorkshopWeaponBreakingTableData;
		/// <summary>
		/// 请求工坊中兵器分解标签页数据
		/// </summary>
		public static string GetWorkshopWeaponBreakingTableDataEcho;
		/// <summary>
		/// 熔解兵器
		/// </summary>
		public static string BreakWeapon;
		/// <summary>
		/// 熔解兵器成功回调
		/// </summary>
		public static string BreakWeaponEcho;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// 工坊相关消息
		/// </summary>
		public static void WorkshopNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.ShowWorkshopPanel, (cityId) => {
				WorkshopPanelCtrl.Show(cityId);
			});
			Messenger.AddListener(NotifyTypes.GetWorkshopPanelData, () => {
				DbManager.Instance.GetWorkshopPanelData();
			});

			Messenger.AddListener<JArray>(NotifyTypes.GetWorkshopPanelDataEcho, (data) => {
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				WorkshopPanelCtrl.MakeGetWorkshopPanelDataEcho(data);
			});

			Messenger.AddListener<ResourceType, int>(NotifyTypes.ChangeResourceWorkerNum, (type, num) => {
				DbManager.Instance.ChangeResourceWorkerNum(type, num);
			});

			Messenger.AddListener<JArray>(NotifyTypes.ChangeResourceWorkerNumEcho, (data) => {
				WorkshopPanelCtrl.MakeChangeResourceWorkerNumEcho(data);
			});

			Messenger.AddListener(NotifyTypes.ModifyResources, () => {
				DbManager.Instance.ModifyResources();
			});

			Messenger.AddListener<JArray>(NotifyTypes.ModifyResourcesEcho, (data) => {
				WorkshopPanelCtrl.MakeModifyResourcesEcho(data);
			});

			Messenger.AddListener(NotifyTypes.GetWorkshopWeaponBuildingTableData, () => {
				DbManager.Instance.GetWorkshopWeaponBuildingTableData();
			});

			Messenger.AddListener<JArray>(NotifyTypes.GetWorkshopWeaponBuildingTableDataEcho, (data) => {
				WorkshopPanelCtrl.MakeGetWorkshopWeaponBuildingTableDataEcho(data);
			});

			Messenger.AddListener<string>(NotifyTypes.CreateNewWeaponOfWorkshop, (weaponId) => {
				DbManager.Instance.CreateNewWeaponOfWorkshop(weaponId);
			});

			Messenger.AddListener(NotifyTypes.GetWorkshopWeaponBreakingTableData, () => {
				DbManager.Instance.GetWorkshopWeaponBreakingTableData();
			});

			Messenger.AddListener<List<WeaponData>>(NotifyTypes.GetWorkshopWeaponBreakingTableDataEcho, (weapons) => {
				WorkshopPanelCtrl.MakeGetWorkshopWeaponBreakingTableDataEcho(weapons);
			});

			Messenger.AddListener<int>(NotifyTypes.BreakWeapon, (primaryKeyId) => {
				DbManager.Instance.BreakWeapon(primaryKeyId);
			});

			Messenger.AddListener<int>(NotifyTypes.BreakWeaponEcho, (primaryKeyId) => {
				WorkshopPanelCtrl.MakeBreakWeaponEcho(primaryKeyId);
				Statics.CreatePopMsg(Vector3.zero, "兵器被扔进熔炼炉内瞬间化为乌有", Color.white, 30);
			});
		}
	}
}