using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game {
	public class BattleMainPanelCtrl : WindowCore<BattleMainPanelCtrl, JArray> {
		
		CanvasGroup canvasGroup;
		//己方
		Image teamWeaponPowerBg;
		Image teamWeaponRunLine;
		Image teamWeaponShowBg;
		Image teamCurSkillIconImage;

		//敌方
		Image enemyBody;
		Text enemyName;
		Image enemyWeaponPowerBg;
		Image enemyWeaponRunLine;
		Image enemyWeaponShowBg;
		Image enemyCurSkillIconImage;

		RoleData currentTeamRole;
		BookData currentTeamBook;

		FightData fightData;
		List<RoleData> enemyRoleDatas;
		RoleData currentEnemyRole;
		BookData currentEnemyBook;
		
		protected override void Init () {
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;
			canvasGroup.DOFade(1, 2).SetAutoKill(false);

			teamWeaponPowerBg = GetChildImage("teamWeaponPowerBg");
			teamWeaponRunLine = GetChildImage("teamWeaponRunLine");
			teamWeaponShowBg = GetChildImage("teamWeaponShowBg");
			teamCurSkillIconImage = GetChildImage("teamCurSkillIconImage");

			enemyBody = GetChildImage("enemyBody");
			enemyName = GetChildText("enemyName");
			enemyWeaponPowerBg = GetChildImage("enemyWeaponPowerBg");
			enemyWeaponRunLine = GetChildImage("enemyWeaponRunLine");
			enemyWeaponShowBg = GetChildImage("enemyWeaponShowBg");
			enemyCurSkillIconImage = GetChildImage("enemyCurSkillIconImage");

		}

		public void UpdateData(RoleData currentRole, FightData fight) {
			UpdateCurrentTeamRole(currentRole);
			fightData = fight;
			enemyRoleDatas = fightData.Enemys;
			callEnemey();
		}

		void callEnemey() {
			currentEnemyRole = popEnemy();
			if (currentEnemyRole != null) {
				currentEnemyBook = currentEnemyRole.Books.Count > 0 ? currentEnemyRole.Books[0] : null;
			} 
		}

		RoleData popEnemy() {
			if (enemyRoleDatas.Count > 0) {
				RoleData enemy = enemyRoleDatas[0];
				enemyRoleDatas.RemoveAt(0);
				return enemy;
			}
			return null;
		}

		void refreshEnemyView() {
			if (currentEnemyRole != null) {
				enemyBody.sprite = Statics.GetHalfBodySprite(currentEnemyRole.HalfBodyId);
				enemyName.text = currentEnemyRole.Name;
			}
		}

		public override void RefreshView () {
			RefreshTeamView();
			refreshEnemyView();
		}

		public void UpdateCurrentTeamRole(RoleData currentRole) {
			if (currentRole != null) {
				currentTeamRole = currentRole;
				UpdateCurrentTeamBookIndex(currentTeamRole.SelectedBookIndex);
			}
		}

		public void UpdateCurrentTeamBookIndex(int index) {
			if (currentTeamRole != null && currentTeamRole.Books.Count > index) {
				currentTeamBook = currentTeamRole.Books[index];
			}
		}

		public void RefreshTeamView() {
			
		}

		public static void Show(RoleData currentRole, FightData fight) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/BattleMainPanelView", "BattleMainPanelCtrl");
			}
			Ctrl.UpdateData(currentRole, fight);
			Ctrl.RefreshView();
		}

		public static void ChangeCurrentTeamRole(RoleData currentRole) {
			if (Ctrl != null) {
				Ctrl.UpdateCurrentTeamRole(currentRole);
				Ctrl.RefreshTeamView();
			}
		}

		public static void ChangeCurrentTeamBook(int index) {
			if (Ctrl != null) {
				Ctrl.UpdateCurrentTeamBookIndex(index);
				Ctrl.RefreshTeamView();
			}
		}
	}
}
