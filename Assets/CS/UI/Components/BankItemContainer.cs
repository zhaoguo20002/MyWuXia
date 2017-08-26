using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Game {
    public class BankItemContainer : MonoBehaviour {
        public Text TimerText;
        public Button PayBtn;
        public string ProductId;
        public bool IsFree = false;

        DateTime date;
        double timeout = 180;
        bool timing;
        float skipDate;
        float skipTimeout = 0.5f;
        // Use this for initialization
        void Start () {
            EventTriggerListener.Get(PayBtn.gameObject).onClick = onClick;
            date = DateTime.MinValue;
            timing = false;
            if (IsFree)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString("BankFreeTimeStamp")))
                {
                    date = Statics.ConvertStringToDateTime(PlayerPrefs.GetString("BankFreeTimeStamp"));
                    double remain = remainSeconds();
                    timing = remain > 0;
                    skipDate = Time.fixedTime;
                    if (timing)
                    {
                        TimerText.text = Statics.GetTime((int)remain);
                    }
                }
            }
            TimerText.gameObject.SetActive(timing);
            PayBtn.gameObject.SetActive(!timing);
        }

        void onClick(GameObject e) {
            if (!PayBtn.enabled)
            {
                return;
            }
            if (!IsFree)
            {
                MaiHandler.PayForProduct(ProductId);
            }
            else
            {
                MaiHandler.StartRewardedVideo(() => {
                    DbManager.Instance.GotSilver(1000);
                    Messenger.Broadcast<string>(NotifyTypes.GetStorePanelData, UserModel.CurrentUserData.CurrentCitySceneId);
                    StartTimer();
                    AlertCtrl.Show("银子 +1000");
                });
            }
        }

        double remainSeconds() {
            if (date != null)
            {
                return timeout - (DateTime.Now - date).TotalSeconds;
            }
            return 0;
        }

        void Update() {
            if (timing)
            {
                float dt = Time.fixedTime;
                if (dt - skipDate > skipTimeout)
                {
                    skipDate = dt;
                    double remain = remainSeconds();
                    TimerText.text = Statics.GetTime((int)remain);
                    if (remain < 0)
                    {
                        timing = false;
                        TimerText.gameObject.SetActive(timing);
                        PayBtn.gameObject.SetActive(!timing);
                    } 
                }
            }
        }

        public void StartTimer() {
            if (IsFree)
            {
                PlayerPrefs.SetString("BankFreeTimeStamp", Statics.GetNowTimeStamp().ToString());
                date = Statics.ConvertStringToDateTime(PlayerPrefs.GetString("BankFreeTimeStamp"));
                double remain = remainSeconds();
                timing = remain > 0;
                skipDate = Time.fixedTime;
                if (timing)
                {
                    TimerText.text = Statics.GetTime((int)remain);
                }
                TimerText.gameObject.SetActive(timing);
                PayBtn.gameObject.SetActive(!timing);
            }
        }
    }
}
