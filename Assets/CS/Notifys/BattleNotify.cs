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
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Battles the notify init.
		/// </summary>
		public static void BattleNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.CreateBattle, (fightId) => {
				//获取队伍角色列表
				RoleData currentRoleData = RoleInfoPanelCtrl.GetCurrentRoleData();
				if (currentRoleData == null) {
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
				FightData fightData = JsonManager.GetInstance().GetMapping<FightData>("Fights", fightId);
				fightData.MakeJsonToModel();
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				Messenger.Broadcast<System.Action, System.Action>(NotifyTypes.PlayCameraVortex, () => {
					BattleMainPanelCtrl.Show(currentRoleData, fightData);
				}, () => {
					Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, true);
				});
			});

			Messenger.AddListener<RoleData, string>(NotifyTypes.CreateTestBattle, (currentRoleData, fightId) => {
				FightData fightData = JsonManager.GetInstance().GetMapping<FightData>("Fights", fightId);
				fightData.MakeJsonToModel();
				BattleMainPanelCtrl.Show(currentRoleData, fightData);
			});

			Messenger.AddListener<bool, List<DropData>>(NotifyTypes.EndBattle, (win, drops) => {
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				Messenger.Broadcast<System.Action, System.Action>(NotifyTypes.PlayCameraVortex, () => {
					BattleMainPanelCtrl.Hide();
				}, () => {
					Messenger.Broadcast<bool>(NotifyTypes.CallRoleInfoPanelData, false);
					Messenger.Broadcast(NotifyTypes.PlayBgm);
				});
			});

			Messenger.AddListener<RoleData>(NotifyTypes.ChangeCurrentTeamRoleInBattle, (roleData) => {
				BattleMainPanelCtrl.ChangeCurrentTeamRole(roleData);
			});

			Messenger.AddListener<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, (index) => {
				BattleMainPanelCtrl.ChangeCurrentTeamBook(index);
			});

			Messenger.AddListener<bool, string, int>(NotifyTypes.SendFightResult, (win, fightId, star) => {
				DbManager.Instance.SendFightResult(win, fightId, star);
			});

			Messenger.AddListener<bool, List<DropData>>(NotifyTypes.SendFightResultEcho, (win, drops) => {
				Messenger.Broadcast<bool, List<DropData>>(NotifyTypes.EndBattle, win, drops);
			});
		}
	}
}
