using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class RoleOfWinShopContainer : MonoBehaviour {
		public Image Icon;
		public Text Name;
		public Image Flag;

		RoleData roleData;

		// Use this for initialization
		void Start () {
			if (Icon == null || Name == null || Flag == null) {
				enabled = false;
			}
		}
		
		public void UpdateData(RoleData role) {
			roleData = role;
		}
		
		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(roleData.IconId);
			Name.text = roleData.Name;
			Flag.gameObject.SetActive(roleData.State != RoleStateType.NotRecruited);
		}
		
	}
}
