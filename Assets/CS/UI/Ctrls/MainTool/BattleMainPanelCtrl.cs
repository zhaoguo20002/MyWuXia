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
		RectTransform teamWeaponPowerBg;
		Image teamWeaponRunLine;
		RectTransform teamWeaponShowBg;
		RectTransform teamWeaponShowLittleBg;
		RectTransform teamCurSkillIconImageRectTrans;
		Image teamCurSkillIconImage;
		WeaponPowerPlus teamWeaponPowerPlus;
		RectTransform teamWeaponRunLineRect;
		GotSkills teamGotSkills;
		SkillNameShow teamSkillNameShow;
		RectTransform teamBloodBgRectTrans;
		Image teamBloodProgress;

		//敌方
		Image enemyBody;
		Text enemyName;
		RectTransform enemyWeaponPowerBg;
		Image enemyWeaponRunLine;
		RectTransform enemyWeaponShowBg;
		RectTransform enemyWeaponShowLittleBg;
		RectTransform enemyCurSkillIconImageRectTrans;
		Image enemyCurSkillIconImage;
		WeaponPowerPlus enemyWeaponPowerPlus;
		RectTransform enemyWeaponRunLineRect;
		GotSkills enemyGotSkills;
		SkillNameShow enemySkillNameShow;
		RectTransform enemyBloodBgRectTrans;
		Image enemyBloodProgress;

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

		float holdOnTimeout = 0.3f;
		float teamSkillHoldOnDate;
		float enemySkillHoldOnDate;

		bool teamAutoFight;
		float teamAutoFightTimeout;
		float teamAutoFightDate;
		bool enemyAutoFight;
		float enemyAutoFightTimeout;
		float enemyAutoFightDate;

		protected override void Init () {
			doSkillButton = GetChildButton("doSkillButton");
			EventTriggerListener.Get(doSkillButton.gameObject).onClick += onClick;
			battleProgressText = GetChildText("battleProgressText");

			canvasGroup = GetComponent<CanvasGroup>();

			teamWeaponPowerBg = GetChild("teamWeaponPowerBg").GetComponent<RectTransform>();
			teamWeaponRunLine = GetChildImage("teamWeaponRunLine");
			teamWeaponShowBg = GetChild("teamWeaponShowBg").GetComponent<RectTransform>();
			teamCurSkillIconImage = GetChildImage("teamCurSkillIconImage");
			teamCurSkillIconImageRectTrans = GetChild("teamCurSkillIconImage").GetComponent<RectTransform>();
			teamWeaponShowLittleBg = GetChild("teamWeaponShowLittleBg").GetComponent<RectTransform>();
			teamWeaponPowerPlus = GetChild("teamWeaponShowLittleBg").GetComponent<WeaponPowerPlus>();
			teamWeaponRunLineRect = GetChild("teamWeaponRunLine").GetComponent<RectTransform>();
			teamGotSkills = GetChild("teamGotSkills").GetComponent<GotSkills>();
			teamSkillNameShow = GetChild("teamSkillNameShowBg").GetComponent<SkillNameShow>();
			teamBloodBgRectTrans = GetChild("teamBloodBg").GetComponent<RectTransform>();
			teamBloodProgress = GetChildImage("teamBloodProgress");

			enemyBody = GetChildImage("enemyBody");
			enemyName = GetChildText("enemyName");
			enemyWeaponPowerBg = GetChild("enemyWeaponPowerBg").GetComponent<RectTransform>();
			enemyWeaponRunLine = GetChildImage("enemyWeaponRunLine");
			enemyWeaponShowBg = GetChild("enemyWeaponShowBg").GetComponent<RectTransform>();
			enemyCurSkillIconImage = GetChildImage("enemyCurSkillIconImage");
			enemyCurSkillIconImageRectTrans = GetChild("enemyCurSkillIconImage").GetComponent<RectTransform>();
			enemyWeaponShowLittleBg = GetChild("enemyWeaponShowLittleBg").GetComponent<RectTransform>();
			enemyWeaponPowerPlus = GetChild("enemyWeaponShowLittleBg").GetComponent<WeaponPowerPlus>();
			enemyWeaponRunLineRect = GetChild("enemyWeaponRunLine").GetComponent<RectTransform>();
			enemyGotSkills = GetChild("enemyGotSkills").GetComponent<GotSkills>();
			enemySkillNameShow = GetChild("enemySkillNameShowBg").GetComponent<SkillNameShow>();
			enemyBloodBgRectTrans = GetChild("enemyBloodBg").GetComponent<RectTransform>();
			enemyBloodProgress = GetChildImage("enemyBloodProgress");
		}

		void onClick(GameObject e) {
			switch (e.name) {
			case "doSkillButton":
				doSkill("Team");
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// 处理招式伤害,附加招式以及buff和debuff
		/// </summary>
		/// <returns>The hurt HP and buffs.</returns>
		/// <param name="skill">Skill.</param>
		/// <param name="fromRole">From role.</param>
		/// <param name="toRole">To role.</param>
		int dealHurtHPAndBuffs(SkillData skill, RoleData fromRole, RoleData toRole) {
			int hurt = 0;
			//预留buff添加逻辑(有些buff是需要在当次攻击产生影响的,所以添加buff和debuff必须在当次计算伤害之前添加)
			switch (skill.Type) {
			case SkillType.Plus:
			default:
				break;
			case SkillType.FixedDamage:
				hurt = -fromRole.FixedDamage;
				break;
			case SkillType.PhysicsAttack:
				hurt = -fromRole.GetPhysicsDamage(toRole);
				break;
			case SkillType.MagicAttack:
				hurt = -fromRole.GetMagicDamage(toRole);
				break;
			}
			return hurt;
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="powerMult">Power mult.</param>
		void doSkill(string teamName) {
			float powerMult;
			int hurtHP;
			if (teamName == "Team") {
				if (currentTeamBook == null) {
					return;
				}
				if (canTeamDoSkill) {
					canTeamDoSkill = false;
					powerMult = teamWeaponPowerPlus.GetPowerMultiplyingByCollision(teamWeaponRunLineRect);
					if (powerMult > 0) {
						SkillData currentSkill = currentTeamBook.GetCurrentSkill();
						if (currentSkill != null) {
							//计算是否闪避
							if (currentTeamRole.IsHited(currentEnemyRole)) {
								teamSkillNameShow.StartPlay(currentSkill.Name);
								currentTeamSkill = currentTeamBook.NextSkill();
								if (currentTeamBook.CurrentSkillIndex == 0) {
									teamGotSkills.Clear();
								}
								else {
									teamGotSkills.Pop(currentTeamBook.CurrentSkillIndex - 1);
								}
								//计算本方对敌方伤害
								hurtHP = dealHurtHPAndBuffs(currentSkill, currentTeamRole, currentEnemyRole);
								if (hurtHP > 0) {
									currentTeamRole.DealHP(hurtHP);
									refreshTeamHP();
									Statics.CreatePopMsg(teamBloodBgRectTrans.position, "+" + hurtHP.ToString(), Color.green, 40, 0.2f);
								}
								else if (hurtHP < 0) {
									currentEnemyRole.DealHP(hurtHP);
									refreshEnemyHP();
									Statics.CreatePopMsg(enemyBloodBgRectTrans.position, hurtHP.ToString(), Color.red, 40, 2);
								}
							}
							else {
								Statics.CreatePopMsg(enemyBloodBgRectTrans.position, "闪避", Color.blue, 40, 0.5f);
							}
							
						}
						else {
							Statics.CreatePopMsg(teamBloodBgRectTrans.position, "空招", Color.cyan, 40, 0.2f);
						}
					}
					else {
						teamGotSkills.Clear();
						currentTeamSkill = currentTeamBook.Restart();
					}
					battleProgressText.text = currentTeamSkill.Name;
					teamSkillHoldOnDate = Time.fixedTime;
				}
			}
			else {
				if (currentEnemyBook == null) {
					return;
				}
				if (canEnemyDoSkill) {
					canEnemyDoSkill = false;
					powerMult = enemyWeaponPowerPlus.GetPowerMultiplyingByCollision(enemyWeaponRunLineRect);
					if (powerMult > 0) {
						SkillData currentSkill = currentEnemyBook.GetCurrentSkill();
						if (currentSkill != null) {
							//计算是否闪避
							if (currentEnemyRole.IsHited(currentTeamRole)) {
								enemySkillNameShow.StartPlay(currentSkill.Name);
								currentEnemySkill = currentEnemyBook.NextSkill();
								if (currentEnemyBook.CurrentSkillIndex == 0) {
									enemyGotSkills.Clear();
								}
								else {
									enemyGotSkills.Pop(currentEnemyBook.CurrentSkillIndex - 1);
								}
								//计算敌方对本方伤害
								hurtHP = dealHurtHPAndBuffs(currentSkill, currentEnemyRole, currentTeamRole);
								if (hurtHP > 0) {
									currentEnemyRole.DealHP(hurtHP);
									refreshEnemyHP();
									Statics.CreatePopMsg(enemyBloodBgRectTrans.position, "+" + hurtHP.ToString(), Color.green, 40, 0.2f);
								}
								else if (hurtHP < 0) {
									currentTeamRole.DealHP(hurtHP);
									refreshTeamHP();
									Statics.CreatePopMsg(teamBloodBgRectTrans.position, hurtHP.ToString(), Color.red, 40, 2);
								}
							}
							else {
								Statics.CreatePopMsg(teamBloodBgRectTrans.position, "闪避", Color.blue, 40, 0.5f);
							}
						}
						else {
							Statics.CreatePopMsg(enemyBloodBgRectTrans.position, "空招", Color.cyan, 40, 0.2f);
						}
					}
					else {
						enemyGotSkills.Clear();
						currentEnemySkill = currentEnemyBook.Restart();
					}
					enemySkillHoldOnDate = Time.fixedTime;
				}
			}
		}

		/// <summary>
		/// 重置武器技能标尺的位置
		/// </summary>
		/// <param name="teamName">Team name.</param>
		void resetSkillAndWeaponPosition(string teamName) {
			float randomPostionX;
			if (teamName == "Team") {
				if (currentTeamRole != null && currentTeamRole.Weapon != null) {
					randomPostionX = Random.Range(100 + currentTeamRole.Weapon.Width * 0.5f, 584 - currentTeamRole.Weapon.Width * 0.5f) - 292;
					teamWeaponPowerBg.anchoredPosition = new Vector2(randomPostionX, teamWeaponPowerBg.anchoredPosition.y);
					teamWeaponShowLittleBg.anchoredPosition = new Vector2(randomPostionX, teamWeaponShowLittleBg.anchoredPosition.y);
					teamCurSkillIconImageRectTrans.anchoredPosition = new Vector2(randomPostionX, teamCurSkillIconImageRectTrans.anchoredPosition.y);
					teamWeaponShowBg.anchoredPosition = new Vector2(randomPostionX, teamWeaponShowBg.anchoredPosition.y);
				}
			}
			else {
				if (currentEnemyRole != null && currentEnemyRole.Weapon != null) {
					randomPostionX = Random.Range(100 + currentEnemyRole.Weapon.Width * 0.5f, 584 - currentEnemyRole.Weapon.Width * 0.5f) - 292;
					enemyWeaponPowerBg.anchoredPosition = new Vector2(randomPostionX, enemyWeaponPowerBg.anchoredPosition.y);
					enemyWeaponShowLittleBg.anchoredPosition = new Vector2(randomPostionX, enemyWeaponShowLittleBg.anchoredPosition.y);
					enemyCurSkillIconImageRectTrans.anchoredPosition = new Vector2(randomPostionX, enemyCurSkillIconImageRectTrans.anchoredPosition.y);
					enemyWeaponShowBg.anchoredPosition = new Vector2(randomPostionX, enemyWeaponShowBg.anchoredPosition.y);
				}
			}
		}

		/// <summary>
		/// 己方自动战斗
		/// </summary>
		void teamAuto() {
			if (!canTeamDoSkill) {
				return;
			}
			float newDate = Time.fixedTime;
			if (teamAutoFightTimeout == 0) {
				float costTime = 584.0f / currentTeamRole.AttackSpeed / 30.0f;
				teamAutoFightDate = newDate;
				teamAutoFightTimeout = Random.Range(costTime * 0.3f, costTime * 0.9f);
			}
			if (newDate - teamAutoFightDate > teamAutoFightTimeout) {
				doSkill("Team");
				teamAutoFightTimeout = 0;
			}
		}

		/// <summary>
		/// 敌人自动战斗
		/// </summary>
		void enemyAuto() {
			if (!canEnemyDoSkill) {
				return;
			}
			float newDate = Time.fixedTime;
			if (enemyAutoFightTimeout == 0) {
				float costTime = 584.0f / currentEnemyRole.AttackSpeed / 30.0f;
				enemyAutoFightDate = newDate;
				enemyAutoFightTimeout = Random.Range(costTime * 0.3f, costTime * 0.9f);
			}
			if (newDate - enemyAutoFightDate > enemyAutoFightTimeout) {
				doSkill("Enemy");
				enemyAutoFightTimeout = 0;
			}
		}

		void Update() {
			if (!playing) {
				return;
			}
			if (currentTeamRole != null) {
				if (Time.fixedTime - teamSkillHoldOnDate >= holdOnTimeout) {
					teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
					teamLineX += currentTeamRole.AttackSpeed;
					if (teamLineX >= 292) {
						canTeamDoSkill = true;
						teamLineX = -292 + (teamLineX - 292);
						resetSkillAndWeaponPosition("Team");
					}
				}
				if (teamAutoFight) {
					teamAuto();
				}
			}
			if (currentEnemyRole != null) {
				if (Time.fixedTime - enemySkillHoldOnDate >= holdOnTimeout) {
					enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);
					enemyLineX += currentEnemyRole.AttackSpeed;
					if (enemyLineX >= 292) {
						canEnemyDoSkill = true;
						enemyLineX = -292 + (enemyLineX - 292);
						resetSkillAndWeaponPosition("Enemy");
					}
				}
				if (enemyAutoFight) {
					enemyAuto();
				}
			}
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
			canTeamDoSkill = true;
			canEnemyDoSkill = true;
			teamSkillHoldOnDate = Time.fixedTime;
			enemySkillHoldOnDate = Time.fixedTime;
			teamLineX = -292;
			enemyLineX = -292;
			teamAutoFightTimeout = 0;
			enemyAutoFightTimeout = 0;
			teamAutoFight = false;
			enemyAutoFight = true;
			UpdateCurrentTeamRole(currentRole);
			fightData = fight;
			enemyRoleDatas = fightData.Enemys;
			callEnemey();
		}

		public override void RefreshView () {
			canvasGroup.alpha = 0;
			Pause();
			canvasGroup.DOFade(1, 2).SetAutoKill(false).OnComplete(() => {
				Play();
			});
			teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
			enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);

			resetSkillAndWeaponPosition("Team");
			resetSkillAndWeaponPosition("Enemy");

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

		void refreshTeamHP() {
			teamBloodProgress.rectTransform.sizeDelta = new Vector2(currentTeamRole.HPRate * 556, teamBloodProgress.rectTransform.sizeDelta.y);
		}

		public void RefreshTeamView() {
			if (currentTeamRole != null) {
				refreshTeamHP();
				if (currentTeamRole.Weapon != null) {
					teamWeaponPowerBg.sizeDelta = new Vector2(currentTeamRole.Weapon.Width, teamWeaponPowerBg.sizeDelta.y);
					teamWeaponPowerPlus.SetRates(currentTeamRole.Weapon.Width, currentTeamRole.Weapon.Rates);
					teamWeaponShowBg.sizeDelta = new Vector2(currentTeamRole.Weapon.Width + 60, teamWeaponShowBg.sizeDelta.y);
				}
			}
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

		void refreshEnemyHP() {
			enemyBloodProgress.rectTransform.sizeDelta = new Vector2(currentEnemyRole.HPRate * 556, enemyBloodProgress.rectTransform.sizeDelta.y);
		}

		void refreshEnemyView() {
			if (currentEnemyRole != null) {
				enemyBody.sprite = Statics.GetHalfBodySprite(currentEnemyRole.HalfBodyId);
				enemyName.text = currentEnemyRole.Name;
				refreshEnemyHP();
				if (currentEnemyRole.Weapon != null) {
					enemyWeaponPowerBg.sizeDelta = new Vector2(currentEnemyRole.Weapon.Width, enemyWeaponPowerBg.sizeDelta.y);
					enemyWeaponPowerPlus.SetRates(currentEnemyRole.Weapon.Width, currentEnemyRole.Weapon.Rates);
					enemyWeaponShowBg.sizeDelta = new Vector2(currentEnemyRole.Weapon.Width + 60, enemyWeaponShowBg.sizeDelta.y);
				}
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
