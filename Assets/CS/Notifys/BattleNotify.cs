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
		/// 切换战斗角色
		/// </summary>
		public static string ChangeCurrentTeamRoleInBattle;
		/// <summary>
		/// 切换战斗秘籍
		/// </summary>
		public static string ChangeCurrentTeamBookInBattle;
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
				FightData fightData = new FightData();
				fightData.Id = fightId;
				fightData.Type = FightType.Normal;
				RoleData enemy0 = new RoleData();
				enemy0.Id = "enemy0";
				enemy0.Name = "赏金刺客";
				enemy0.HalfBodyId = "enemy000001";
				BookData book0 = new BookData();
				book0.Id = "book20001";
				book0.Name = "地痞撒泼";
				book0.IconId = "200000";
				SkillData skill0 = new SkillData();
				skill0.Type = SkillType.PhysicsAttack;
				skill0.Name = "背负投";
				SkillData skill1 = new SkillData();
				skill1.Type = SkillType.PhysicsAttack;
				skill1.Name = "抱摔";
				SkillData skill2 = new SkillData();
				skill2.Type = SkillType.PhysicsAttack;
				skill2.Name = "撕咬";
				book0.Skills.Add(skill0);
				book0.Skills.Add(skill1);
				book0.Skills.Add(skill2);
				enemy0.Books.Add(book0);
				enemy0.AttackSpeed = 15;
				fightData.Enemys = new List<RoleData>() {
					enemy0
				};

				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
				Messenger.Broadcast<System.Action, System.Action>(NotifyTypes.PlayCameraVortex, () => {
					BattleMainPanelCtrl.Show(currentRoleData, fightData);
				}, () => {
					Messenger.Broadcast(NotifyTypes.CallRoleInfoPanelData);
				});
			});

			Messenger.AddListener<RoleData>(NotifyTypes.ChangeCurrentTeamRoleInBattle, (roleData) => {
				BattleMainPanelCtrl.ChangeCurrentTeamRole(roleData);
			});

			Messenger.AddListener<int>(NotifyTypes.ChangeCurrentTeamBookInBattle, (index) => {
				BattleMainPanelCtrl.ChangeCurrentTeamBook(index);
			});
		}
	}
}
