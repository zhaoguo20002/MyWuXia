﻿using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 请求队伍信息面板数据
		/// </summary>
		public static string CallRoleInfoPanelData;
		/// <summary>
		/// 请求队伍信息面板数据回调
		/// </summary>
		public static string CallRoleInfoPanelDataEcho;
		/// <summary>
		/// 关闭队伍信息面板
		/// </summary>
		public static string HideRoleInfoPanel;
		/// <summary>
		/// 控制角色属性面板是否可以切换角色
		/// </summary>
		public static string MakeChangeRoleEnable;
		/// <summary>
		/// 控制角色属性面板是否可以切书
		/// </summary>
		public static string MakeChangeBookEnable;
		/// <summary>
		/// 禁用/启用队伍信息面板交互
		/// </summary>
		public static string MakeRoleInfoPanelDisable;
		/// <summary>
		/// 请求酒馆侠客列表数据
		/// </summary>
		public static string GetRolesOfWinShopPanelData;
		/// <summary>
		/// 请求酒馆侠客列表数据回调
		/// </summary>
		public static string GetRolesOfWinShopPanelDataEcho;
		/// <summary>
		/// 获取兵器匣界面数据
		/// </summary>
		public static string GetWeaponsListPanelData;
		/// <summary>
		/// 获取兵器匣界面数据回调
		/// </summary>
		public static string GetWeaponsListPanelDataEcho;
		/// <summary>
		/// 替换兵器
		/// </summary>
		public static string ReplaceWeapon;
		/// <summary>
		/// 请求书筐界面数据
		/// </summary>
		public static string GetBooksListPanelData;
		/// <summary>
		/// 请求书筐界面数据回调
		/// </summary>
		public static string GetBooksListPanelDataEcho;
		/// <summary>
		/// 装备秘籍
		/// </summary>
		public static string UseBook;
		/// <summary>
		/// 卸下秘籍
		/// </summary>
		public static string UnuseBook;
		/// <summary>
		/// 请求特定城镇秘境中的秘籍数据
		/// </summary>
		public static string GetBooksOfForbiddenAreaPanelData;
		/// <summary>
		/// 请求特定城镇秘境中的秘籍数据回调
		/// </summary>
		public static string GetBooksOfForbiddenAreaPanelDataEcho;
		/// <summary>
		/// 结交侠客
		/// </summary>
		public static string InviteRole;
		/// <summary>
		/// 研读秘籍
		/// </summary>
		public static string ReadBook;
		/// <summary>
		/// 获取准备出发界面数据
		/// </summary>
		public static string GetReadyToTravelPanelData;
		/// <summary>
		/// 获取准备出发界面数据回调
		/// </summary>
		public static string GetReadyToTravelPanelDataEcho;
		/// <summary>
		/// 选中角色进队伍
		/// </summary>
		public static string MakeSelectRoleInTeam;
		/// <summary>
		/// 取消选中角色进队伍
		/// </summary>
		public static string MakeUnSelectRoleInTeam;
		/// <summary>
		/// 改变角色阵容
		/// </summary>
		public static string ChangeRolesSeatNo;
		/// <summary>
		/// 改变角色阵容回调
		/// </summary>
		public static string ChangeRolesSeatNoEcho;
		/// <summary>
		/// 请求医馆角色数据
		/// </summary>
		public static string GetHospitalPanelData;
		/// <summary>
		/// 请求医馆角色数据回调
		/// </summary>
		public static string GetHospitalPanelDataEcho;
		/// <summary>
		/// 治疗侠客
		/// </summary>
		public static string CureRole;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void RoleNotifyInit() {

			Messenger.AddListener<bool>(NotifyTypes.CallRoleInfoPanelData, (isfighting) => {
				DbManager.Instance.CallRoleInfoPanelData(isfighting);
			});

			Messenger.AddListener<JObject, bool>(NotifyTypes.CallRoleInfoPanelDataEcho, (obj, isfighting) => {
				RoleInfoPanelCtrl.Show((JArray)obj["data"], isfighting);
				if (!isfighting) {
					Messenger.Broadcast(NotifyTypes.ShowTaskBtnPanel);
				}
			});

			Messenger.AddListener(NotifyTypes.HideRoleInfoPanel, () => {
				RoleInfoPanelCtrl.Hide();
				Messenger.Broadcast(NotifyTypes.HideTaskBtnPanel);
			});

			Messenger.AddListener<bool>(NotifyTypes.MakeChangeRoleEnable, (enable) => {
				RoleInfoPanelCtrl.MakeChangeRoleEnable(enable);
			});

			Messenger.AddListener<bool>(NotifyTypes.MakeChangeBookEnable, (enable) => {
				RoleInfoPanelCtrl.MakeChangeBookEnable(enable);
			});

			Messenger.AddListener<bool>(NotifyTypes.MakeRoleInfoPanelDisable, (dis) => {
				RoleInfoPanelCtrl.MakeDisable(dis);
			});

			Messenger.AddListener<string>(NotifyTypes.GetRolesOfWinShopPanelData, (cityId) => {
				DbManager.Instance.GetRolesOfWinShopPanelData(cityId);
			});

			Messenger.AddListener<List<RoleData>>(NotifyTypes.GetRolesOfWinShopPanelDataEcho, (roles) => {
				RolesOfWinShopPanelCtrl.Show(roles);
			});

			Messenger.AddListener(NotifyTypes.GetWeaponsListPanelData, () => {
				DbManager.Instance.GetWeaponsListPanelData();
			});

			Messenger.AddListener<List<WeaponData>>(NotifyTypes.GetWeaponsListPanelDataEcho, (weapons) => {
				WeaponListPanelCtrl.Show(weapons);
			});

			Messenger.AddListener<int, string>(NotifyTypes.ReplaceWeapon, (id, beUsingByRoleId) => {
				DbManager.Instance.ReplaceWeapon(id, beUsingByRoleId);
			});

			Messenger.AddListener(NotifyTypes.GetBooksListPanelData, () => {
				DbManager.Instance.GetBooksListPanelData();
			});

			Messenger.AddListener<List<BookData>>(NotifyTypes.GetBooksListPanelDataEcho, (books) => {
				BookListPanelCtrl.Show(books);
			});

			Messenger.AddListener<int>(NotifyTypes.UseBook, (id => {
				DbManager.Instance.UseBook(id);
			}));

			Messenger.AddListener<int>(NotifyTypes.UnuseBook, (id => {
				DbManager.Instance.UnuseBook(id);
			}));

			Messenger.AddListener<string>(NotifyTypes.GetBooksOfForbiddenAreaPanelData, (cityId) => {
				DbManager.Instance.GetBooksOfForbiddenAreaPanelData(cityId);
			});

			Messenger.AddListener<List<BookData>>(NotifyTypes.GetBooksOfForbiddenAreaPanelDataEcho, (books) => {
				BooksOfForbiddenAreaPanelCtrl.Show(books);
			});

			Messenger.AddListener<int>(NotifyTypes.InviteRole, (id) => {
				DbManager.Instance.InviteRole(id);
			});

			Messenger.AddListener<int>(NotifyTypes.ReadBook, (id => {
				DbManager.Instance.ReadBook(id);
			}));

			Messenger.AddListener(NotifyTypes.GetReadyToTravelPanelData, () => {
				Messenger.Broadcast(NotifyTypes.ModifyResources);
				DbManager.Instance.GetReadyToTravelPanelData();
			});

			Messenger.AddListener<List<RoleData>, ItemData>(NotifyTypes.GetReadyToTravelPanelDataEcho, (roles, food) => {
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				ReadyToTravelPanelCtrl.Show(roles, food);
			});

			Messenger.AddListener<RoleData>(NotifyTypes.MakeSelectRoleInTeam, (role) => {
				ReadyToTravelPanelCtrl.MakeSelectRole(role);
			});

			Messenger.AddListener<RoleData>(NotifyTypes.MakeUnSelectRoleInTeam, (role) => {
				ReadyToTravelPanelCtrl.MakeUnSelectRole(role);
			});

			Messenger.AddListener<JArray>(NotifyTypes.ChangeRolesSeatNo, (ids) => {
				DbManager.Instance.ChangeRolesSeatNo(ids);
			});

			Messenger.AddListener(NotifyTypes.ChangeRolesSeatNoEcho, () => {
				Messenger.Broadcast(NotifyTypes.HideCityScenePanel);
				Messenger.Broadcast(NotifyTypes.FromCitySceneBackToArea);
				Messenger.Broadcast(NotifyTypes.CallAreaMainPanelData);
				ReadyToTravelPanelCtrl.Hide();
			});

			Messenger.AddListener(NotifyTypes.GetHospitalPanelData, () => {
				DbManager.Instance.GetHospitalPanelData();
			});

			Messenger.AddListener<List<RoleData>>(NotifyTypes.GetHospitalPanelDataEcho, (roles) => {
				HospitalPanelCtrl.Show(roles);
			});

			Messenger.AddListener<int>(NotifyTypes.CureRole, (id => {
				DbManager.Instance.CureRole(id);
			}));
		}
	}
}
