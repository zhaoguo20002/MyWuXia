using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;

namespace Game {
	public class WorkshopWeaponBuildingContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Text Cost;
		public Button CreateBtn;
		public Image NewFlag;
		public Button ViewBtn;

		WeaponData weaponData;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(CreateBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(ViewBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (weaponData == null) {
				return;
			}
			switch(e.name) {
			case "CreateBtn":
				ConfirmCtrl.Show(string.Format("开始打造<color=\"{0}\">{1}</color>?", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name), () => {
					Messenger.Broadcast<string>(NotifyTypes.CreateNewWeaponOfWorkshop, weaponData.Id);
				}, null, "打造", "放弃");
				break;
			case "ViewBtn":
				Messenger.Broadcast<WeaponData>(NotifyTypes.ShowWeaponDetailPanel, weaponData);
				break;
			default:
				break;
			}
			viewedNewFlag();
		}

		void viewedNewFlag() {
			if (NewFlag.gameObject.activeSelf) {
				PlayerPrefs.SetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "WeaponIdOfWorkShopNewFlagIsHide_" + weaponData.Id, "true"); //让新增提示消失
				NewFlag.gameObject.SetActive(false);
			}
		}

		public void UpdateData(WeaponData weapon) {
			weaponData = weapon;
		}

		public void RefreshView() {
			if (weaponData == null) {
				return;
			}
			Icon.sprite = Statics.GetIconSprite(weaponData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name);
			string costStr = "";
			ResourceData need;
			for (int i = 0; i < weaponData.Needs.Count; i++) {
				need = weaponData.Needs[i];
				costStr += string.Format("{0}-{1}\n", Statics.GetResourceName(need.Type), need.Num);
			}
			Cost.text = costStr;
			//判断是否为新增兵器，控制新增标记显示隐藏
			NewFlag.gameObject.SetActive(string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "WeaponIdOfWorkShopNewFlagIsHide_" + weaponData.Id)));
		}

	}
}