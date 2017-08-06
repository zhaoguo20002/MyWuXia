using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace Game {
	public class StoreItemContainer : ComponentCore {
		public Image Icon;
		public Text NameText;
		public Text PriceText;
		public Text TypeText;
		public Button Btn;
		public Button BuyBtn;

		ItemData itemData;

		// Use this for initialization
		void Start () {
			EventTriggerListener.Get(Btn.gameObject).onClick = onClick;
			EventTriggerListener.Get(BuyBtn.gameObject).onClick = onClick;
		}

		void onClick(GameObject e) {
			if (!e.GetComponent<Button>().enabled) {
				return;
			}
            switch (e.name)
            {
                case "Btn":
                    Messenger.Broadcast<ItemData, bool>(NotifyTypes.ShowItemDetailPanel, itemData, false);
                    break;
                case "BuyBtn":
                    ConfirmCtrl.Show(string.Format("确定花费{0}两银子购买<color=\"#1ABDE6\">{1}</color>？", itemData.BuyPrice, itemData.Name), () =>
                    {
                        Messenger.Broadcast<string>(NotifyTypes.BuyItem, itemData.Id);
                    }, null, "确定", "取消");
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
			PriceText.text = itemData.BuyPrice.ToString();
			TypeText.text = string.Format("类型:{0}", Statics.GetItemTypeName(itemData.Type));
		}
		
	}
}
