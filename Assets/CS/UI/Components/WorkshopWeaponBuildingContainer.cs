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

		WeaponData weaponData;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(CreateBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "CreateBtn":
				
				break;
			default:
				break;
			}
		}

		public void UpdateData(WeaponData weapon) {
			weaponData = weapon;
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(weaponData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name);
			string costStr = "";
			ResourceData need;
			for (int i = 0; i < weaponData.Needs.Count; i++) {
				need = weaponData.Needs[i];
				costStr += string.Format("{0}-{1}\n", Statics.GetResourceName(need.Type), need.Num);
			}
			Cost.text = costStr;
		}

	}
}