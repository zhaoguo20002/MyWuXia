using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public class RoleInfoPanelCtrl : WindowCore<RoleInfoPanelCtrl, JArray> {
		Text testText;

		List<RoleData> roleDateList;
		protected override void Init () {
			testText = GetChildText("stateMsg0");
		}

		public override void UpdateData (object obj) {
			JArray data = (JArray)obj;
			roleDateList = new List<RoleData>(); 
			JArray itemData;
			for (int i = 0; i < data.Count; i++) {
				itemData = (JArray)data[i];
				roleDateList.Add(JsonManager.GetInstance().DeserializeObject<RoleData>(itemData[1].ToString()));
			}
		}

		public override void RefreshView () {
			if(roleDateList.Count > 0) {
				testText.text = roleDateList[0].Name;
			}
		}

		public static void Show(JArray data) {
			InstantiateView("Prefabs/UI/RoleInfoPanelView");
			Ctrl.UpdateData(data);
			Ctrl.RefreshView();
		}

		public static void Hide() {
			if (Ctrl != null) {
				Ctrl.Close();
			}
		}
	}
}
