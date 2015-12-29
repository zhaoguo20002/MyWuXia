using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game {
	public class BattleMainPanelCtrl : WindowCore<BattleMainPanelCtrl, JArray> {
		
		CanvasGroup canvasGroup;
		Button doSkillButton;
		Text battleProgressText;

		//己方
		Image teamWeaponPowerBg;
		Image teamWeaponRunLine;
		Image teamWeaponShowBg;
		Image teamCurSkillIconImage;
		WeaponPowerPlus teamWeaponPowerPlus;
		RectTransform teamWeaponRunLineRect;

		//敌方
		Image enemyBody;
		Text enemyName;
		Image enemyWeaponPowerBg;
		Image enemyWeaponRunLine;
		Image enemyWeaponShowBg;
		Image enemyCurSkillIconImage;
		WeaponPowerPlus enemyWeaponPowerPlus;
		RectTransform enemyWeaponRunLineRect;

		RoleData currentTeamRole;
		BookData currentTeamBook;

		FightData fightData;
		List<RoleData> enemyRoleDatas;
		RoleData currentEnemyRole;
		BookData currentEnemyBook;

		bool playing;
		float teamLineX;
		float enemyLineX;

		float rayCastDisntance = 2000;
		Ray ray;
		RaycastHit hit;
		LayerMask layerMaskDefault;

		protected override void Init () {
			layerMaskDefault = 1 << LayerMask.NameToLayer("UI");

			doSkillButton = GetChildButton("doSkillButton");
			EventTriggerListener.Get(doSkillButton.gameObject).onClick += onClick;
			battleProgressText = GetChildText("battleProgressText");

			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;
			Pause();
			canvasGroup.DOFade(1, 2).SetAutoKill(false).OnComplete(() => {
				Play();
			});

			teamWeaponPowerBg = GetChildImage("teamWeaponPowerBg");
			teamWeaponRunLine = GetChildImage("teamWeaponRunLine");
			teamWeaponShowBg = GetChildImage("teamWeaponShowBg");
			teamCurSkillIconImage = GetChildImage("teamCurSkillIconImage");
			teamWeaponPowerPlus = GetChild("teamWeaponShowLittleBg").GetComponent<WeaponPowerPlus>();
			teamWeaponRunLineRect = GetChild("teamWeaponRunLine").GetComponent<RectTransform>();

			enemyBody = GetChildImage("enemyBody");
			enemyName = GetChildText("enemyName");
			enemyWeaponPowerBg = GetChildImage("enemyWeaponPowerBg");
			enemyWeaponRunLine = GetChildImage("enemyWeaponRunLine");
			enemyWeaponShowBg = GetChildImage("enemyWeaponShowBg");
			enemyCurSkillIconImage = GetChildImage("enemyCurSkillIconImage");
			enemyWeaponPowerPlus = GetChild("enemyWeaponShowLittleBg").GetComponent<WeaponPowerPlus>();
			enemyWeaponRunLineRect = GetChild("enemyWeaponRunLine").GetComponent<RectTransform>();

		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "doSkillButton":
				battleProgressText.text = "攻击倍率 = " + teamWeaponPowerPlus.GetPowerMultiplyingByCollision(teamWeaponRunLineRect);
				break;
			default:
				break;
			}
		}
//		bool isMouseDown;
		void Update() {
			if (!playing) {
				return;
			}
			if (currentTeamRole != null) {
				teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
				teamLineX += currentTeamRole.AttackSpeed * 4;
				teamLineX = teamLineX >= 292 ? -292 : teamLineX;
			}
			if (currentEnemyRole != null) {
				enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);
				enemyLineX += currentEnemyRole.AttackSpeed;
				enemyLineX = enemyLineX >= 292 ? -292 : enemyLineX;
			}
//			isMouseDown = false;
//			if(Input.GetMouseButtonDown(0)){
//				isMouseDown = true;
//			}
//			else if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) {
//				isMouseDown = true;
//			}
//			if (isMouseDown) {
//				battleProgressText.text = "攻击倍率 = " + teamWeaponPowerPlus.GetPowerMultiplyingByCollision(teamWeaponRunLineRect);
//				Debug.LogWarning(isMouseDown);
//			}
		}

		public void Play() {
			playing = true;
			teamLineX = -292;
			enemyLineX = -292;
		}

		public void Pause() {
			playing = false;
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
			if (currentTeamRole != null) {
				teamWeaponPowerPlus.SetRates(new float[] { 1, 0.8f, 0.3f, 0.1f });
			}
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
