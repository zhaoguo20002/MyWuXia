using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class ReadyToTraveRoleContainer : MonoBehaviour {
		public Image Icon;
		public Text Desc;
		public Text WeaponNameText;
		public Image WeaponWidth;
		public Image WeaponIcon;
		public Image BookIcon0;
		public Image BookIcon1;
		public Image BookIcon2;
		public Button SelectBtn;
		public Button CancelBtn;

		Image bg;

		RoleData roleData;
		void Awake() {
			bg = GetComponent<Image>();
		}

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(SelectBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(CancelBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "SelectBtn":

				break;
			case "CancelBtn":

				break;
			default:
				break;
			}
		}

		public void UpdateData(RoleData role) {
			roleData = role;
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(roleData.IconId);
		}

	}
}
