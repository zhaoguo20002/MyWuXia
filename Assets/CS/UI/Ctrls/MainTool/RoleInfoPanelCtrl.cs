using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public class RoleInfoPanelCtrl : WindowCore<RoleInfoPanelCtrl, JArray> {

		protected override void Init () {

		}

		public override void UpdateData (object obj) {

		}

		public override void RefreshView () {

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
