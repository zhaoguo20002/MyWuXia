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
				enemy0.Books = new List<BookData>();
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
