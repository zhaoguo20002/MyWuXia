using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Game {
	public class AlertCtrl : WindowCore<AlertCtrl, JArray> {
		string msg;
		Text msgText;
		Button okButton;
		protected override void Init () {
			msgText = GetChildText("msgText");
			okButton = GetChildButton("okButton");
			EventTriggerListener.Get(okButton.gameObject).onClick += onClick;
		}

		void onClick(GameObject e) {
			MoveOut();
		}

		public override void UpdateData (object obj) {
			JArray data = (JArray)obj;
			msg = data[0].ToString();
		}

		public override void RefreshView () {
			msgText.text = msg;
		}

		/// <summary>
		/// Show the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public static void Show(JArray data) {
			string id = System.DateTime.Now.ToFileTime().ToString();
			InstantiateView("Prefabs/UI/Comm/AlertView", id, Screen.width);
			var ctrl = id == "" ? Ctrl : Ctrls[id];
			ctrl.UpdateData(data);
			ctrl.RefreshView();
			ctrl.MoveIn();
		}
	}
}