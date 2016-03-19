using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class WeaponItemContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Image WeaponWidth;
		public Text PowerIndexDesc0;
		public Text PowerIndexDesc1;
		public Text PowerIndexDesc2;
		public Image PowerIndexFlag0;
		public Image PowerIndexFlag1;
		public Image PowerIndexFlag2;
		public Button Btn;

		WeaponData weaponData;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			
		}
		
		public void UpdateData(WeaponData weapon) {
			weaponData = weapon;
		}
		
		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(weaponData.IconId);
			Name.text = weaponData.Name;
			Btn.gameObject.SetActive(weaponData.BeUsingByRoleId == "");
		}
		
	}
}
