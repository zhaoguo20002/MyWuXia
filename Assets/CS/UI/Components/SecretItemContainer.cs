using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
    public class SecretItemContainer : MonoBehaviour {
        public Image IconImage;
        public Text NameText;
        public Text DescText;
        public Button StudyBtn;
        public Button ForgetBtn;
        public Button MixBtn;
        public Image FlashImage;

        SecretData secretData;

        void Start() {
            EventTriggerListener.Get(StudyBtn.gameObject).onClick = onClick;
            EventTriggerListener.Get(ForgetBtn.gameObject).onClick = onClick;
            EventTriggerListener.Get(MixBtn.gameObject).onClick = onClick;
        }

        public void UpdateData(SecretData data) {
            secretData = data;
        }

        void onClick(GameObject e) {
            switch (e.name)
            {
                case "StudyBtn":
                    SendMessageUpwards("study", secretData);
                    break;
                case "ForgetBtn":
                    SendMessageUpwards("forget", secretData);
                    break;
                case "MixBtn":
                    ConfirmCtrl.Show(string.Format("融合<color=\"{0}\">{1}</color>需要消耗3张同类同品质的诀要，是否继续？", Statics.GetQualityColorString(secretData.Quality), secretData.Name), () => {
                        SendMessageUpwards("mix", secretData);
                    });
                    break;
                default:
                    break;
            }
        }

        public void RefreshView() {
            IconImage.sprite = Statics.GetIconSprite(secretData.IconId);
            FlashImage.gameObject.SetActive(((int)secretData.Quality) >= ((int)QualityType.FlashGold));
            NameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(secretData.Quality), secretData.Name);

            DescText.text = secretData.GetDesc();
            StudyBtn.gameObject.SetActive(string.IsNullOrEmpty(secretData.BelongToBookId));
            ForgetBtn.gameObject.SetActive(!string.IsNullOrEmpty(secretData.BelongToBookId));
        }
    }
}
