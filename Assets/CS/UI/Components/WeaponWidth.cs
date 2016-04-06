using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	public class WeaponWidth : MonoBehaviour {
		public Image WeaponPowerBg;
		public Image WeaponShowBg;
		public WeaponPowerPlus WeaponPowerPlusScript;

		WeaponData weaponData;

		public void UpdateData(WeaponData weapon) {
			weaponData = weapon;
		}

		public void RefreshView() {
			WeaponPowerBg.rectTransform.sizeDelta = new Vector2(weaponData.Width, WeaponPowerBg.rectTransform.sizeDelta.y);
			WeaponShowBg.rectTransform.sizeDelta = new Vector2(weaponData.Width + 60, WeaponShowBg.rectTransform.sizeDelta.y);
			WeaponPowerPlusScript.SetRates(weaponData.Width, weaponData.Rates);
		}
	}
}
