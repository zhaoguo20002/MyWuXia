using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class RoleOfWinShopContainer : ComponentCore {
		public Image Icon;
		public Text Name;
		public Image Flag;
		public Image NewFlag;
		public Button Btn;
		public Button MakeBtn;

		RoleData roleData;
		WeaponData weapon;

		// Use this for initialization
		void Start () {
			if (Icon == null || Name == null || Flag == null || Btn == null) {
				enabled = false;
			}
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
			EventTriggerListener.Get(MakeBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
			switch(e.name) {
			case "MakeBtn":
				if (roleData.State == RoleStateType.NotRecruited) {
					ConfirmCtrl.Show(string.Format("是否将<color=\"{0}\">{1}</color>赠给{2}与其结交？", Statics.GetQualityColorString(weapon.Quality), weapon.Name, roleData.Name), () => {
						Messenger.Broadcast<int>(NotifyTypes.InviteRole, roleData.PrimaryKeyId);
					});
				}
				else {
					AlertCtrl.Show(string.Format("{0}已经和你结交", roleData.Name), null);
				}
				break;
			case "Btn":
				Messenger.Broadcast<RoleData>(NotifyTypes.ShowRoleDetailPanel, roleData);
				break;
			default:
				break;
			}
			viewedNewFlag();
		}

		void viewedNewFlag() {
			if (NewFlag.gameObject.activeSelf) {
				PlayerPrefs.SetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "RoleOfWinShopNewFlagIsHide_" + roleData.Id, "true"); //让新增提示消失
				NewFlag.gameObject.SetActive(false);
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
			Flag.gameObject.SetActive(roleData.State != RoleStateType.NotRecruited);
//			MakeBtn.gameObject.SetActive(roleData.State == RoleStateType.NotRecruited);
			MakeButtonEnable(MakeBtn, roleData.State == RoleStateType.NotRecruited);
			//判断是否为新增侠客，控制新增标记显示隐藏
			NewFlag.gameObject.SetActive(string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefs.GetString("CurrentRoleId") + "_" + "RoleOfWinShopNewFlagIsHide_" + roleData.Id)));
		}
		
	}
}
