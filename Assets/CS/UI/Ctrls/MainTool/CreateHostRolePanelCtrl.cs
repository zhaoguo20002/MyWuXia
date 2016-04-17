using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace Game {
	public class CreateHostRolePanelCtrl : WindowCore<CreateHostRolePanelCtrl, JArray> {
		List<Text> msgTexts;
		CanvasGroup roleInfoBg;
		Toggle maleToggle;
		CreateHostRoleContainer createMaleHostRoleContainer;
		Toggle femaleToggle;
		CreateHostRoleContainer createFemaleHostRoleContainer;

		List<string> firstNames;
		List<string> secondNames;
		string hostRoleId;

		protected override void Init () {
			msgTexts = new List<Text>() {
				GetChildText("Msg0Text"),
				GetChildText("Msg1Text"),
				GetChildText("Msg2Text"),
				GetChildText("Msg3Text"),
				GetChildText("Msg4Text")
			};
			roleInfoBg = GetChildCanvasGroup("RoleInfoBg");
			maleToggle = GetChildToggle("MaleToggle");
			maleToggle.onValueChanged.AddListener(onChange0);
			createMaleHostRoleContainer = GetChildComponent<CreateHostRoleContainer>(gameObject, "CreateMaleHostRoleContainer");
			femaleToggle = GetChildToggle("FemaleToggle");
			femaleToggle.onValueChanged.AddListener(onChange1);
			createFemaleHostRoleContainer = GetChildComponent<CreateHostRoleContainer>(gameObject, "CreateFemaleHostRoleContainer");

			TextAsset asset = Resources.Load<TextAsset>("Data/Json/FirstNamesList");
			firstNames = JsonManager.GetInstance().DeserializeObject<List<string>>(asset.text);
			asset = Resources.Load<TextAsset>("Data/Json/SecondNamesList");
			secondNames = JsonManager.GetInstance().DeserializeObject<List<string>>(asset.text);
			asset = null;
		}

		void onChange0(bool check) {
			if (!createMaleHostRoleContainer.gameObject.activeSelf) {
				maleToggle.targetGraphic.color = new Color(1, 1, 0, 1);
				femaleToggle.targetGraphic.color = new Color(1, 1, 1, 1);
				createMaleHostRoleContainer.gameObject.SetActive(true);
				createFemaleHostRoleContainer.gameObject.SetActive(false);
			}
		}

		void onChange1(bool check) {
			if (!createFemaleHostRoleContainer.gameObject.activeSelf) {
				maleToggle.targetGraphic.color = new Color(1, 1, 1, 1);
				femaleToggle.targetGraphic.color = new Color(1, 1, 0, 1);
				createMaleHostRoleContainer.gameObject.SetActive(false);
				createFemaleHostRoleContainer.gameObject.SetActive(true);
			}
		}

		public void UpdateData(string id) {
			hostRoleId = id;
		}

		public override void RefreshView () {
			for (int i = 0; i < msgTexts.Count; i++) {
				msgTexts[i].DOFade(0, 0);
			}
			roleInfoBg.DOFade(0, 0);

			msgTexts[0].DOFade(1, 1).SetDelay(0.5f);
			msgTexts[1].DOFade(1, 1).SetDelay(2);
			roleInfoBg.DOFade(1, 1).SetDelay(3.5f);
			onChange0(true);
			createMaleHostRoleContainer.UpdateData(hostRoleId, GenderType.Male, firstNames, secondNames);
			createMaleHostRoleContainer.RefreshView();
			createFemaleHostRoleContainer.UpdateData(hostRoleId, GenderType.Female, firstNames, secondNames);
			createFemaleHostRoleContainer.RefreshView();
		}

		public void StoryContinue(string name) {
			roleInfoBg.gameObject.SetActive(false);
			msgTexts[2].text = string.Format("你叫{0}。", name);
			msgTexts[2].DOFade(1, 1).SetDelay(0.5f);
			msgTexts[3].DOFade(1, 1).SetDelay(2);
			msgTexts[4].DOFade(1, 1).SetDelay(3.5f).OnComplete(() => {
				Close();
				//进入游戏
				Messenger.Broadcast(NotifyTypes.EnterGame);
			});
		}

		public static void Show(string id) {
			if (Ctrl == null) {
				InstantiateView("Prefabs/UI/MainTool/CreateHostRolePanelView", "CreateHostRolePanelCtrl", 0, 0, UIModel.FrameCanvas.transform);
			}
			Ctrl.UpdateData(id);
			Ctrl.RefreshView();
		}

		/// <summary>
		/// 继续剧情描述后进入游戏
		/// </summary>
		/// <param name="name">Name.</param>
		public static void MakeStoryContinue(string name) {
			if (Ctrl != null) {
				Ctrl.StoryContinue(name);
			}
		}
	}
}
