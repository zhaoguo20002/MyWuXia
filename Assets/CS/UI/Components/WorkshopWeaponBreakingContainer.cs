using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using DG;
using DG.Tweening;

namespace Game {
	public class WorkshopWeaponBreakingContainer : MonoBehaviour {
		public int PrimaryKeyId;
		public Image Icon;
		public Text Name;
		public Text Got;
		public Button BreakBtn;

		WeaponData weaponData;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(BreakBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (weaponData == null) {
				return;
			}
			switch(e.name) {
			case "BreakBtn":
				ConfirmCtrl.Show(string.Format("确定将<color=\"{0}\">{1}</color>熔解?\n<color=\"#FF0000\">熔解后此兵器将永久消失</color>!", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name), () => {
					Messenger.Broadcast<int>(NotifyTypes.BreakWeapon, weaponData.PrimaryKeyId);
				}, null, "熔解", "放弃");
				break;
			default:
				break;
			}
		}

		public void UpdateData(WeaponData weapon) {
			weaponData = weapon;
			PrimaryKeyId = weaponData.PrimaryKeyId;
		}

		public void RefreshView() {
			if (weaponData == null) {
				return;
			}
			BreakBtn.gameObject.SetActive(weaponData.BeUsingByRoleId == "");
			Icon.sprite = Statics.GetIconSprite(weaponData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name);
			string gotStr = "";
			ResourceData need;
			for (int i = 0; i < weaponData.Needs.Count; i++) {
				need = weaponData.Needs[i];
				if (need.Num > 0) {
					gotStr += string.Format("{0}+{1}\n", Statics.GetResourceName(need.Type), need.Num);
				}
			}
			gotStr = gotStr == "" ? "无" : gotStr;
			Got.text = gotStr;
		}

	}
}