using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class BagItemContainer : ComponentCore {
		public Image Icon;
		public Text NameText;
		public Text NumText;
		public Text TypeText;
		public Button Btn;

		ItemData itemData;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			switch(e.name) {
			case "Btn":
				Messenger.Broadcast<ItemData, bool>(NotifyTypes.ShowItemDetailPanel, itemData, true);
				break;
			default:
				break;
			}
		}
		
		public void UpdateData(ItemData item) {
			itemData = item;
		}
		
		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(itemData.IconId);
			NameText.text = string.Format("<color=\"#1ABDE6\">{0}</color>", itemData.Name);
			TypeText.text = string.Format("类型:{0}", Statics.GetItemTypeName(itemData.Type));
			NumText.text = string.Format("数量:{0}/{1}", itemData.Num, itemData.MaxNum);
		}
		
	}
}
