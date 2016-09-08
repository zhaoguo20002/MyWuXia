using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using Newtonsoft.Json.Linq;

namespace Game {
	public class TaskDetailDialogNoticeContainer : MonoBehaviour, ITaskDetailDialogInterface {
		public Text Msg;

		CanvasGroup alphaGroup;
		string msgStr;

		public void UpdateData(string id, JArray data, bool willDuring) {
			msgStr = data[2].ToString();
            msgStr = msgStr.Replace("<n>", DbManager.Instance.HostData.Name);
            msgStr = msgStr.Replace("<o>", Statics.GetOccupationName(DbManager.Instance.HostData.Occupation));
            msgStr = msgStr.Replace("<s>", Statics.GetGenderDesc(DbManager.Instance.HostData.Gender));
            msgStr = msgStr.Replace("<ss>", DbManager.Instance.HostData.Gender == GenderType.Male ? "哥哥" : "姐姐");
            msgStr = msgStr.Replace("<sss>", DbManager.Instance.HostData.Gender == GenderType.Male ? "公子" : "小姐");
			if (willDuring) {
				alphaGroup = gameObject.AddComponent<CanvasGroup>();
				alphaGroup.alpha = 0;
				alphaGroup.DOFade(1, 0.5f).OnComplete(() => {
					if (alphaGroup != null) {
						Destroy(alphaGroup);
					}
				});
			}
		}

		public void RefreshView() {
			Msg.text = msgStr;
		}
	}
}
