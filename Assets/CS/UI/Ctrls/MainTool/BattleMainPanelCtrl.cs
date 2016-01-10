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
		Color teamWeaponRunLineColor;
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
		GotBuffs teamGotBuffs;
		Image teamDisableMask;

		//敌方
		Image enemyBody;
		Text enemyName;
		RectTransform enemyWeaponPowerBg;
		Image enemyWeaponRunLine;
		Color enemyWeaponRunLineColor;
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
		GotBuffs enemyGotBuffs;
		Image enemyDisableMask;

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

		float holdOnTimeout = 0.5f;
		float teamSkillHoldOnDate;
		float enemySkillHoldOnDate;

		bool teamAutoFight;
		float teamAutoFightTimeout;
		float teamAutoFightDate;
		bool enemyAutoFight;
		float enemyAutoFightTimeout;
		float enemyAutoFightDate;

		List<BuffData> teamBuffs;
		List<BuffData> enemyBuffs;

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
			teamGotBuffs = GetChild("teamGotBuffs").GetComponent<GotBuffs>();
			teamDisableMask = GetChildImage("teamDisableMask");
			//技能遮罩条默认藏掉
			teamDisableMask.gameObject.SetActive(false);

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
			enemyGotBuffs = GetChild("enemyGotBuffs").GetComponent<GotBuffs>();
			enemyDisableMask = GetChildImage("enemyDisableMask");
			//敌人的技能条遮罩一直存在
			enemyDisableMask.gameObject.SetActive(true);
			enemyDisableMask.transform.GetChild(0).gameObject.SetActive(false);

			teamBuffs = new List<BuffData>();
			enemyBuffs = new List<BuffData>();

			teamWeaponRunLineColor = teamWeaponRunLine.color;
			enemyWeaponRunLineColor = enemyWeaponRunLine.color;
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

		void popMsg(string teamName, string msg, Color color, int fontSize, float strength) {
			Vector3 position = teamName == "Team" ? teamBloodBgRectTrans.position + Vector3.down : enemyBloodBgRectTrans.position;
			Statics.CreatePopMsg(position, msg, color, fontSize, strength);
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
				//判断角色是否被禁止使用技能
				if (!currentTeamRole.CanUseSkill) {
					return;
				}
				if (canTeamDoSkill) {
					canTeamDoSkill = false;
					teamWeaponRunLine.color = new Color(0.3f, 0.3f, 0.3f);
					powerMult = teamWeaponPowerPlus.GetPowerMultiplyingByCollision(teamWeaponRunLineRect);
					if (powerMult > 0) {
						SkillData currentSkill = currentTeamBook.GetCurrentSkill();
						if (currentSkill != null) {
							//计算是否闪避
							if (currentTeamRole.IsHited(currentEnemyRole)) {
								teamSkillNameShow.StartPlay(currentSkill.Name);
								currentTeamSkill = currentTeamBook.NextSkill();
								//处理技能招式循环
								if (currentTeamBook.CurrentSkillIndex == 0) {
									teamGotSkills.Clear();
								}
								else {
									teamGotSkills.Pop(currentTeamBook.CurrentSkillIndex - 1);
								}
								//计算buff和debuff, 这里需要buff对象的克隆
								BuffData buff;
								BuffData searchBuff;
								for (int i = 0; i < currentSkill.BuffDatas.Count; i++) {
									buff = currentSkill.BuffDatas[i];
									if (buff.IsTrigger()) {
										if (buff.FirstEffect) {
											appendBuffParams("Team", buff);
										}
										//相同的buff不能重复添加
										searchBuff = teamBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											teamBuffs.Add(buff.GetClone());
										}
									}
								}
								for (int i = 0; i < currentSkill.DeBuffDatas.Count; i++) {
									buff = currentSkill.DeBuffDatas[i];
									if (buff.IsTrigger()) {
										if (buff.FirstEffect) {
											appendBuffParams("Enemy", buff);
										}
										//相同的debuff不能重复添加
										searchBuff = enemyBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											enemyBuffs.Add(buff.GetClone());
										}
									}
								}
								//计算本方对敌方伤害
								hurtHP = dealHurtHPAndBuffs(currentSkill, currentTeamRole, currentEnemyRole);
								if (hurtHP > 0) {
									currentTeamRole.DealHP(hurtHP);
									popMsg("Team", "+" + hurtHP.ToString(), Color.green, 40, 0.2f);
								}
								else if (hurtHP < 0) {
									if (currentTeamRole.CanNotMakeMistake) {
										currentEnemyRole.DealHP(hurtHP);
										popMsg("Enemy", hurtHP.ToString(), Color.red, 40, 2);
									}
									else {
										//处理混乱误伤
										if (Random.Range(1, 100) <= 50) {
											currentEnemyRole.DealHP(hurtHP);
											popMsg("Enemy", hurtHP.ToString(), Color.red, 40, 2);
										}
										else {
											currentTeamRole.DealHP(hurtHP);
											popMsg("Team", hurtHP.ToString() + "(误伤)", Color.red, 40, 2);
										}
									}
								}
								refreshTeamHP();
								refreshTeamBuffs();
								refreshEnemyHP();
								refreshEnemyBuffs();
							}
							else {
								popMsg("Enemy", "闪避", new Color(0, 0.7f, 1), 40, 0.5f);
							}
							
						}
						else {
							popMsg("Team", "空招", Color.cyan, 40, 0.2f);
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
				//判断角色是否被禁止使用技能
				if (!currentEnemyRole.CanUseSkill) {
					return;
				}
				if (canEnemyDoSkill) {
					canEnemyDoSkill = false;
					enemyWeaponRunLine.color = new Color(0.3f, 0.3f, 0.3f);
					powerMult = enemyWeaponPowerPlus.GetPowerMultiplyingByCollision(enemyWeaponRunLineRect);
					if (powerMult > 0) {
						SkillData currentSkill = currentEnemyBook.GetCurrentSkill();
						if (currentSkill != null) {
							//计算是否闪避
							if (currentEnemyRole.IsHited(currentTeamRole)) {
								enemySkillNameShow.StartPlay(currentSkill.Name);
								currentEnemySkill = currentEnemyBook.NextSkill();
								//处理技能招式循环
								if (currentEnemyBook.CurrentSkillIndex == 0) {
									enemyGotSkills.Clear();
								}
								else {
									enemyGotSkills.Pop(currentEnemyBook.CurrentSkillIndex - 1);
								}
								//计算buff和debuff, 这里需要buff对象的克隆
								BuffData buff;
								BuffData searchBuff;
								for (int i = 0; i < currentSkill.BuffDatas.Count; i++) {
									buff = currentSkill.BuffDatas[i];
									if (buff.IsTrigger()) {
										if (buff.FirstEffect) {
											appendBuffParams("Enemy", buff);
										}
										//相同的buff不能重复添加
										searchBuff = enemyBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											enemyBuffs.Add(buff.GetClone());
										}
									}
								}
								for (int i = 0; i < currentSkill.DeBuffDatas.Count; i++) {
									buff = currentSkill.DeBuffDatas[i];
									if (buff.IsTrigger()) {
										if (buff.FirstEffect) {
											appendBuffParams("Team", buff);
										}
										searchBuff = teamBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											teamBuffs.Add(buff.GetClone());
										}
									}
								}
								//计算敌方对本方伤害
								hurtHP = dealHurtHPAndBuffs(currentSkill, currentEnemyRole, currentTeamRole);
								if (hurtHP > 0) {
									currentEnemyRole.DealHP(hurtHP);
									popMsg("Enemy", "+" + hurtHP.ToString(), Color.green, 40, 0.2f);
								}
								else if (hurtHP < 0) {
									
									if (currentEnemyRole.CanNotMakeMistake) {
										currentTeamRole.DealHP(hurtHP);
										popMsg("Team", hurtHP.ToString(), Color.red, 40, 2);
									}
									else {
										//处理混乱误伤
										if (Random.Range(1, 100) <= 50) {
											currentTeamRole.DealHP(hurtHP);
											popMsg("Team", hurtHP.ToString(), Color.red, 40, 2);
										}
										else {
											currentEnemyRole.DealHP(hurtHP);
											popMsg("Enemy", hurtHP.ToString() + "(误伤)", Color.red, 40, 2);
										}
									}
								}
								refreshEnemyHP();
								refreshEnemyBuffs();
								refreshTeamHP();
								refreshTeamBuffs();
							}
							else {
								popMsg("Team", "闪避", new Color(0, 0.7f, 1), 40, 0.5f);
							}
						}
						else {
							popMsg("Enemy", "空招", Color.cyan, 40, 0.2f);
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
					teamLineX += currentTeamRole.AttackSpeed * (canTeamDoSkill ? 1 : 2);
					if (teamLineX >= 292) {
						canTeamDoSkill = true;
						teamWeaponRunLine.color = teamWeaponRunLineColor;
						teamLineX = -292 + (teamLineX - 292);
						resetSkillAndWeaponPosition("Team");
						buffsAction("Team");
					}
				}
				if (teamAutoFight) {
					teamAuto();
				}
			}
			if (currentEnemyRole != null) {
				if (Time.fixedTime - enemySkillHoldOnDate >= holdOnTimeout) {
					enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);
					enemyLineX += currentEnemyRole.AttackSpeed * (canEnemyDoSkill ? 1 : 2);
					if (enemyLineX >= 292) {
						canEnemyDoSkill = true;
						enemyWeaponRunLine.color = enemyWeaponRunLineColor;
						enemyLineX = -292 + (enemyLineX - 292);
						resetSkillAndWeaponPosition("Enemy");
						buffsAction("Enemy");
					}
				}
				if (enemyAutoFight) {
					enemyAuto();
				}
			}
		}

		public void Play() {
			playing = true;
			canTeamDoSkill = true;
			canEnemyDoSkill = true;
			teamLineX = -292;
			enemyLineX = -292;
		}

		public void Pause() {
			playing = false;
			canTeamDoSkill = false;
			canEnemyDoSkill = false;
		}

		public void UpdateData(RoleData currentRole, FightData fight) {
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
			teamBuffs.Clear();
			enemyBuffs.Clear();
		}

		public override void RefreshView () {
			canvasGroup.alpha = 0;
			Pause();
			canvasGroup.DOFade(1, 2).SetAutoKill(false).OnComplete(() => {
				Play();
			});
			teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
			enemyWeaponRunLineRect.anchoredPosition = new Vector2(enemyLineX, enemyWeaponRunLineRect.anchoredPosition.y);

			teamDisableMask.gameObject.SetActive(false);
			enemyDisableMask.transform.GetChild(0).gameObject.SetActive(false);

			resetSkillAndWeaponPosition("Team");
			resetSkillAndWeaponPosition("Enemy");

			RefreshTeamView();
			refreshEnemyView();
		}

		public void UpdateCurrentTeamRole(RoleData currentRole) {
			if (currentRole != null) {
				currentTeamRole = currentRole;
				currentTeamRole.Init();
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

		void refreshTeamBuffs() {
			teamGotBuffs.SetBuffDatas(teamBuffs);
		}

		public void RefreshTeamView() {
			if (currentTeamRole != null) {
				refreshTeamHP();
				refreshTeamBuffs();
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
				currentEnemyRole.Init();
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

		void refreshEnemyBuffs() {
			enemyGotBuffs.SetBuffDatas(enemyBuffs);
		}

		void refreshEnemyView() {
			if (currentEnemyRole != null) {
				enemyBody.sprite = Statics.GetHalfBodySprite(currentEnemyRole.HalfBodyId);
				enemyName.text = currentEnemyRole.Name;
				refreshEnemyHP();
				refreshEnemyBuffs();
				if (currentEnemyRole.Weapon != null) {
					enemyWeaponPowerBg.sizeDelta = new Vector2(currentEnemyRole.Weapon.Width, enemyWeaponPowerBg.sizeDelta.y);
					enemyWeaponPowerPlus.SetRates(currentEnemyRole.Weapon.Width, currentEnemyRole.Weapon.Rates);
					enemyWeaponShowBg.sizeDelta = new Vector2(currentEnemyRole.Weapon.Width + 60, enemyWeaponShowBg.sizeDelta.y);
				}
			}
		}

		void buffsAction(string teamName) {
			BuffData curBuff;
			List<BuffData> buffs = teamName == "Team" ? teamBuffs : enemyBuffs;
			RoleData role = teamName == "Team" ? currentTeamRole : currentEnemyRole;

			//清空旧的buff和debuff
			role.ClearPluses();

			for (int i = buffs.Count - 1; i >= 0; i--) {
				curBuff = buffs[i];
//				if (curBuff.RoundNumber-- <= 0) {
//					buffs.RemoveAt(i);
//					continue;
//				}
				if (curBuff.RoundNumber-- > 0) {
					appendBuffParams(teamName, curBuff);
				}
				if (curBuff.RoundNumber <= 0) {
					buffs.RemoveAt(i);
				}
			}

			if (teamName == "Team") {
				refreshTeamHP();
				refreshTeamBuffs();
			}
			else {
				refreshEnemyHP();
				refreshEnemyBuffs();
			}
			//处理界面上的展示效果
			refreshDisable(teamName);
		}

		/// <summary>
		/// 清空充值buff和debuff的属性叠加
		/// </summary>
		/// <param name="teamName">Team name.</param>
		void clearBuffParams(string teamName) {
			RoleData role = teamName == "Team" ? currentTeamRole : currentEnemyRole;
			role.ClearPluses();
		}

		/// <summary>
		/// 处理buff和debuff的属性叠加
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="buff">Buff.</param>
		void appendBuffParams(string teamName, BuffData buff) {
			RoleData role = teamName == "Team" ? currentTeamRole : currentEnemyRole;
			switch (buff.Type) {
			case BuffType.Slow: //迟缓
				role.AttackSpeedPlus -= role.AttackSpeed * buff.Value;
				break;
			case BuffType.Fast: //疾走
				role.AttackSpeedPlus += role.AttackSpeed * buff.Value;
				break;
			case BuffType.Drug: //中毒
				int cutHP = -(int)((float)role.HP * 0.1f);
				role.HP += cutHP;
				popMsg(teamName, cutHP.ToString() + "(毒)", Color.red, 30, 1);
				break;
			case BuffType.CanNotMove: //定身
				role.CanChangeRole = false;
				//处理界面上的展示效果
				refreshDisable(teamName);
				break;
			case BuffType.Chaos: //混乱
				role.CanNotMakeMistake = false;
				break;
			case BuffType.Disarm: //缴械
				role.CanUseSkill = false;
				//处理界面上的展示效果
				refreshDisable(teamName);
				break;
			case BuffType.Vertigo: //眩晕
				role.CanUseSkill = false;
				role.CanChangeRole = false;
				//处理界面上的展示效果
				refreshDisable(teamName);
				break;
			case BuffType.IncreaseDamageRate: //增减益伤害比例
				role.DamageRatePlus += (int)((float)role.DamageRate * buff.Value);
				break;
			case BuffType.IncreaseFixedDamage: //增减益固定伤害
				role.FixedDamagePlus += (int)buff.Value;
				break;
			case BuffType.IncreaseHP: //增减益气血
				int addHP = (int)buff.Value;
				role.HP += addHP;
				popMsg(teamName, "+" + addHP.ToString(), Color.green, 40, 0.2f);
				break;
			case BuffType.IncreaseMaxHP: //增减益气血上限
				role.MaxHPPlus += (int)buff.Value;
				break;
			case BuffType.IncreaseMaxHPRate: //增减益气血上限比例
				role.MaxHPPlus += (int)((float)role.MaxHP * buff.Value);
				break;
			case BuffType.IncreaseHurtCutRate: //增减减伤比例
				role.HurtCutRatePlus += buff.Value;
				break;
			case BuffType.IncreaseMagicAttack: //增减益内功点数
				role.MagicAttackPlus += buff.Value;
				break;
			case BuffType.IncreaseMagicAttackRate: //增减益内功比例
				role.MagicAttackPlus += (role.MagicAttack * buff.Value);
				break;
			case BuffType.IncreaseMagicDefense: //增减益内防点数
				role.MagicDefensePlus += buff.Value;
				break;
			case BuffType.IncreaseMagicDefenseRate: //增减益内防比例
				role.MagicDefensePlus += (role.MagicDefense * buff.Value);
				break;
			case BuffType.IncreasePhysicsAttack: //增减益外功点数
				role.PhysicsAttackPlus += buff.Value;
				break;
			case BuffType.IncreasePhysicsAttackRate: //增减益外功比例
				role.PhysicsAttackPlus += (role.PhysicsAttack * buff.Value);
				break;
			case BuffType.IncreasePhysicsDefense: //增减益外防点数
				role.PhysicsDefensePlus += buff.Value;
				break;
			case BuffType.IncreasePhysicsDefenseRate: //增减益外防比例
				role.PhysicsDefensePlus += (role.PhysicsDefense * buff.Value);
				break;
			default:
				break;
			}
		}

		void refreshDisable(string teamName) {
			if (teamName == "Team") {
				teamDisableMask.gameObject.SetActive(!currentTeamRole.CanUseSkill);
				Messenger.Broadcast<bool>(NotifyTypes.MakeChangeBookEnable, currentTeamRole.CanUseSkill);
				Messenger.Broadcast<bool>(NotifyTypes.MakeChangeRoleEnable, currentTeamRole.CanChangeRole);
			}
			else {
				enemyDisableMask.transform.GetChild(0).gameObject.SetActive(!currentEnemyRole.CanUseSkill);
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
