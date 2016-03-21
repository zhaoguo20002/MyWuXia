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

		Image bg;

		WeaponData weaponData;
		WeaponData hostWeaponData;
		void Awake() {
			bg = GetComponent<Image>();
		}

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			Messenger.Broadcast<int, string>(NotifyTypes.ReplaceWeapon, weaponData.PrimaryKeyId, UserModel.CurrentUserData.Id);
		}

		public void UpdateData(WeaponData weapon, WeaponData hostWeapon) {
			weaponData = weapon;
			hostWeaponData = hostWeapon;
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(weaponData.IconId);
			Name.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(weaponData.Quality), weaponData.Name);
			WeaponWidth.rectTransform.sizeDelta = new Vector2(100f * (weaponData.Width / 100f), WeaponWidth.rectTransform.sizeDelta.y);
			PowerIndexDesc0.text = string.Format("+100%伤害概率: {0}%", (int)(weaponData.Rates[1] * 100));
			PowerIndexDesc1.text = string.Format("+50%伤害概率: {0}%", (int)(weaponData.Rates[2] * 100));
			PowerIndexDesc2.text = string.Format("+25%伤害概率: {0}%", (int)(weaponData.Rates[3] * 100));
			if (weaponData.BeUsingByRoleId != "") {
				Btn.gameObject.SetActive(false);
				bg.sprite = Statics.GetSprite("Border12");
				PowerIndexFlag0.gameObject.SetActive(false);
				PowerIndexFlag1.gameObject.SetActive(false);
				PowerIndexFlag2.gameObject.SetActive(false);
			}
			else {
				Btn.gameObject.SetActive(true);
				if (weaponData.Rates[1] != hostWeaponData.Rates[1]) {
					PowerIndexFlag0.gameObject.SetActive(true);
					PowerIndexFlag0.sprite = Statics.GetSprite(weaponData.Rates[1] > hostWeaponData.Rates[1] ? "StateUp" : "StateDown");
				}
				else {
					PowerIndexFlag0.gameObject.SetActive(false);
				}

				if (weaponData.Rates[2] != hostWeaponData.Rates[2]) {
					PowerIndexFlag1.gameObject.SetActive(true);
					PowerIndexFlag1.sprite = Statics.GetSprite(weaponData.Rates[2] > hostWeaponData.Rates[2] ? "StateUp" : "StateDown");
				}
				else {
					PowerIndexFlag1.gameObject.SetActive(false);
				}
				if (weaponData.Rates[3] != hostWeaponData.Rates[3]) {
					PowerIndexFlag2.gameObject.SetActive(true);
					PowerIndexFlag2.sprite = Statics.GetSprite(weaponData.Rates[3] > hostWeaponData.Rates[3] ? "StateUp" : "StateDown");
				}
				else {
					PowerIndexFlag2.gameObject.SetActive(false);
				}
			}
		}

	}
}
