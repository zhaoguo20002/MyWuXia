using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
	public class SellItemContainer : MonoBehaviour {
		public Image Icon;
		public Image FlagImage;
		public Text NumText;
		public Toggle Btn;

		ItemData itemData;

		void Start() {
			Btn.onValueChanged.AddListener(onToggle);
		}

		void onToggle(bool check) {
			Messenger.Broadcast(NotifyTypes.MakeSelectedItemOfSellItemsPanel);
		}

		/// <summary>
		/// 判断是否被选中
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked {
			get {
				return Btn.isOn;
			}
		}

		/// <summary>
		/// 物品数据
		/// </summary>
		/// <value>The item.</value>
		public ItemData Item {
			get {
				return itemData;
			}
		}

		public void UpdateData(ItemData item) {
			itemData = item;
		}

		public void RefreshView() {
			Icon.sprite = Statics.GetIconSprite(itemData.IconId);
			Btn.isOn = false;
			NumText.text = string.Format("{0}/{1}", itemData.Num, itemData.MaxNum);
		}
	}
}
