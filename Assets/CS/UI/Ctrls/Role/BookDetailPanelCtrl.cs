﻿using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using System.Collections.Generic;

namespace Game {
	public class BookDetailPanelCtrl : WindowCore<BookDetailPanelCtrl, JArray> {
		Image bg;
		Button block;
		Image iconImage;
        Image flashImage;
		Text nameText;
		Text descText;
//		RectTransform gridTrans;
//		Image emptyImage;
        Image descBgImage;
        Text titleText;
        Text secretsDescText;
        Button studyBtn;
        Text studyText;

		BookData bookData;
        ExpAndSecretData expAndSecretData;
		Object prefabObj;
//		List<SkillItemContainer> containers;
//		bool initedGrid;
		string info;
        string secretInfo;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			iconImage = GetChildImage("IconImage");
            flashImage = GetChildImage("flashImage");
			nameText = GetChildText("NameText");
			descText = GetChildText("DescText");
//			gridTrans = GetChildComponent<RectTransform>(gameObject, "Grid");
//			emptyImage = GetChildImage("emptyImage");
//			containers = new List<SkillItemContainer>();
//			initedGrid = false;
            descBgImage = GetChildImage("descBgImage");
            titleText = GetChildText("titleText");
            secretsDescText = GetChildText("secretsDescText");
            studyBtn = GetChildButton("studyBtn");
            EventTriggerListener.Get(studyBtn.gameObject).onClick = onClick;
            studyText = GetChildText("studyText");
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "studyBtn":
                    if (studyBtn.enabled)
                    {
                        Messenger.Broadcast<BookData, List<SecretData>>(NotifyTypes.GetSecretListPanelData, bookData, expAndSecretData.Secrets);
                    }
                    break;
                default:
                    Back();
                    break;
            }
		}

		public void Pop() {
			bg.transform.DOScale(0, 0);
			bg.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

        public void UpdateData(BookData book, ExpAndSecretData expAndSecret) {
			bookData = book;
            bookData.MakeJsonToModel();
            expAndSecretData = expAndSecret;
			info = "";
			if (bookData.MaxHPPlus != 0) {
				info += string.Format("最大气血:{0}", (bookData.MaxHPPlus > 0 ? "+" : "") + bookData.MaxHPPlus.ToString());
			}
			if (bookData.PhysicsDefensePlus != 0) {
				info += info == "" ? "" : ", ";
				info += string.Format("外防:{0}", (bookData.PhysicsDefensePlus > 0 ? "+" : "") + bookData.PhysicsDefensePlus.ToString());
			}
			if (bookData.MagicAttackPlus != 0) {
				info += info == "" ? "" : ", ";
				info += string.Format("内功:{0}", (bookData.MagicAttackPlus > 0 ? "+" : "") + bookData.MagicAttackPlus.ToString());
			}
			if (bookData.MagicDefensePlus != 0) {
				info += info == "" ? "" : ", ";
				info += string.Format("内防:{0}", (bookData.MagicDefensePlus > 0 ? "+" : "") + bookData.MagicDefensePlus.ToString());
			}
			if (bookData.DodgePlus != 0) {
				info += info == "" ? "" : ", ";
				info += string.Format("轻功:{0}", (bookData.DodgePlus > 0 ? "+" : "") + bookData.DodgePlus.ToString());
			}
			if (bookData.HurtCutRatePlus != 0) {
				info += info == "" ? "" : ", ";
				info += string.Format("减伤:{0}%", (bookData.HurtCutRatePlus > 0 ? "+" : "") + (bookData.HurtCutRatePlus * 100).ToString());
            }
            if (bookData.DrugResistance > 0) {
                info += info == "" ? "" : ", ";
                info += string.Format("抗中毒:抵消{0}秒", bookData.DrugResistance);
            }
            if (bookData.DisarmResistance > 0) {
                info += info == "" ? "" : ", ";
                info += string.Format("抗缴械:抵消{0}秒", bookData.DisarmResistance);
            }
            if (bookData.CanNotMoveResistance > 0) {
                info += info == "" ? "" : ", ";
                info += string.Format("抗定身:抵消{0}秒", bookData.CanNotMoveResistance);
            }
            if (bookData.VertigoResistance > 0) {
                info += info == "" ? "" : ", ";
                info += string.Format("抗眩晕:抵消{0}秒", bookData.VertigoResistance);
            }
            if (bookData.SlowResistance > 0) {
                info += info == "" ? "" : ", ";
                info += string.Format("抗迟缓:抵消{0}秒", bookData.SlowResistance);
            }
            if (bookData.ChaosResistance > 0) {
                info += info == "" ? "" : ", ";
                info += string.Format("抗混乱:抵消{0}秒", bookData.ChaosResistance);
            }
            secretInfo = "";
            if (expAndSecretData.Secrets.Count > 0)
            {
                for (int i = 0, len = expAndSecretData.Secrets.Count; i < len; i++)
                {
                    secretInfo += expAndSecretData.Secrets[i].GetDesc();
                    if (i < len - 1)
                    {
                        secretInfo += ", ";
                    }
                }
            }
		}

		public override void RefreshView () {
			iconImage.sprite = Statics.GetIconSprite(bookData.IconId);
            flashImage.gameObject.SetActive(((int)bookData.Quality) >= ((int)QualityType.FlashGold));
			nameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
            descText.text = string.Format("{0}{1}{2}{3}\n描述:\n{4}", 
                bookData.GetCurrentSkill() != null ? bookData.GetCurrentSkill().Desc : "心法无招式", 
                bookData.LimitWeaponType != WeaponType.None ? string.Format("\n兵器限制:{0}", Statics.GetEnmuDesc<WeaponType>(bookData.LimitWeaponType)) : "", 
                info != "" ? string.Format("\n附加属性:\n<color=\"#00FF00\">{0}</color>", info) : "", 
                secretInfo != "" ? string.Format("\n诀要加成属性:\n<color=\"#00FF00\">{0}</color>", secretInfo) : "", 
                !string.IsNullOrEmpty(bookData.Desc) ? bookData.Desc : "无");
            if (bookData.IsMindBook)
            {
                titleText.text = "心法";
            }
            else if (bookData.IsLostKnowledge)
            {
                titleText.text = "绝学";
            }
            else
            {
                titleText.text = "秘籍";
            }
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/SkillItemContainer");
			}
            StartCoroutine(refreshHeight());
            if (!bookData.IsMindBook)
            {
                secretsDescText.text = string.Format("修为:\n{0}/{1}", expAndSecretData.Exp.Cur, expAndSecretData.Exp.Max > 0 ? Statics.GetBookStepExp(expAndSecretData.Exp.Cur) : 0);
                studyText.text = string.Format("领悟:{0}/{1}", expAndSecretData.Secrets.Count, Statics.GetBookLV(expAndSecretData.Exp.Cur));
                MakeButtonEnable(studyBtn, true);
            }
            else
            {
                secretsDescText.text = "修为:\n不可修炼";
                studyText.text = "不可领悟";
                MakeButtonEnable(studyBtn, false);
            }
//			if (bookData.Skills.Count > 0) {
//				emptyImage.gameObject.SetActive(false);
//				SkillData skill;
//				GameObject itemPrefab;
//				SkillItemContainer container;
//				for (int i = 0; i < bookData.Skills.Count; i++) {
//					skill = bookData.Skills[i];
//					if (containers.Count <= i) {
//						itemPrefab = Statics.GetPrefabClone(prefabObj);
//						MakeToParent(gridTrans.transform, itemPrefab.transform);
//						container = itemPrefab.GetComponent<SkillItemContainer>();
//						containers.Add(container);
//					}
//					else {
//						container = containers[i];
//					}
//					container.UpdateData(skill, i != bookData.Skills.Count - 1);
//					container.RefreshView();
//				}
//			}
//			else {
//				emptyImage.gameObject.SetActive(true);
//			}
		}

//		void Update() {
//			//处理grid的高度
//			if (!initedGrid) {
//				if (containers.Count > 0) {
//					if (containers[0].GetComponent<PerfectChildSize>() == null) {
//						initedGrid = true;
//						RectTransform rect;
//						List<RectTransform> rects = new List<RectTransform>();
//						float gridHeight = 0;
//						for (int i = 0; i < containers.Count; i++) {
//							rect = containers[i].GetComponent<RectTransform>();
//							rect.anchoredPosition = new Vector2(0, -gridHeight);
//							gridHeight += rect.sizeDelta.y + 18;
//							rects.Add(rect);
//						}
//						gridHeight -= 18;
//						gridTrans.sizeDelta = new Vector2(gridTrans.sizeDelta.x, gridHeight);
//					}
//				}
//			}
//		}

        IEnumerator refreshHeight() {
            yield return null;
            bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, bg.rectTransform.sizeDelta.y + Mathf.Clamp(descBgImage.rectTransform.sizeDelta.y - 300, 0, 800));
        }

        public static void Show(BookData book, ExpAndSecretData expAndSecret) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/BookDetailPanelView", "BookDetailPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
            Ctrl.UpdateData(book, expAndSecret);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}

	}
}
