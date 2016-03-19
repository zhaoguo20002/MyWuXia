using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class RoleOfWinShopContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Image Flag;
		public Button Btn;

		RoleData roleData;
		WeaponData weapon;

		// Use this for initialization
		void Start () {
			if (Icon == null || Name == null || Flag == null || Btn == null) {
				enabled = false;
			}
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (roleData.State == RoleStateType.NotRecruited) {
				ConfirmCtrl.Show(string.Format("是否将<color=\"{0}\">{1}</color>赠给{2}与其结交？", Statics.GetQualityColorString(weapon.Quality), weapon.Name, roleData.Name), () => {
					Debug.LogWarning("结交");
				});
			}
			else {
				AlertCtrl.Show(string.Format("{0}已经和你结交", roleData.Name), null);
			}
		}
		
		public void UpdateData(RoleData role) {
			roleData = role;
			weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", roleData.ResourceWeaponDataId);
		}
		
		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(roleData.IconId);
			Name.text = roleData.Name;
			Flag.gameObject.SetActive(roleData.State != RoleStateType.NotRecruited);
		}
		
	}
}
