using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class RoleOfHospitalContainer : ComponentCore {
		public Image Icon;
		public Text Name;
		public Image InjuryImage;
		public Button Btn;
		public Button CureBtn;

		RoleData roleData;
		WeaponData weapon;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
			EventTriggerListener.Get(CureBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
			case "CureBtn":
				if (roleData.Injury != InjuryType.None) {
					ConfirmCtrl.Show(string.Format("要想治愈{0}{1}的伤, 需要使用能够治疗{2}状态的药物, 确定治疗?", roleData.Name, Statics.GetGenderDesc(roleData.Gender), Statics.GetInjuryName(roleData.Injury)), () => {
						Messenger.Broadcast<int>(NotifyTypes.CureRole, roleData.PrimaryKeyId);
					});
				}
				break;
			case "Btn":
				Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleData);
				break;
			default:
				break;
			}
		}
		
		public void UpdateData(RoleData role) {
			roleData = role;
			roleData.MakeJsonToModel();
			weapon = roleData.Weapon;
		}
		
		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(roleData.IconId);
			Name.text = roleData.Name;
			InjuryImage.gameObject.SetActive(roleData.Injury != InjuryType.None);
			CureBtn.gameObject.SetActive(roleData.Injury != InjuryType.None);
			if (roleData.Injury != InjuryType.None) {
				InjuryImage.color = Statics.GetInjuryColor(roleData.Injury);
			}
		}
		
	}
}
