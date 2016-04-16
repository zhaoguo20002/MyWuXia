using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public class CreateHostRolePanelCtrl : WindowCore<CreateHostRolePanelCtrl, JArray> {
		
		List<string> firstNames;
		List<string> secondNames;

		protected override void Init () {
			TextAsset asset = Resources.Load<TextAsset>("Data/Json/FirstNamesList");
			firstNames = JsonManager.GetInstance().DeserializeObject<List<string>>(asset.text);
			asset = Resources.Load<TextAsset>("Data/Json/SecondNamesList");
			secondNames = JsonManager.GetInstance().DeserializeObject<List<string>>(asset.text);
			asset = null;
		}
	}
}
