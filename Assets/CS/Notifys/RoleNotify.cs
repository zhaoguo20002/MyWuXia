using UnityEngine;
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
		/// 卸下兵器
		/// </summary>
		public static string TakeOffWeapon;
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
		/// <summary>
		/// 请求行囊界面
		/// </summary>
		public static string GetBagPanelData;
		/// <summary>
		/// 请求行囊界面回调
		/// </summary>
		public static string GetBagPanelDataEcho;
		/// <summary>
		/// 打开查看物品信息界面
		/// </summary>
		public static string ShowItemDetailPanel;
		/// <summary>
		/// 请求出售物品界面
		/// </summary>
		public static string GetSellItemsPanelData;
		/// <summary>
		/// 请求出售物品界面回调
		/// </summary>
		public static string GetSellItemsPanelDataEcho;
		/// <summary>
		/// 选中/取消选中待出售物品时的更新通知消息
		/// </summary>
		public static string MakeSelectedItemOfSellItemsPanel;
		/// <summary>
		/// 批量出售物品
		/// </summary>
		public static string SellItems;
		/// <summary>
		/// 批量出售物品回调
		/// </summary>
		public static string SellItemsEcho;
		/// <summary>
		/// 丢弃物品
		/// </summary>
		public static string DiscardItem;
		/// <summary>
		/// 使用物品
		/// </summary>
		public static string UseItem;
		/// <summary>
		/// 打开秘籍详情界面
		/// </summary>
		public static string ShowBookDetailPanel;
		/// <summary>
		/// 查看武器详情
		/// </summary>
		public static string ShowWeaponDetailPanel;
		/// <summary>
		/// 查看角色详情
		/// </summary>
		public static string ShowRoleDetailPanel;
		/// <summary>
		/// 打开掉落物显示面板
		/// </summary>
		public static string ShowDropsListPanel;
		/// <summary>
		/// 获取城镇中驿站的传送点数据
		/// </summary>
		public static string GetInnInCityData;
		/// <summary>
		/// 获取城镇中驿站的传送点数据回调
		/// </summary>
		public static string GetInnInCityDataEcho;
		/// <summary>
		/// 传送到城镇
		/// </summary>
		public static string GoToCity;
		/// <summary>
		/// 传送到城镇回调
		/// </summary>
		public static string GoToCityEcho;
        /// <summary>
        /// 特殊Npc事件集中处理
        /// </summary>
        public static string NpcsEventHandler;
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
				RoleInfoPanelCtrl.MoveDown();
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

			Messenger.AddListener<List<WeaponData>, RoleData>(NotifyTypes.GetWeaponsListPanelDataEcho, (weapons, host) => {
				WeaponListPanelCtrl.Show(weapons, host);
			});

			Messenger.AddListener<int, string>(NotifyTypes.ReplaceWeapon, (id, beUsingByRoleId) => {
				DbManager.Instance.ReplaceWeapon(id, beUsingByRoleId);
			});

			Messenger.AddListener<int>(NotifyTypes.TakeOffWeapon, (id => {
				DbManager.Instance.TakeOffWeapon(id);
			}));

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

			Messenger.AddListener<List<BookData>, RoleData>(NotifyTypes.GetBooksOfForbiddenAreaPanelDataEcho, (books, host) => {
				BooksOfForbiddenAreaPanelCtrl.Show(books, host);
			});

			Messenger.AddListener<int>(NotifyTypes.InviteRole, (id) => {
//                DbManager.Instance.InviteRole(id);
                DbManager.Instance.InviteRoleWithResources(id);
			});

			Messenger.AddListener<int>(NotifyTypes.ReadBook, (id => {
				DbManager.Instance.ReadBook(id);
			}));

			Messenger.AddListener(NotifyTypes.GetReadyToTravelPanelData, () => {
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

			Messenger.AddListener(NotifyTypes.GetBagPanelData, () => {
				DbManager.Instance.GetBagPanelData();
			});

			Messenger.AddListener<List<ItemData>, double>(NotifyTypes.GetBagPanelDataEcho, (items, silver) => {
				BagPanelCtrl.Show(items, silver);
			});

			Messenger.AddListener<ItemData, bool>(NotifyTypes.ShowItemDetailPanel, (item, fromBag) => {
				ItemDetailPanelCtrl.Show(item, fromBag);
			});

			Messenger.AddListener(NotifyTypes.GetSellItemsPanelData, () => {
				DbManager.Instance.GetSellItemsPanelData();
			});

			Messenger.AddListener<List<ItemData>>(NotifyTypes.GetSellItemsPanelDataEcho, (items) => {
				SellItemsPanelCtrl.Show(items);
			});

			Messenger.AddListener(NotifyTypes.MakeSelectedItemOfSellItemsPanel, () => {
				SellItemsPanelCtrl.MakeSelectedItem();
			});

			Messenger.AddListener<JArray>(NotifyTypes.SellItems, (ids) => {
				DbManager.Instance.SellItems(ids);
			});

			Messenger.AddListener<double>(NotifyTypes.SellItemsEcho, (silver) => {
				SellItemsPanelCtrl.Hide();
				StorePanelCtrl.MakeChangeSilverNum(silver);
			});

			Messenger.AddListener<int>(NotifyTypes.DiscardItem, (id) => {
				DbManager.Instance.DiscardItem(id);
			});

			Messenger.AddListener<BookData>(NotifyTypes.ShowBookDetailPanel, (book) => {
				BookDetailPanelCtrl.Show(book);
			});

			Messenger.AddListener<WeaponData>(NotifyTypes.ShowWeaponDetailPanel, (weapon) => {
				WeaponDetailPanelCtrl.Show(weapon);
			});

			Messenger.AddListener<RoleData>(NotifyTypes.ShowRoleDetailPanel, (role) => {
				RoleDetailPanelCtrl.Show(role);
			});

			Messenger.AddListener<List<DropData>>(NotifyTypes.ShowDropsListPanel, (drops) => {
				DropsListPanelCtrl.Show(drops);
			});

			Messenger.AddListener<int>(NotifyTypes.UseItem, (id => {
				DbManager.Instance.UseItem(id);
			}));

			Messenger.AddListener<string>(NotifyTypes.GetInnInCityData, (cityId) => {
				DbManager.Instance.GetInnInCityData(cityId);
			});

			Messenger.AddListener<List<FloydResult>>(NotifyTypes.GetInnInCityDataEcho, (results) => {
				InnPanelCtrl.Show(results);
			});

			Messenger.AddListener<int, int>(NotifyTypes.GoToCity, (fromIndex, toIndex) => {
				DbManager.Instance.GoToCity(fromIndex, toIndex);
			});

			Messenger.AddListener<SceneData>(NotifyTypes.GoToCityEcho, (scene) => {
				string eventId = JsonManager.GetInstance().GetMapping<string>("AreaCityPosDatas", scene.Id);
				string[] fen = eventId.Split(new char[] { '_' });
				if (fen.Length >= 3) {
					string areaName = fen[0];
					int x = int.Parse(fen[1]);
					int y = int.Parse(fen[2]);
					if (UserModel.CurrentUserData != null) {
						CityScenePanelCtrl.MakeClose();
						InnPanelCtrl.Hide();
						UserModel.CurrentUserData.PositionStatu = UserPositionStatusType.InCity;
						UserModel.CurrentUserData.CurrentCitySceneId = scene.Id;
						UserModel.CurrentUserData.CurrentAreaSceneName = areaName;
						UserModel.CurrentUserData.CurrentAreaX = x;
						UserModel.CurrentUserData.CurrentAreaY = y;
						//清空临时事件
						Messenger.Broadcast(NotifyTypes.ClearDisableEventIdMapping);
						Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.UpdateUserData, null);
						Messenger.Broadcast<string>(NotifyTypes.GoToScene, areaName);
					}
				}
			});

            Messenger.AddListener<string>(NotifyTypes.NpcsEventHandler, (npcId) => {
                switch (npcId) {
                    case "05002001":
                        if (DbManager.Instance.HostData.Occupation == OccupationType.None) {
                            if (!DbManager.Instance.HasAnyTask((new List<string>(){ 
                                "task_occupation0", 
                                "task_occupation1", 
                                "task_occupation2", 
                                "task_occupation3", 
                                "task_occupation4", 
                                "task_occupation5" 
                            }).ToArray())) {
                                OccupationPanelCtrl.Show();
                            }
                            else {
                                AlertCtrl.Show("去吧去吧,老夫已为你做了引荐");
                            }
                        }
                        else {
                            AlertCtrl.Show(string.Format("你已是{0}{1},可喜可贺啊,哦哈哈哈哈", Statics.GetOccupationName(DbManager.Instance.HostData.Occupation), Statics.GetOccupationDesc(DbManager.Instance.HostData.Occupation)));
                        }
                        break;
                    default:
                        break;
                }
            });
		}
	}
}
