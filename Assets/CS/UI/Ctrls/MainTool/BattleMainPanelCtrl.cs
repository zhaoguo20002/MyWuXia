using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Net.Cache;

namespace Game {
	public class BattleMainPanelCtrl : WindowCore<BattleMainPanelCtrl, JArray> {
		
		CanvasGroup canvasGroup;
		Button doSkillButton;
		Text battleProgressText;
		Text roundbattleProgressText;

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
		Text teamBloodText;
		GotBuffs teamGotBuffs;
		Image teamDisableMask;
		CanvasGroup teamWeaponGroup;
		Text teamNoticeText;

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
		Text enemyBloodText;
		GotBuffs enemyGotBuffs;
		Image enemyDisableMask;
		CanvasGroup enemyWeaponGroup;
		Text enemyNoticeText;

		Image winSprite;
		Image failSprite;

		RoleData currentTeamRole;
		BookData currentTeamBook;
		SkillData currentTeamSkill;

		FightData fightData;
		List<RoleData> enemyRoleDatas;
		int maxEnemyNum;
		RoleData currentEnemyRole;
		BookData currentEnemyBook;
		SkillData currentEnemySkill;

		bool playing;
		bool ending;
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

		System.Action playDelayCallback;
		float playDelayDate = 0;
		float playDelayTimeout = 0;

		GameObject guoJinSkillObj;
		Queue<MsgQueueData> noticeQueue;

		Dictionary<string, int> usedSkillIdMapping; //记录使用过的招式
		Dictionary<int, int> plusIndexMapping; //记录兵器暴击

		SkillData normalSkill;

		int curRound = 0;
		int maxRound = 300; //最大招数，这个招数归0时还未分出胜负则判攻方败

		protected override void Init () {
			doSkillButton = GetChildButton("doSkillButton");
			EventTriggerListener.Get(doSkillButton.gameObject).onClick += onClick;
			battleProgressText = GetChildText("battleProgressText");
			roundbattleProgressText = GetChildText("roundbattleProgressText");

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
			teamBloodText = GetChildText("teamBloodText");
			teamGotBuffs = GetChild("teamGotBuffs").GetComponent<GotBuffs>();
			teamDisableMask = GetChildImage("teamDisableMask");
			//技能遮罩条默认藏掉
			teamDisableMask.gameObject.SetActive(false);
			teamWeaponGroup = GetChildCanvasGroup("teamWeaponBg");
			teamNoticeText = GetChildText("teamNoticeText");
			teamNoticeText.text = "";

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
			enemyBloodText = GetChildText("enemyBloodText");
			enemyGotBuffs = GetChild("enemyGotBuffs").GetComponent<GotBuffs>();
			enemyDisableMask = GetChildImage("enemyDisableMask");
			enemyWeaponGroup = GetChildCanvasGroup("enemyWeaponBg");
			enemyNoticeText = GetChildText("enemyNoticeText");
			enemyNoticeText.text = "";

			winSprite = GetChildImage("winSprite");
			failSprite = GetChildImage("failSprite");

			//敌人的技能条遮罩一直存在
			enemyDisableMask.gameObject.SetActive(true);
			enemyDisableMask.transform.GetChild(0).gameObject.SetActive(false);

			teamBuffs = new List<BuffData>();
			enemyBuffs = new List<BuffData>();

			teamWeaponRunLineColor = teamWeaponRunLine.color;
			enemyWeaponRunLineColor = enemyWeaponRunLine.color;
			noticeQueue = new Queue<MsgQueueData>();

			usedSkillIdMapping = new Dictionary<string, int>();
			plusIndexMapping = new Dictionary<int, int>();
		}

		void onClick(GameObject e) {
			if (!playing) {
				return;
			}
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
		/// 处理技能特效和音效
		/// </summary>
		/// <param name="skill">Skill.</param>
		void dealSkillEffectAndSound(string teamName, SkillData skill) {
			if (skill == null || skill.EffectSoundId == "" || skill.EffectSrc == "") {
				return;
			}
			if (teamName == "Team") {
				GameObject effect = Statics.GetSkillEffectPrefabClone(skill.EffectSrc);
				if (effect != null) {
					effect.transform.SetParent(enemyBody.transform);
					effect.transform.localPosition = Vector3.zero;
				}
			}
			else {
				
			}
			SoundManager.GetInstance().PushSound(skill.EffectSoundId);
		}

		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="teamName">Team name.</param>
		/// <param name="powerMult">Power mult.</param>
		void doSkill(string teamName) {
			if (ending || !playing) {
				return;
			}
			float powerMult;
			int hurtHP;
			if (teamName == "Team") {
//				if (currentTeamBook == null) {
//					return;
//				}
				//判断角色是否被禁止使用技能
				if (!currentTeamRole.CanUseSkill) {
					return;
				}
				if (canTeamDoSkill) {
					canTeamDoSkill = false;
					teamWeaponRunLine.color = new Color(0.3f, 0.3f, 0.3f);
					powerMult = teamWeaponPowerPlus.GetPowerMultiplyingByCollision(teamWeaponRunLineRect);
					if (powerMult > 0) {
						SkillData currentSkill;
						if (currentTeamBook != null) {
							currentSkill = currentTeamBook.GetCurrentSkill();
						}
						else {
							if (normalSkill == null) {
								normalSkill = JsonManager.GetInstance().GetMapping<SkillData>("Skills", "1");
							}
							currentSkill = normalSkill;
						}

						if (currentSkill != null) {
							//记录使用过的招式
							if (!usedSkillIdMapping.ContainsKey(currentSkill.Id)) {
								usedSkillIdMapping.Add(currentSkill.Id, 1);
							}
							else {
								usedSkillIdMapping[currentSkill.Id]++;
							}
							//记录武器暴击
							int plusIndex;
							if (powerMult == 2f) {
								plusIndex = 3;
							}
							else if (powerMult == 1.5f) {
								plusIndex = 2;
							}
							else {
								plusIndex = 1;
							}
							if (!plusIndexMapping.ContainsKey(plusIndex)) {
								plusIndexMapping.Add(plusIndex, 1);
							}
							else {
								plusIndexMapping[plusIndex]++;
							}
							//播放技能粒子特效和音效
							dealSkillEffectAndSound("Team", currentSkill);
							enemyBody.transform.DOShakePosition(0.5f, powerMult * 10, 20, 180);
							//计算是否闪避
							if (currentTeamRole.IsHited(currentEnemyRole)) {
								teamSkillNameShow.StartPlay(currentSkill.Name);
								if (currentTeamBook != null) {
									currentTeamSkill = currentTeamBook.NextSkill();
									//处理技能招式循环
									if (currentTeamBook.CurrentSkillIndex == 0) {
										teamGotSkills.Clear();
									}
									else {
										teamGotSkills.Pop(currentTeamBook.CurrentSkillIndex - 1);
									}
                                    //处理当前招式icon
                                    teamCurSkillIconImage.sprite = Statics.GetIconSprite(currentTeamBook.GetCurrentSkill().IconId);
								}
								else {
									currentTeamSkill = normalSkill;
								}
								//计算buff和debuff, 这里需要buff对象的克隆
								BuffData buff;
								BuffData searchBuff;
								for (int i = 0; i < currentSkill.BuffDatas.Count; i++) {
									buff = currentSkill.BuffDatas[i];
									if (buff.IsTrigger()) {
										//相同的buff不能重复添加
										searchBuff = teamBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											if (buff.FirstEffect) {
												appendBuffParams("Team", buff);
											}
											teamBuffs.Add(buff.GetClone());
										}
									}
								}
								bool willBeDebuff;
								for (int i = 0; i < currentSkill.DeBuffDatas.Count; i++) {
									buff = currentSkill.DeBuffDatas[i];
									if (buff.IsTrigger()) {
										//相同的debuff不能重复添加
										searchBuff = enemyBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											willBeDebuff = true;
											//处理抵抗
											switch (buff.Type) {
											case BuffType.Drug:
												//判断敌方是否有中毒抵抗
												if (enemyBuffs.FindIndex((item) => { return item.Type == BuffType.DrugResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Disarm:
												//判断敌方是否有缴械抵抗
												if (enemyBuffs.FindIndex((item) => { return item.Type == BuffType.DisarmResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Vertigo:
												//判断敌方是否有眩晕抵抗
												if (enemyBuffs.FindIndex((item) => { return item.Type == BuffType.VertigoResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.CanNotMove:
												//判断敌方是否有抵抗抵抗
												if (enemyBuffs.FindIndex((item) => { return item.Type == BuffType.CanNotMoveResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Slow:
												//判断敌方是否有迟缓抵抗
												if (enemyBuffs.FindIndex((item) => { return item.Type == BuffType.SlowResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Chaos:
												//判断敌方是否有混乱抵抗
												if (enemyBuffs.FindIndex((item) => { return item.Type == BuffType.ChaosResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											default:
												break;
											}
											if (willBeDebuff) {
												if (buff.FirstEffect) {
													appendBuffParams("Enemy", buff);
												}
												enemyBuffs.Add(buff.GetClone());
											}
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
									hurtHP = (int)((float)hurtHP * powerMult);
									string hurtHPStr = hurtHP.ToString() + (powerMult > 1 ? "<color=\"#FFFF00\">[x" + powerMult + "]</color>" : "");
									if (currentTeamRole.CanNotMakeMistake) {
										currentEnemyRole.DealHP(hurtHP);
										popMsg("Enemy", hurtHPStr, Color.red, 40, 2);
									}
									else {
										//处理混乱误伤
										if (Random.Range(1, 100) <= 50) {
											currentEnemyRole.DealHP(hurtHP);
											popMsg("Enemy", hurtHPStr, Color.red, 40, 2);
											//处理反伤，被攻击者没被打死再计算反伤
											if (currentEnemyRole.HP > 0) {
												BuffData reboundBuff = enemyBuffs.Find((item) => { return item.Type == BuffType.ReboundInjury; });
												if (reboundBuff != null) {
													int reboundHurtHp = (int)(hurtHP * reboundBuff.Value);
													currentTeamRole.DealHP(reboundHurtHp);
													popMsg("Team", reboundHurtHp + "(被反伤)", Color.red, 40, 2);
												}
											}
										}
										else {
											currentTeamRole.DealHP(hurtHP);
											popMsg("Team", hurtHPStr + "(误伤)", Color.red, 40, 2);
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
						if (currentTeamBook != null) {
							currentTeamSkill = currentTeamBook.Restart();
						}
						else {
							currentTeamSkill = normalSkill;
						}
                        //处理当前招式icon
                        teamCurSkillIconImage.sprite = Statics.GetIconSprite(currentTeamBook.GetCurrentSkill().IconId);
						Statics.CreatePopMsg(teamCurSkillIconImageRectTrans.position, "失手", Color.cyan, 30, 0.5f);
					}
					teamSkillHoldOnDate = Time.fixedTime;

					curRound--; //攻方或者守方施放招数后算进招数限制里
					refreshRound();
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
							//播放技能粒子特效和音效
							dealSkillEffectAndSound("Enemy", currentSkill);
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
                                //处理当前招式icon
                                enemyCurSkillIconImage.sprite = Statics.GetIconSprite(currentEnemyBook.GetCurrentSkill().IconId);
								//计算buff和debuff, 这里需要buff对象的克隆
								BuffData buff;
								BuffData searchBuff;
								for (int i = 0; i < currentSkill.BuffDatas.Count; i++) {
									buff = currentSkill.BuffDatas[i];
									if (buff.IsTrigger()) {
										//相同的buff不能重复添加
										searchBuff = enemyBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											if (buff.FirstEffect) {
												appendBuffParams("Enemy", buff);
											}
											enemyBuffs.Add(buff.GetClone());
										}
									}
								}
								bool willBeDebuff;
								for (int i = 0; i < currentSkill.DeBuffDatas.Count; i++) {
									buff = currentSkill.DeBuffDatas[i];
									if (buff.IsTrigger()) {
										searchBuff = teamBuffs.Find((item) => { return item.Type == buff.Type; });
										if (searchBuff == null) {
											willBeDebuff = true;
											//处理抵抗
											switch (buff.Type) {
											case BuffType.Drug:
												//判断本方是否有中毒抵抗
												if (teamBuffs.FindIndex((item) => { return item.Type == BuffType.DrugResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Disarm:
												//判断本方是否有缴械抵抗
												if (teamBuffs.FindIndex((item) => { return item.Type == BuffType.DisarmResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Vertigo:
												//判断本方是否有眩晕抵抗
												if (teamBuffs.FindIndex((item) => { return item.Type == BuffType.VertigoResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.CanNotMove:
												//判断本方是否有抵抗抵抗
												if (teamBuffs.FindIndex((item) => { return item.Type == BuffType.CanNotMoveResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Slow:
												//判断本方是否有迟缓抵抗
												if (teamBuffs.FindIndex((item) => { return item.Type == BuffType.SlowResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											case BuffType.Chaos:
												//判断本方是否有混乱抵抗
												if (teamBuffs.FindIndex((item) => { return item.Type == BuffType.ChaosResistance; }) >= 0) {
													willBeDebuff = false;
												}
												break;
											default:
												break;
											}
											if (willBeDebuff) {
												if (buff.FirstEffect) {
													appendBuffParams("Team", buff);
												}
												teamBuffs.Add(buff.GetClone());
											}
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
									hurtHP = (int)((float)hurtHP * powerMult);
									string hurtHPStr = hurtHP.ToString() + (powerMult > 1 ? "<color=\"#FFFF00\">[x" + powerMult + "]</color>" : "");
									if (currentEnemyRole.CanNotMakeMistake) {
										currentTeamRole.DealHP(hurtHP);
										popMsg("Team", hurtHPStr, Color.red, 40, 2);
									}
									else {
										//处理混乱误伤
										if (Random.Range(1, 100) <= 50) {
											currentTeamRole.DealHP(hurtHP);
											popMsg("Team", hurtHPStr, Color.red, 40, 2);
											//处理反伤，被攻击者没被打死再计算反伤
											if (currentTeamRole.HP > 0) {
												BuffData reboundBuff = teamBuffs.Find((item) => { return item.Type == BuffType.ReboundInjury; });
												if (reboundBuff != null) {
													int reboundHurtHp = (int)(hurtHP * reboundBuff.Value);
													currentEnemyRole.DealHP(reboundHurtHp);
													popMsg("Enemy", reboundHurtHp + "(被反伤)", Color.red, 40, 2);
												}
											}
										}
										else {
											currentEnemyRole.DealHP(hurtHP);
											popMsg("Enemy", hurtHPStr + "(误伤)", Color.red, 40, 2);
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
                        //处理当前招式icon
                        enemyCurSkillIconImage.sprite = Statics.GetIconSprite(currentEnemyBook.GetCurrentSkill().IconId);
						Statics.CreatePopMsg(enemyCurSkillIconImageRectTrans.position, "失手", Color.cyan, 30, 0.5f);
					}
					enemySkillHoldOnDate = Time.fixedTime;

					curRound--; //攻方或者守方施放招数后算进招数限制里
					refreshRound();
				}
			}
			checkDie();
		}

		/// <summary>
		/// 刷新回合数
		/// </summary>
		void refreshRound() {
			roundbattleProgressText.text = string.Format("{0}/{1}", curRound, maxRound);
		}

		/// <summary>
		/// 战败
		/// </summary>
		public void Faild() {
			SoundManager.GetInstance().PushSound(currentTeamRole.DeadSoundId, 1.5f);
			Pause();
			end(false);
		}

		/// <summary>
		/// 判定死亡
		/// </summary>
		void checkDie() {
			if (curRound <= 0) {
				Pause();
				AlertCtrl.Show(string.Format("大战{0}回合，你却未能击败对方！", maxRound), () => {
					end(false);
				}, "确定");
				return;
			}
			if (currentTeamRole != null) {
				if (currentTeamRole.HP <= 0) {
					//战死替换,延迟1秒 
					Timer.RemoveTimer("MakePopRoleTimer");
					Timer.AddTimer("MakePopRoleTimer", 1, null, (timer) => {
						Messenger.Broadcast<string>(NotifyTypes.MakePopRole, currentTeamRole.Id);
					});
					return;
				}
			}
			if (currentEnemyRole != null) {
				if (currentEnemyRole.HP <= 0) {
					SoundManager.GetInstance().PushSound(currentEnemyRole.DeadSoundId, 0.5f);
					Pause();
					enemyBody.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() => {
						callEnemy();
						if (currentEnemyRole == null) {
							end(true);
						}
						else {
							enemyBody.DOFade(1, 0.5f).OnComplete(() => {
								Play();
								enemyLineX = -292;
							});
						}
					});
				}
			}
		}

		/// <summary>
		/// 结束战斗前处理
		/// </summary>
		/// <param name="win">If set to <c>true</c> window.</param>
		void end(bool win) {
			ending = true;
			removeGuoJin();
			Finish();
			battleProgressText.text = "";
			teamWeaponGroup.DOFade(0, 1).SetDelay(2);
			enemyWeaponGroup.DOFade(0, 1).SetDelay(2);
			TweenCallback callback = () => {
				//将使用过的招式和兵器暴击整理后传递入库
				JArray usedSkillIdData = new JArray();
				foreach(string key in usedSkillIdMapping.Keys) {
					usedSkillIdData.Add(new JArray(key, usedSkillIdMapping[key]));
				}
				JArray plusIndexData = new JArray();
				foreach(int index in plusIndexMapping.Keys) {
					plusIndexData.Add(new JArray(index, plusIndexMapping[index]));
				}
				Messenger.Broadcast<JArray>(NotifyTypes.SendFightResult, new JArray(win, fightData.Id, win ? 1 : 0, usedSkillIdData, plusIndexData)); //目前没有考虑战斗评级系统，所以默认所有战斗都是1星
				winSprite.DOFade(0, 2);
				failSprite.DOFade(0, 2);
			};
			if (win) {
				setNoticeMsg("Team", " 技高一筹,战胜对手!", Color.green, true);
				winSprite.DOFade(1, 2).SetDelay(2).OnComplete(callback);
			}
			else {
				setNoticeMsg("Team", " 技不如人,战败!", Color.red, true);
				failSprite.DOFade(1, 2).SetDelay(2).OnComplete(callback);
			}
			SoundManager.GetInstance().StopBGM();
		}

		/// <summary>
		/// 重置武器技能标尺的位置
		/// </summary>
		/// <param name="teamName">Team name.</param>
		void resetSkillAndWeaponPosition(string teamName) {
			float randomPostionX;
			float left = 150;
			if (teamName == "Team") {
				if (currentTeamRole != null && currentTeamRole.Weapon != null && currentTeamBook != null) {
					//考虑下发招失误后是否立即让技能判定线回到初始位置,这样可以增加一个自己断招的玩法(当前招式太靠后端,为了抢先发招自己断招后判定线回到初始位置,辅助的需要增加一个设定是,第一招的技能标尺一定是处于靠前的位置)
					randomPostionX = currentTeamBook.CurrentSkillIndex == 0 ? left + currentTeamRole.Weapon.Width * 0.5f - 292 : 
						Random.Range(left + currentTeamRole.Weapon.Width * 0.5f, 584 - currentTeamRole.Weapon.Width * 0.5f) - 292;
					teamWeaponPowerBg.anchoredPosition = new Vector2(randomPostionX, teamWeaponPowerBg.anchoredPosition.y);
					teamWeaponShowLittleBg.anchoredPosition = new Vector2(randomPostionX, teamWeaponShowLittleBg.anchoredPosition.y);
					teamCurSkillIconImageRectTrans.anchoredPosition = new Vector2(randomPostionX, teamCurSkillIconImageRectTrans.anchoredPosition.y);
					teamWeaponShowBg.anchoredPosition = new Vector2(randomPostionX, teamWeaponShowBg.anchoredPosition.y);
				}
			}
			else {
				if (currentEnemyRole != null && currentEnemyRole.Weapon != null && currentEnemyBook != null) {
					randomPostionX = currentEnemyBook.CurrentSkillIndex == 0 ? left + currentEnemyRole.Weapon.Width * 0.5f - 292 : 
						Random.Range(left + currentEnemyRole.Weapon.Width * 0.5f, 584 - currentEnemyRole.Weapon.Width * 0.5f) - 292;
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
				if (playDelayDate > 0 && Time.fixedTime - playDelayDate >= playDelayTimeout) {
					if (playDelayCallback != null) {
						playDelayCallback();
						playDelayCallback = null;
						playDelayDate = 0;
					}
					Play();
				}
				return;
			}
			if (currentTeamRole != null) {
				if (Time.fixedTime - teamSkillHoldOnDate >= holdOnTimeout) {
					teamWeaponRunLineRect.anchoredPosition = new Vector2(teamLineX, teamWeaponRunLineRect.anchoredPosition.y);
//					teamLineX += currentTeamRole.AttackSpeed * (canTeamDoSkill ? 1 : 2);
					if (canTeamDoSkill) {
						teamLineX += currentTeamRole.AttackSpeed;
					}
					else {
						//使用技能后技能标尺恢复静止状态后将重头开始出现
						teamLineX = 292;
					}
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
//					enemyLineX += currentEnemyRole.AttackSpeed * (canEnemyDoSkill ? 1 : 2);
					if (canEnemyDoSkill) {
						enemyLineX += currentEnemyRole.AttackSpeed;
					}
					else {
						enemyLineX = 292;
					}
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
			popNotice();
		}

//		public void ReStart() {
//			Play();
//			reStartTeam();
//			reStartEnemy();
//		}

		void reStartTeam() {
			canTeamDoSkill = true;
			teamLineX = -292;
		}

		void reStartEnemy() {
			canEnemyDoSkill = true;
			enemyLineX = -292;
		}

		void removeGuoJin() {
			if (guoJinSkillObj != null) {
				Destroy(guoJinSkillObj);
				guoJinSkillObj = null;
			}
		}

		void pushNotice(string teamName, string msg, bool willClearMsg = false) {
			noticeQueue.Enqueue(new MsgQueueData("", teamName, msg, Color.cyan, willClearMsg));
		}

		void popNotice() {
			if (noticeQueue.Count > 0) {
				MsgQueueData msg = noticeQueue.Dequeue();
				setNoticeMsg(msg.Name, msg.Msg, msg.Color, msg.WillClearMsg);
			}
		}

		void setNoticeMsg(string teamName, string msg, Color color, bool willClearMsg = false) {
			int msgMaxLength = 34;
			if (teamName == "Team") {
				teamNoticeText.color = color;
				teamNoticeText.DOKill();
//				teamNoticeText.DOText(teamNoticeText.text + msg, 0.5f);
				if (willClearMsg) {
					teamNoticeText.text = "";
				}
				teamNoticeText.text += msg;
				if (teamNoticeText.text.Length > msgMaxLength) {
					teamNoticeText.text = teamNoticeText.text.Remove(0, teamNoticeText.text.Length - msgMaxLength);
				}
				teamNoticeText.DOFade(1, 0);
				teamNoticeText.DOFade(0, 0.5f).SetDelay(2).OnComplete(() => {
					teamNoticeText.text = "";
				});
			}
			else {
				enemyNoticeText.color = color;
				enemyNoticeText.DOKill();
//				enemyNoticeText.DOText(enemyNoticeText.text + msg, 0.5f);
				if (willClearMsg) {
					enemyNoticeText.text = "";
				}
				enemyNoticeText.text += msg;
				if (enemyNoticeText.text.Length > msgMaxLength) {
					enemyNoticeText.text = enemyNoticeText.text.Remove(0, enemyNoticeText.text.Length - msgMaxLength);
				}
				enemyNoticeText.DOFade(1, 0);
				enemyNoticeText.DOFade(0, 0.5f).SetDelay(2).OnComplete(() => {
					enemyNoticeText.text = "";
				});
			}
		}

		public void Finish() {
			Pause();
			canTeamDoSkill = false;
			canEnemyDoSkill = false;
		}

		public void Pause() {
			playing = false;
			Messenger.Broadcast<bool>(NotifyTypes.MakeRoleInfoPanelDisable, true);
		}

		public void Play(float delay = 0, System.Action callback = null) {
			if (delay <= 0) {
				playing = true;
				Messenger.Broadcast<bool>(NotifyTypes.MakeRoleInfoPanelDisable, false);
			}
			else {
				Pause();
				playDelayDate = Time.fixedTime;
				playDelayTimeout = delay;
				playDelayCallback = callback;
			}
		}

        public void UpdateData(RoleData currentRole, FightData fight) {
            teamBuffs.Clear();
            enemyBuffs.Clear();
			ending = false;
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
			maxEnemyNum = enemyRoleDatas.Count;
			callEnemy();
			curRound = maxRound;
		}

		public override void RefreshView () {
			canvasGroup.alpha = 0;
			teamWeaponGroup.DOFade(1, 0);
			enemyWeaponGroup.DOFade(1, 0);
			winSprite.DOFade(0, 0);
			failSprite.DOFade(0, 0);
			Finish();
			reStartTeam();
			reStartEnemy();
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
			SoundManager.GetInstance().PlayBGM("bgm0004");
			refreshRound();
		}

		public void FadeOut() {
			Finish();
			canvasGroup.DOFade(0, 0.5f).SetAutoKill(false).OnComplete(() => {
				Close();
			});
		}

        /// <summary>
        /// 移除抗性buff
        /// </summary>
        /// <param name="teamName">Team name.</param>
        /// <param name="role">Role.</param>
        void removeResistanceBuff(string teamName, RoleData role) {
            List<BuffData> buffList = teamName == "Team" ? teamBuffs : enemyBuffs;
            BookData book;
            int removeIndex;
            for (int i = 0, len = role.Books.Count; i < len; i++) {
                book = role.Books[i];
                if (book.DrugResistance > 0) { //移除毒抗
                    removeIndex = buffList.FindIndex(item => item.Id == "drugResistanceBuff");
                    if (removeIndex >= 0) {
                        buffList.RemoveAt(removeIndex);
                    }
                }
                if (book.DisarmResistance > 0) { //移除缴械抗
                    removeIndex = buffList.FindIndex(item => item.Id == "disarmResistanceBuff");
                    if (removeIndex >= 0) {
                        buffList.RemoveAt(removeIndex);
                    }
                }
                if (book.VertigoResistance > 0) { //移除晕抗
                    removeIndex = buffList.FindIndex(item => item.Id == "vertigoResistanceBuff");
                    if (removeIndex >= 0) {
                        buffList.RemoveAt(removeIndex);
                    }
                }
                if (book.CanNotMoveResistance > 0) { //移除定抗
                    removeIndex = buffList.FindIndex(item => item.Id == "canNotMoveResistanceBuff");
                    if (removeIndex >= 0) {
                        buffList.RemoveAt(removeIndex);
                    }
                }
                if (book.SlowResistance > 0) { //移除缓抗
                    removeIndex = buffList.FindIndex(item => item.Id == "slowResistanceBuff");
                    if (removeIndex >= 0) {
                        buffList.RemoveAt(removeIndex);
                    }
                }
                if (book.ChaosResistance > 0) { //移除乱抗
                    removeIndex = buffList.FindIndex(item => item.Id == "chaosResistanceBuff");
                    if (removeIndex >= 0) {
                        buffList.RemoveAt(removeIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 添加抗性buff
        /// </summary>
        /// <param name="teamName">Team name.</param>
        /// <param name="role">Role.</param>
        void addResistanceBuff(string teamName, RoleData role) {
            List<BuffData> buffList = teamName == "Team" ? teamBuffs : enemyBuffs;
            BookData book;
            BuffData buff;
            for (int i = 0, len = role.Books.Count; i < len; i++) {
                book = role.Books[i];
                if (book.DrugResistance > 0) { //添加毒抗
                    buff = new BuffData();
                    buff.Id = "drugResistanceBuff";
                    buff.Type = BuffType.DrugResistance;
                    buff.RoundNumber = book.DrugResistance;
                    buffList.Add(buff);
                }
                if (book.DisarmResistance > 0) { //添加缴械抗
                    buff = new BuffData();
                    buff.Id = "disarmResistanceBuff";
                    buff.Type = BuffType.DisarmResistance;
                    buff.RoundNumber = book.DisarmResistance;
                    buffList.Add(buff);
                }
                if (book.VertigoResistance > 0) { //添加晕抗
                    buff = new BuffData();
                    buff.Id = "vertigoResistanceBuff";
                    buff.Type = BuffType.VertigoResistance;
                    buff.RoundNumber = book.VertigoResistance;
                    buffList.Add(buff);
                }
                if (book.CanNotMoveResistance > 0) { //添加定抗
                    buff = new BuffData();
                    buff.Id = "canNotMoveResistanceBuff";
                    buff.Type = BuffType.CanNotMoveResistance;
                    buff.RoundNumber = book.CanNotMoveResistance;
                    buffList.Add(buff);
                }
                if (book.SlowResistance > 0) { //添加缓抗
                    buff = new BuffData();
                    buff.Id = "slowResistanceBuff";
                    buff.Type = BuffType.SlowResistance;
                    buff.RoundNumber = book.SlowResistance;
                    buffList.Add(buff);
                }
                if (book.ChaosResistance > 0) { //添加乱抗
                    buff = new BuffData();
                    buff.Id = "chaosResistanceBuff";
                    buff.Type = BuffType.ChaosResistance;
                    buff.RoundNumber = book.ChaosResistance;
                    buffList.Add(buff);
                }
            }
        }

		public void UpdateCurrentTeamRole(RoleData currentRole) {
			if (!ending && currentRole != null) {
                //清空旧的抗性buff
                if (currentTeamRole != null) {
                    removeResistanceBuff("Team", currentTeamRole);
                }
				currentTeamRole = currentRole;
                //添加新的抗性buff
                currentTeamRole.Init();
                addResistanceBuff("Team", currentTeamRole);
				removeGuoJin();
				guoJinSkillObj = Statics.GetPrefabClone(Statics.GuoJingPrefab);
				guoJinSkillObj.transform.position = new Vector3(0.2f, 0, -8.5f);
				Play(1.8f, () => {
					reStartTeam();
				});
				pushNotice("Team", " " + currentTeamRole.Name + "出战", true);
				UpdateCurrentTeamBookIndex(currentTeamRole.SelectedBookIndex);
			}
		}

		public void UpdateCurrentTeamBookIndex(int index) {
            if (!ending && currentTeamRole != null && currentTeamRole.Books.Count > index) {
                currentTeamRole.SelectBook(index);
                currentTeamBook = currentTeamRole.GetCurrentBook();
                currentTeamSkill = currentTeamBook.GetCurrentSkill();

                teamGotSkills.Clear();
//				teamGotSkills.SetIconIds(new List<string>() { "300000", "300000", "300000", "300000" });
                teamGotSkills.SetIconIds(currentTeamBook.GetSkillIconIds());
                resetSkillAndWeaponPosition("Team");
                reStartTeam();
                pushNotice("Team", " 切换" + currentTeamBook.Name);
                //处理当前招式icon
                teamCurSkillIconImage.sprite = Statics.GetIconSprite(currentTeamBook.GetCurrentSkill().IconId);
            } else {
                teamCurSkillIconImage.sprite = Statics.GetIconSprite("700000");
            }
		}

		void refreshTeamHP() {
			teamBloodProgress.rectTransform.sizeDelta = new Vector2(currentTeamRole.HPRate * 556, teamBloodProgress.rectTransform.sizeDelta.y);
			teamBloodText.text = string.Format("{0}/{1}", currentTeamRole.HP, currentTeamRole.MaxHP);
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

		void callEnemy() {
			if (currentEnemyRole != null) {
                //清空旧的抗性buff
                removeResistanceBuff("Enemy", currentEnemyRole);
				pushNotice("Enemy", " " + currentEnemyRole.Name + "落败", true);
			}
            currentEnemyRole = popEnemy();
			if (currentEnemyRole != null) {
				pushNotice("Enemy", " " + currentEnemyRole.Name + "出战");
                currentEnemyRole.Init();
                //添加新的抗性buff
                addResistanceBuff("Enemy", currentEnemyRole);
				currentEnemyBook = currentEnemyRole.Books.Count > 0 ? currentEnemyRole.Books[0] : null;
				enemyGotSkills.Clear();
                if (currentEnemyBook != null) {
                    enemyGotSkills.SetIconIds(currentEnemyBook.GetSkillIconIds());
                    //处理当前招式icon
                    enemyCurSkillIconImage.sprite = Statics.GetIconSprite(currentEnemyBook.GetCurrentSkill().IconId);
                } else {
                    enemyCurSkillIconImage.sprite = Statics.GetIconSprite("700000");
                }
			}
		}

		RoleData popEnemy() {
			if (enemyRoleDatas.Count > 0) {
				battleProgressText.text = string.Format("{0}/{1}", enemyRoleDatas.Count, maxEnemyNum);
				RoleData enemy = enemyRoleDatas[0];
				enemyRoleDatas.RemoveAt(0);
				return enemy;
			}
			return null;
		}

		void refreshEnemyHP() {
			enemyBloodProgress.rectTransform.sizeDelta = new Vector2(currentEnemyRole.HPRate * 556, enemyBloodProgress.rectTransform.sizeDelta.y);
			enemyBloodText.text = string.Format("{0}/{1}", currentEnemyRole.HP, currentEnemyRole.MaxHP);
		}

		void refreshEnemyBuffs() {
			enemyGotBuffs.SetBuffDatas(enemyBuffs);
		}

		void refreshEnemyView() {
			if (currentEnemyRole != null) {
				enemyBody.sprite = Statics.GetHalfBodySprite(currentEnemyRole.HalfBodyId);
                enemyBody.SetNativeSize();
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
				InstantiateView("Prefabs/UI/MainTool/BattleMainPanelView", "BattleMainPanelCtrl");
			}
			Ctrl.UpdateData(currentRole, fight);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.FadeOut();
			}
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

		public static void MakeFaild() {
			if (Ctrl != null) {
				Ctrl.Faild();
			}
		}

		/// <summary>
		/// 当窗口关闭时调用
		/// </summary>
		void OnDestroy() {
			Timer.RemoveTimer("MakePopRoleTimer");
		}
	}
}
