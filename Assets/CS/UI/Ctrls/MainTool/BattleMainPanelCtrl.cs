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
		GotSkills teamGotSkills;

		//敌方
		Image enemyBody;
		Text enemyName;
		Image enemyWeaponPowerBg;
		Image enemyWeaponRunLine;
		Image enemyWeaponShowBg;
		Image enemyCurSkillIconImage;
		WeaponPowerPlus enemyWeaponPowerPlus;
		RectTransform enemyWeaponRunLineRect;
		GotSkills enemyGotSkills;

		RoleData currentTeamRole;
		BookData currentTeamBook;
		SkillData currentTeamSkill;

		FightData fightData;
		List<RoleData> enemyRoleDatas;
		RoleData currentEnemyRole;
		BookData currentEnemyBook;
		SkillData currentEnemySkill;

		bool playing;
		float teamLineX;
		float enemyLineX;
		bool canTeamDoSkill;
		bool canEnemyDoSkill;

		protected override void Init () {
			doSkillButton = GetChildButton("doSkillButton");
			EventTriggerListener.Get(doSkillButton.gameObject).onClick += onClick;
			battleProgressText = GetChildText("battleProgressText");

			canvasGroup = GetComponent<CanvasGroup>();

			teamWeaponPowerBg = GetChildImage("teamWeaponPowerBg");
			teamWeaponRunLine = GetChildImage("teamWeaponRunLine");
			teamWeaponShowBg = GetChildImage("teamWeaponShowBg");
			teamCurSkillIconImage = GetChildImage("teamCurSkillIconImage");
			teamWeaponPowerPlus = GetChild("teamWeaponShowLittleBg").GetComponent<WeaponPowerPlus>();
			teamWeaponRunLineRect = GetChild("teamWeaponRunLine").GetComponent<RectTransform>();
			teamGotSkills = GetChild("teamGotSkills").GetComponent<GotSkills>();

			enemyBody = GetChildImage("enemyBody");
			enemyName = GetChildText("enemyName");
			enemyWeaponPowerBg = GetChildImage("enemyWeaponPowerBg");
			enemyWeaponRunLine = GetChildImage("enemyWeaponRunLine");
			enemyWeaponShowBg = GetChildImage("enemyWeaponShowBg");
			enemyCurSkillIconImage = GetChildImage("enemyCurSkillIconImage");
			enemyWeaponPowerPlus = GetChild("enemyWeaponShowLittleBg").GetComponent<WeaponPowerPlus>();
			enemyWeaponRunLineRect = GetChild("enemyWeaponRunLine").GetComponent<RectTransform>();
			enemyGotSkills = GetChild("enemyGotSkills").GetComponent<GotSkills>();

			canTeamDoSkill = true;
			canEnemyDoSkill = true;
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "doSkillButton":
				if (canTeamDoSkill) {
					canTeamDoSkill = false;
					if (currentTeamBook != null) {
						float powerMultiplying = teamWeaponPowerPlus.GetPowerMultiplyingByCollision(teamWeaponRunLineRect);
						doSkill("Team", powerMultiplying);
					}
				}
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="powerMult">Power mult.</param>
		void doSkill(string teamName, float powerMult) {
			if (teamName == "Team") {
				if (powerMult > 0) {
					currentTeamSkill = currentTeamBook.NextSkill();
					if (currentTeamBook.CurrentSkillIndex == 0) {
						teamGotSkills.Clear();
					}
					else {
						teamGotSkills.Pop(currentTeamBook.CurrentSkillIndex - 1);
					}
				}
				else {
					teamGotSkills.Clear();
					currentTeamSkill = currentTeamBook.Restart();
				}
				battleProgressText.text = currentTeamSkill.Name;
			}
			else {
				if (powerMult > 0) {
					currentEnemySkill = currentEnemyBook.NextSkill();
					if (currentEnemyBook.CurrentSkillIndex == 0) {
						enemyGotSkills.Clear();
					}
					else {
						enemyGotSkills.Pop(currentEnemyBook.CurrentSkillIndex - 1);
					}
				}
				else {
					enemyGotSkills.Clear();
					currentEnemySkill = currentEnemyBook.Restart();
				}
			}
		}


		/// <summary>
		/// 敌人自动战斗
		/// </summary>
		void enemyAuto() {
			
		}

		void Update() {
			if (!playing) {
				return;
			}
			if (currentTeamRole != null) {
				teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
				teamLineX += currentTeamRole.AttackSpeed * 5;
				if (teamLineX >= 292) {
					canTeamDoSkill = true;
					teamLineX = -292 + (teamLineX - 292);
				}
			}
			if (currentEnemyRole != null) {
				enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);
				enemyLineX += currentEnemyRole.AttackSpeed;
				if (enemyLineX >= 292) {
					canEnemyDoSkill = true;
					enemyLineX = -292 + (enemyLineX - 292);
				}
			}
			enemyAuto();
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
			teamLineX = -292;
			enemyLineX = -292;
			UpdateCurrentTeamRole(currentRole);
			fightData = fight;
			enemyRoleDatas = fightData.Enemys;
			callEnemey();
		}

		void callEnemey() {
			currentEnemyRole = popEnemy();
			if (currentEnemyRole != null) {
				currentEnemyBook = currentEnemyRole.Books.Count > 0 ? currentEnemyRole.Books[0] : null;
				enemyGotSkills.Clear();
				enemyGotSkills.SetIconIds(new List<string>() { "300000", "300000", "300000" });
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
			canvasGroup.alpha = 0;
			Pause();
			canvasGroup.DOFade(1, 2).SetAutoKill(false).OnComplete(() => {
				Play();
			});
			teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
			enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);
			RefreshTeamView();
			refreshEnemyView();
		}

		public void UpdateCurrentTeamRole(RoleData currentRole) {
			if (currentRole != null) {
				currentTeamRole = currentRole;
				Debug.LogWarning("换人, " + currentTeamRole.Name);
				UpdateCurrentTeamBookIndex(0);
			}
		}

		public void UpdateCurrentTeamBookIndex(int index) {
			if (currentTeamRole != null && currentTeamRole.Books.Count > index) {
				currentTeamRole.SelectBook(index);
				currentTeamBook = currentTeamRole.GetCurrentBook();
				currentTeamSkill = currentTeamBook.GetCurrentSkill();

				teamGotSkills.Clear();
				teamGotSkills.SetIconIds(new List<string>() { "300000", "300000", "300000", "300000" });

				Debug.LogWarning("切书, " + currentTeamBook.Name);
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
