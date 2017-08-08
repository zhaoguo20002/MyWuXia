using UnityEngine;
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
		Text nameText;
		Text descText;
//		RectTransform gridTrans;
//		Image emptyImage;
        Image descBgImage;

		BookData bookData;
		Object prefabObj;
//		List<SkillItemContainer> containers;
//		bool initedGrid;
		string info;
		protected override void Init () {
			bg = GetChildImage("Bg");
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
			iconImage = GetChildImage("IconImage");
			nameText = GetChildText("NameText");
			descText = GetChildText("DescText");
//			gridTrans = GetChildComponent<RectTransform>(gameObject, "Grid");
//			emptyImage = GetChildImage("emptyImage");
//			containers = new List<SkillItemContainer>();
//			initedGrid = false;
            descBgImage = GetChildImage("descBgImage");
		}

		void onClick(GameObject e) {
			Back();
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

		public void UpdateData(BookData book) {
			bookData = book;
            bookData.MakeJsonToModel();
			info = "";
			if (bookData.MaxHPPlus != 0) {
				info += string.Format("最大气血:{0}", (bookData.MaxHPPlus > 0 ? "+" : "") + bookData.MaxHPPlus.ToString());
			}
			if (bookData.PhysicsDefensePlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("外防:{0}", (bookData.PhysicsDefensePlus > 0 ? "+" : "") + bookData.PhysicsDefensePlus.ToString());
			}
			if (bookData.MagicAttackPlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("内功:{0}", (bookData.MagicAttackPlus > 0 ? "+" : "") + bookData.MagicAttackPlus.ToString());
			}
			if (bookData.MagicDefensePlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("内防:{0}", (bookData.MagicDefensePlus > 0 ? "+" : "") + bookData.MagicDefensePlus.ToString());
			}
			if (bookData.DodgePlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("轻功:{0}", (bookData.DodgePlus > 0 ? "+" : "") + bookData.DodgePlus.ToString());
			}
			if (bookData.HurtCutRatePlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("减伤:{0}%", (bookData.HurtCutRatePlus > 0 ? "+" : "") + (bookData.HurtCutRatePlus * 100).ToString());
            }
            if (bookData.DrugResistance > 0) {
                info += info == "" ? "" : "\n";
                info += string.Format("抗中毒:持续{0}秒", bookData.DrugResistance);
            }
            if (bookData.DisarmResistance > 0) {
                info += info == "" ? "" : "\n";
                info += string.Format("抗缴械:持续{0}秒", bookData.DisarmResistance);
            }
            if (bookData.CanNotMoveResistance > 0) {
                info += info == "" ? "" : "\n";
                info += string.Format("抗定身:持续{0}秒", bookData.CanNotMoveResistance);
            }
            if (bookData.VertigoResistance > 0) {
                info += info == "" ? "" : "\n";
                info += string.Format("抗眩晕:持续{0}秒", bookData.VertigoResistance);
            }
            if (bookData.SlowResistance > 0) {
                info += info == "" ? "" : "\n";
                info += string.Format("抗迟缓:持续{0}秒", bookData.SlowResistance);
            }
            if (bookData.ChaosResistance > 0) {
                info += info == "" ? "" : "\n";
                info += string.Format("抗混乱:持续{0}秒", bookData.ChaosResistance);
            }
		}

		public override void RefreshView () {
			iconImage.sprite = Statics.GetIconSprite(bookData.IconId);
			nameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(bookData.Quality), bookData.Name);
            descText.text = string.Format("{0}\n{1}{2}\n描述:\n{3}", bookData.GetCurrentSkill() != null ? bookData.GetCurrentSkill().Desc : "心法无招式", bookData.LimitWeaponType != WeaponType.None ? string.Format("兵器限制:{0}\n", Statics.GetEnmuDesc<WeaponType>(bookData.LimitWeaponType)) : "", info != "" ? string.Format("附加属性:\n<color=\"#00FF00\">{0}</color>", info) : "", !string.IsNullOrEmpty(bookData.Desc) ? bookData.Desc : "无");
		
			if (prefabObj == null) {
				prefabObj = Statics.GetPrefab("Prefabs/UI/GridItems/SkillItemContainer");
			}
            StartCoroutine(refreshHeight());
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
            bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, bg.rectTransform.sizeDelta.y + Mathf.Clamp(descBgImage.rectTransform.sizeDelta.y - 180, 0, 500));
        }

		public static void Show(BookData book) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/BookDetailPanelView", "BookDetailPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
				Ctrl.Pop();
			}
			Ctrl.UpdateData(book);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Back();
			}
		}

	}
}
