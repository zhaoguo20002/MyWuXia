using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 常见战场(根据战斗Id获取相关静态数据表,生成战场数据)
		/// </summary>
		public static string CreateBattle;
		/// <summary>
		/// 创建一场测试战斗,让某一个角色触发一场战斗
		/// </summary>
		public static string CreateTestBattle;
		/// <summary>
		/// 战斗结束
		/// </summary>
		public static string EndBattle;
		/// <summary>
		/// 切换战斗角色
		/// </summary>
		public static string ChangeCurrentTeamRoleInBattle;
		/// <summary>
		/// 切换战斗秘籍
		/// </summary>
		public static string ChangeCurrentTeamBookInBattle;
		/// <summary>
		/// 发送战斗结果
		/// </summary>
		public static string SendFightResult;
		/// <summary>
		/// 发送战斗结果回调
		/// </summary>
		public static string SendFightResultEcho;
		/// <summary>
		/// 回城
		/// </summary>
		public static string BackToCity;
		/// <summary>
		/// 战斗失败消息
		/// </summary>
		public static string BattleFaild;
		/// <summary>
		/// 侠客战死后替换
		/// </summary>
		public static string MakePopRole;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Battles the notify init.
		/// </summary>
		public static void BattleNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.CreateBattle, (fightId) => {
                if (BattleFightPanelCtrl.Ctrl != null) {
                    Statics.CreatePopMsg(Vector3.zero, "已在战斗中", Color.white, 30);
                    return;
                }
				//获取队伍角色列表
//				RoleData currentRoleData = RoleInfoPanelCtrl.GetCurrentRoleData();
				RoleData currentRoleData = DbManager.Instance.GetHostRoleData();
				currentRoleData.MakeJsonToModel();
				if (currentRoleData == null) {
					return;
				}
				if (currentRoleData.Injury == InjuryType.Moribund) {
                    AlertCtrl.Show("你已奄奄一息无法再战!", () => {
                        Messenger.Broadcast(NotifyTypes.BackToCity);
                    });
					return;
				}
				//获取战斗数据
//				FightData fightData = new FightData();
//				fightData.Id = fightId;
//				fightData.Type = FightType.Normal;
//				RoleData enemy0 = new RoleData();
//				enemy0.Id = "enemy0";
//				enemy0.Name = "赏金刺客";
//				enemy0.HalfBodyId = "enemy000001";
//				BookData book0 = new BookData();
//				book0.Id = "book20001";
//				book0.Name = "地痞撒泼";
//				book0.IconId = "200000";
//				SkillData skill0 = new SkillData();
//				skill0.Type = SkillType.MagicAttack;
//				skill0.Name = "背负投";
//				BuffData buff0 = new BuffData();
//				buff0.Type = BuffType.Vertigo;
////				buff0.Value = 8888;
////				buff0.FirstEffect = true;
//				buff0.RoundNumber = 3;
//				buff0.Rate = 30;
//				buff0.FirstEffect = true;
//				skill0.DeBuffDatas.Add(buff0);
//				SkillData skill1 = new SkillData();
//				skill1.Type = SkillType.PhysicsAttack;
//				skill1.Name = "抱摔";
//				SkillData skill2 = new SkillData();
//				skill2.Type = SkillType.PhysicsAttack;
//				skill2.Name = "撕咬";
//				book0.Skills.Add(skill0);
//				book0.Skills.Add(skill1);
//				book0.Skills.Add(skill2);
//				enemy0.Books.Add(book0);
//				enemy0.AttackSpeed = 2;
//				enemy0.HP = 10000;
//				enemy0.MaxHP = 10000;
//				WeaponData weapon5 = new WeaponData();
//				weapon5.Id = "weapon5";
//				weapon5.Id = "阔刃刀";
//				weapon5.Width = 360;
//				weapon5.Rates = new float[] { 1, 0.6f, 0.2f, 0.1f };
//				enemy0.Weapon = weapon5;
//				fightData.Enemys = new List<RoleData>() {
//					enemy0
//				};
//                List<RoleData> teams = RoleInfoPanelCtrl.GetRoleDatas();
                List<RoleData> teams = DbManager.Instance.GetRolesInTeam();
                for (int i = 0, len = teams.Count; i < len; i++) {
                    teams[i].MakeJsonToModel();
                }
				FightData fightData = JsonManager.GetInstance().GetMapping<FightData>("Fights", fightId);
				fightData.MakeJsonToModel();
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
                SoundManager.GetInstance().PauseBGM();
                SoundManager.GetInstance().PushSound("ui0003");
				Messenger.Broadcast<System.Action, System.Action>(NotifyTypes.PlayCameraVortex, () => {
//					BattleMainPanelCtrl.Show(currentRoleData, fightData);
                    List<ItemData> drugs = new List<ItemData>();
//                    drugs.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "100001"));
//                    drugs.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "100002"));
//                    drugs.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "100003"));
                    List<ItemData> allVulnerary = DbManager.Instance.GetItems(ItemType.Drug);
                    for (int i = 0; i < 3; i++) {
                        if (allVulnerary.Count > i) {
                            drugs.Add(allVulnerary[i]);
                        }
                        else {
                            break;
                        }
                    }
                    BattleFightPanelCtrl.Show(fightData, teams, fightData.Enemys, drugs);
				}, () => {
//					Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, true);
				});
			});

			Messenger.AddListener<List<RoleData>, string>(NotifyTypes.CreateTestBattle, (roles, fightId) => {
				FightData fightData = JsonManager.GetInstance().GetMapping<FightData>("Fights", fightId);
				fightData.MakeJsonToModel();
//				BattleMainPanelCtrl.Show(currentRoleData, fightData);
                List<ItemData> drugs = new List<ItemData>();
                drugs.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "100001"));
                drugs.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "100002"));
                drugs.Add(JsonManager.GetInstance().GetMapping<ItemData>("ItemDatas", "100003"));
                BattleFightPanelCtrl.Show(fightData, roles, fightData.Enemys, drugs);
			});

			Messenger.AddListener<bool, List<DropData>, FightData>(NotifyTypes.EndBattle, (win, drops, fightData) => {
                if (!win) {
                    MaiHandler.SendEvent("FightFail", DbManager.Instance.HostData.Lv.ToString(), fightData.Id);
                }
//				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				Messenger.Broadcast<System.Action, System.Action>(NotifyTypes.PlayCameraVortex, () => {
					//如果普通战斗失败则回之前到过的城镇去疗伤
					if (fightData.Type == FightType.Normal && !win) {
						AlertCtrl.Show("江湖凶险, 稍事休息后再出发!", () => {
							Messenger.Broadcast(NotifyTypes.BackToCity);
						});
					}
//					BattleMainPanelCtrl.Hide();
                    BattleFightPanelCtrl.Hide();
				}, () => {
					//任务详情界面打开时不呼出角色信息板
					if (TaskDetailInfoPanelCtrl.Ctrl == null) {
						Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
					}
					Messenger.Broadcast(NotifyTypes.PlayBgm);
					if (drops.Count > 0) {
						Messenger.Broadcast<List<DropData>>(NotifyTypes.ShowDropsListPanel, drops);
					}
					if (fightData.Type == FightType.Task) {
						Messenger.Broadcast(NotifyTypes.ReloadTaslDetailInfoData);
						if (win) {
							Messenger.Broadcast<string>(NotifyTypes.MakeFightWinedBtnDisable, fightData.Id);
						}
					}
				});
			});

			Messenger.AddListener<RoleData>(NotifyTypes.ChangeCurrentTeamRoleInBattle, (roleData) => {
				BattleMainPanelCtrl.ChangeCurrentTeamRole(roleData);
			});

			Messenger.AddListener<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, (index) => {
				BattleMainPanelCtrl.ChangeCurrentTeamBook(index);
			});

			Messenger.AddListener<JArray>(NotifyTypes.SendFightResult, (data) => {
				DbManager.Instance.SendFightResult((bool)data[0], data[1].ToString(), (int)data[2]);
				JArray usedSkillIdData = (JArray)data[3];
				JArray d;
				for (int i = 0; i < usedSkillIdData.Count; i++) {
					d = (JArray)usedSkillIdData[i];
					DbManager.Instance.UpdateUsedTheSkillRecords(d[0].ToString(), (int)d[1]);
				}
				JArray plusIndexData = (JArray)data[4];
				for (int i = 0; i < plusIndexData.Count; i++) {
					d = (JArray)plusIndexData[i];
					DbManager.Instance.UpdateWeaponPowerPlusSuccessedRecords((int)d[0], (int)d[1]);
				}
			});

			Messenger.AddListener<bool, List<DropData>, FightData>(NotifyTypes.SendFightResultEcho, (win, drops, fightData) => {
				//加载动态事件列表
				Messenger.Broadcast<string>(NotifyTypes.GetActiveEventsInArea, UserModel.CurrentUserData.CurrentAreaSceneName);
				Messenger.Broadcast<bool, List<DropData>, FightData>(NotifyTypes.EndBattle, win, drops, fightData);
			});

			Messenger.AddListener(NotifyTypes.BackToCity, () => {
				string eventId = JsonManager.GetInstance().GetMapping<string>("AreaCityPosDatas", UserModel.CurrentUserData.CurrentCitySceneId);
				string[] fen = eventId.Split(new char[] { '_' });
				if (fen.Length >= 3) {
					string areaName = fen[0];
					int x = int.Parse(fen[1]);
					int y = int.Parse(fen[2]);
					if (UserModel.CurrentUserData != null) {
						UserModel.CurrentUserData.PositionStatu = UserPositionStatusType.InCity;
						UserModel.CurrentUserData.CurrentAreaSceneName = areaName;
						UserModel.CurrentUserData.CurrentAreaX = x;
						UserModel.CurrentUserData.CurrentAreaY = y;
						Messenger.Broadcast<System.Action<UserData>>(NotifyTypes.UpdateUserData, null);
						Messenger.Broadcast<string>(NotifyTypes.GoToScene, areaName);
					}
				}
			});

			Messenger.AddListener(NotifyTypes.BattleFaild, () => {
				BattleMainPanelCtrl.MakeFaild();
			});

			Messenger.AddListener<string>(NotifyTypes.MakePopRole, (dieRoleId) => {
				RoleInfoPanelCtrl.MakePopRole(dieRoleId);
			});
		}
	}
}
