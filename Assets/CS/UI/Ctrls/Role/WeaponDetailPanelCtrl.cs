using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;
using UnityEngine.UI;

namespace Game {
	public class WeaponDetailPanelCtrl : WindowCore<WeaponDetailPanelCtrl, JArray> {
		Image bg;
		Button block;
        Image icon;
        Image flashImage;
		Text nameText;
//		WeaponWidth weaponWidthScript;
		Image infoBgImage;
		Text infoText;
		Image descBgImage;
		Text descText;
		Text occupationText;

		WeaponData weaponData;
		string info;
		bool initedHeight;
		protected override void Init () {
			bg = GetChildImage("Bg");
			bg.transform.DOScale(0, 0);
			block = GetChildButton("Block");
			EventTriggerListener.Get(block.gameObject).onClick = onClick;
            icon = GetChildImage("Icon");
            flashImage = GetChildImage("flashImage");
			nameText = GetChildText("NameText");
//			weaponWidthScript = GetChildComponent<WeaponWidth>(gameObject, "WeaponWidth");
			infoBgImage = GetChildImage("InfoBgImage");
			infoText = GetChildText("InfoText");
			descBgImage = GetChildImage("DescBgImage");
			descText = GetChildText("DescText");
			occupationText = GetChildText("OccupationText");
		}

		void onClick(GameObject e) {
			Back();
		}

		public void UpdateData (WeaponData weapon) {
			weaponData = weapon;
			info = "";
//			if (weaponData.Rates[3] > 0) {
//				info += string.Format("<color=\"#FF0000\">追加100%伤害概率:{0}%</color>", (int)(weaponData.Rates[3] * 100));
//			}
//			if (weaponData.Rates[2] > 0) {
//				info += info == "" ? "" : "\n";
//				info += string.Format("<color=\"#FFA300\">追加50%伤害概率:{0}%</color>", (int)(weaponData.Rates[2] * 100));
//			}
//			if (weaponData.Rates[1] > 0) {
//				info += info == "" ? "" : "\n";
//				info += string.Format("<color=\"#DBFF00\">追加25%伤害概率:{0}%</color>", (int)(weaponData.Rates[1] * 100));
//			}
			if (weaponData.FixedDamagePlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("固定伤害:{0}", (weaponData.FixedDamagePlus > 0 ? "+" : "") + weaponData.FixedDamagePlus.ToString());
			}
			if (weaponData.DamageRatePlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("最终伤害:{0}%", (weaponData.DamageRatePlus > 0 ? "+" : "") + (weaponData.DamageRatePlus * 100).ToString());
			}
			if (weaponData.PhysicsAttackPlus != 0) {
				info += info == "" ? "" : "\n";
				info += string.Format("外功:{0}", (weaponData.PhysicsAttackPlus > 0 ? "+" : "") + weaponData.PhysicsAttackPlus.ToString());
			}
//			if (weaponData.AttackSpeedPlus != 0) {
//				info += info == "" ? "" : "\n";
//				info += string.Format("攻速:{0}", (weaponData.AttackSpeedPlus > 0 ? "+" : "") + weaponData.AttackSpeedPlus.ToString());
//			}
			info = info == "" ? "无任何附加属性" : info;
		}

		public override void RefreshView () {
			icon.sprite = Statics.GetIconSprite(weaponData.IconId);
            flashImage.gameObject.SetActive(((int)weaponData.Quality) >= ((int)QualityType.FlashGold));
			nameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name);
//			weaponWidthScript.UpdateData(weaponData);
//			weaponWidthScript.RefreshView();
			infoText.text = info;
            if (weaponData.BelongToRoleId == "") {
                occupationText.text = string.Format("门派限制:{0}", weaponData.Occupation != OccupationType.None ? Statics.GetOccupationName(weaponData.Occupation) : "无限制");
            } else {
                occupationText.text = string.Format("仅限 {0} 使用", JsonManager.GetInstance().GetMapping<RoleData>("RoleDatas", weaponData.BelongToRoleId).Name);
            }
		}

		void LateUpdate() {
			//处理Bg的高度
			if (!initedHeight) {
				if (infoBgImage.gameObject.GetComponent<PerfectChildSize>() == null) {
					initedHeight = true;
					float height = Mathf.Abs(infoBgImage.rectTransform.anchoredPosition.y) + infoBgImage.rectTransform.sizeDelta.y;
					if (weaponData.Desc != "") {
						descBgImage.gameObject.SetActive(true);
						descText.text = string.Format("描述:{0}", weaponData.Desc);
						descBgImage.rectTransform.anchoredPosition = new Vector2(descBgImage.rectTransform.anchoredPosition.x, -(height + 5));
						height = Mathf.Abs(descBgImage.rectTransform.anchoredPosition.y) + descBgImage.rectTransform.sizeDelta.y;
					}
					else {
						descBgImage.gameObject.SetActive(false);
					}
					bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, height + 30);
					Pop();
				}
			}
		}

		public void Pop() {
			bg.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

		public static void Show(WeaponData weapon) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/WeaponDetailPanelView", "WeaponDetailPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
			}
			Ctrl.UpdateData(weapon);
			Ctrl.RefreshView();
		}
	}
}
