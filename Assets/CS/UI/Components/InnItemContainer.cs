using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	public class InnItemContainer : MonoBehaviour {
		public Text CityNameText;
		public Text SliverText;
		public Button Btn;

		FloydResult resultData;
		
		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			ConfirmCtrl.Show(string.Format("马上就要出发前往<color=\"#00FF00\">{0}</color>了，客官是否准备妥当？", resultData.Name), () => {
				Messenger.Broadcast<int, int>(NotifyTypes.GoToCity, resultData.FromIndex, resultData.ToIndex);
			}, null, "是的", "等下");
		}

		public void UpdateData(FloydResult result) {
			resultData = result;
		}

		public void RefreshView() {
			CityNameText.text = resultData.Name;
			SliverText.text = ((int)resultData.Distance).ToString();
		}

	}
}
