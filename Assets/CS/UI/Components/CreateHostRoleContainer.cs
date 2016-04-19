using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game {
	public class CreateHostRoleContainer : MonoBehaviour {
		public Toggle[] IconToggles;
		public Image[] IconImages;
		public Image[] MaskImages;
		public InputField NameInputField;
		public Button RandomBtn;
		public Button CreateBtn;

		List<string> firstNames;
		List<string> secondNames;
		string firstName;
		string secondName;
		int iconIndex = 0;
		string hostRoleId;
		GenderType genderType;

		// Use this for initialization
		void Start () {
			IconToggles[0].onValueChanged.AddListener(onChange0);
			IconToggles[1].onValueChanged.AddListener(onChange1);
			IconToggles[2].onValueChanged.AddListener(onChange2);
			IconToggles[3].onValueChanged.AddListener(onChange3);
			IconToggles[4].onValueChanged.AddListener(onChange4);
			EventTriggerListener.Get(RandomBtn.gameObject).onClick = onClick;
			EventTriggerListener.Get(CreateBtn.gameObject).onClick = onClick;
		}

		void onChange0(bool check) {
			iconIndex = 0;
			for (int i = 0; i < MaskImages.Length; i++) {
				if (i == 0) {
					MaskImages[i].gameObject.SetActive(true);
				}
				else {
					MaskImages[i].gameObject.SetActive(false);
				}
			}
		}

		void onChange1(bool check) {
			iconIndex = 1;
			for (int i = 0; i < MaskImages.Length; i++) {
				if (i == 1) {
					MaskImages[i].gameObject.SetActive(true);
				}
				else {
					MaskImages[i].gameObject.SetActive(false);
				}
			}
		}

		void onChange2(bool check) {
			iconIndex = 2;
			for (int i = 0; i < MaskImages.Length; i++) {
				if (i == 2) {
					MaskImages[i].gameObject.SetActive(true);
				}
				else {
					MaskImages[i].gameObject.SetActive(false);
				}
			}
		}

		void onChange3(bool check) {
			iconIndex = 3;
			for (int i = 0; i < MaskImages.Length; i++) {
				if (i == 3) {
					MaskImages[i].gameObject.SetActive(true);
				}
				else {
					MaskImages[i].gameObject.SetActive(false);
				}
			}
		}

		void onChange4(bool check) {
			iconIndex = 4;
			for (int i = 0; i < MaskImages.Length; i++) {
				if (i == 4) {
					MaskImages[i].gameObject.SetActive(true);
				}
				else {
					MaskImages[i].gameObject.SetActive(false);
				}
			}
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "RandomBtn":
				randomName();
				break;
			case "CreateBtn":
				if (NameInputField.text.Length > 6) {
					AlertCtrl.Show("称谓不能超过6个字！");
					return;
				}
				ConfirmCtrl.Show(string.Format("<color=\"{0}\">{1}</color>以后就是你的称谓，确定后不可更改", Statics.GetGenderColor(genderType), NameInputField.text), () => {
					RoleData role = new RoleData();
					role.Id = hostRoleId;
					role.Name = NameInputField.text;
					role.Gender = genderType;
					role.IsHost = true;
					role.ResourceWeaponDataId = "1"; //默认武器是布缠手
					role.Occupation = OccupationType.None;
					role.IconId = IconImages[iconIndex].sprite.name;
					role.DeadSoundId = role.Gender == GenderType.Male ? "die0003" : "die0002";
					Messenger.Broadcast<RoleData>(NotifyTypes.CreateHostRole, role);
				}, null, "确定", "取消");
				break;
			default:
				break;
			}
		}

		void randomName() {
			firstName = firstNames[Random.Range(0, firstNames.Count - 1)];
			secondName = secondNames[Random.Range(0, secondNames.Count - 1)];
			NameInputField.text = firstName + secondName;
		}

		public void UpdateData(string id, GenderType gender, List<string> firstnames, List<string> secondnames) {
			hostRoleId = id;
			genderType = gender;
			firstNames = firstnames;
			secondNames = secondnames;
		}

		public void RefreshView() {
			randomName();
			onChange0(true);
		}

	}
}
