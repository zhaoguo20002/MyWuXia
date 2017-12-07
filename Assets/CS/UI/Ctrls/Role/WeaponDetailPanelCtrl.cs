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
        RectTransform toolGridRect;
        Text lvFullNoticeText;
        Button lvUpgradeBtn;

		WeaponData weaponData;
        WeaponLVData weaponLVData;
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
            toolGridRect = GetChildComponent<RectTransform>(gameObject, "toolGrid");
            lvFullNoticeText = GetChildText("lvFullNoticeText");
            lvUpgradeBtn = GetChildButton("lvUpgradeBtn");
            EventTriggerListener.Get(lvUpgradeBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
            switch (e.name)
            {
                case "Block":
                    Back();
                    break;
                case "lvUpgradeBtn":
                    if (weaponData.LV >= weaponLVData.MaxLV)
                    {
                        AlertCtrl.Show(string.Format("<color=\"{0}\">{1}</color>强化度已到满级", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name));
                        break;
                    }
                    string needMsg = "";
                    double needRate = DbManager.Instance.GetWeaponNeedRate(weaponData.LV + 1);
                    for (int i = 0, len = weaponData.Needs.Count; i < len; i++)
                    {
                        needMsg += string.Format("{0}个{1}", weaponData.Needs[i].Num * needRate, Statics.GetEnmuDesc<ResourceType>(weaponData.Needs[i].Type)) + (i < len - 1 ? "," : "");
                    }
                    ConfirmCtrl.Show(string.Format("将<color=\"{0}\">{1}</color>强化度+{2}\n需要{3}\n是否立即锻造兵器？", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name, weaponData.LV + 1, needMsg), () => {
                        Messenger.Broadcast<WeaponData>(NotifyTypes.WeaponLVUpgrade, weaponData);
                    });
                    break;
                default:
                    break;
            }
		}

        public void UpdateData (WeaponData weapon, WeaponLVData lvData) {
			weaponData = weapon;
            UpdateData(lvData);
		}

        public void UpdateData(WeaponLVData lvData) {
            weaponLVData = lvData;
            weaponData.Init(weaponLVData.LV);
            updateDesc();
        }

        void updateDesc() {
            info = "";
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
            info = info == "" ? "无任何附加属性" : info;
            string weaponBuffDesc = weaponData.GetBuffDesc();
            info += string.Format("{0}\n<color=\"#DDDDDD\">描述:{1}</color>", weaponBuffDesc != "" ? ("\n<color=\"#FFFF00\">附加效果:" + weaponBuffDesc + "</color>") : "", weaponData.Desc);
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
            lvFullNoticeText.gameObject.SetActive(weaponData.Quality >= QualityType.FlashGold && weaponData.LV >= weaponLVData.MaxLV);
            lvUpgradeBtn.gameObject.SetActive(weaponData.Quality >= QualityType.FlashGold && weaponData.LV < weaponLVData.MaxLV);
            StartCoroutine(refreshHeight());
		}

        IEnumerator refreshHeight() {
            yield return new WaitForEndOfFrame();
            float height = Mathf.Abs(infoBgImage.rectTransform.anchoredPosition.y) + infoBgImage.rectTransform.sizeDelta.y + toolGridRect.sizeDelta.y;
            bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, height + 60);
            Pop();
        }

//		void LateUpdate() {
//			//处理Bg的高度
//			if (!initedHeight) {
//				if (infoBgImage.gameObject.GetComponent<PerfectChildSize>() == null) {
//					initedHeight = true;
//					float height = Mathf.Abs(infoBgImage.rectTransform.anchoredPosition.y) + infoBgImage.rectTransform.sizeDelta.y;
//					if (weaponData.Desc != "") {
//						descBgImage.gameObject.SetActive(true);
//                        string weaponBuffDesc = weaponData.GetBuffDesc();
//                        descText.text = string.Format("描述:{0}{1}", weaponData.Desc, weaponBuffDesc != "" ? ("\n附加效果:" + weaponBuffDesc) : "");
//						descBgImage.rectTransform.anchoredPosition = new Vector2(descBgImage.rectTransform.anchoredPosition.x, -(height + 5));
//						height = Mathf.Abs(descBgImage.rectTransform.anchoredPosition.y) + descBgImage.rectTransform.sizeDelta.y;
//					}
//					else {
//						descBgImage.gameObject.SetActive(false);
//					}
//					bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, height + 30);
//					Pop();
//				}
//			}
//		}

		public void Pop() {
			bg.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
		}

		public void Back() {
			bg.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
				Close();
			});
		}

        public static void Show(WeaponData weapon, WeaponLVData lvData) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/Role/WeaponDetailPanelView", "WeaponDetailPanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
			}
            Ctrl.UpdateData(weapon, lvData);
			Ctrl.RefreshView();
		}

        public static void WeaponLVUpgrade(WeaponLVData lvData) {
            if (Ctrl != null)
            {
                Ctrl.UpdateData(lvData);
                Ctrl.RefreshView();
            }
        }
	}
}
