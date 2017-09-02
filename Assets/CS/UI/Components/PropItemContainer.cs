using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Game {
    public class PropItemContainer : MonoBehaviour {
        public List<Sprite> PropSprites;
        public Image IconImage;
        public Text TimerText;
        public Button PayBtn;
        public bool IsFree = false;

        PropData propData;
        string productId;
        DateTime date;
        double timeout = 120;
        bool timing;
        float skipDate;
        float skipTimeout = 0.5f;
        // Use this for initialization
        void Start () {
            EventTriggerListener.Get(PayBtn.gameObject).onClick = onClick;
        }

        void onClick(GameObject e) {
            if (!PayBtn.enabled)
            {
                return;
            }
            if (propData.Num >= propData.Max)
            {
                switch (propData.Type)
                {
                    case PropType.NocturnalClothing:
                        AlertCtrl.Show(string.Format("最多只能携带{0}件夜行衣", propData.Max));
                        break;
                    case PropType.Bodyguard:
                        AlertCtrl.Show(string.Format("最多只能雇佣{0}位镖师", propData.Max));
                        break;
                    case PropType.LimePowder:
                        AlertCtrl.Show(string.Format("最多只能携带{0}包石灰粉", propData.Max));
                        break;
                    case PropType.Scout:
                        AlertCtrl.Show(string.Format("最多只能拥有{0}个探子", propData.Max));
                        break;
                    default:
                        break;
                }
                return;
            }

            if (!IsFree)
            {
                if (propData.Num > 0)
                {
                    switch (propData.Type)
                    {
                        case PropType.NocturnalClothing:
                            AlertCtrl.Show(string.Format("你还有{0}件夜行衣，不可再买", propData.Num));
                            break;
                        case PropType.Bodyguard:
                            AlertCtrl.Show(string.Format("你还有{0}位镖师，不可再买", propData.Num));
                            break;
                        case PropType.LimePowder:
                            AlertCtrl.Show(string.Format("你还有{0}包石灰粉，不可再买", propData.Num));
                            break;
                        case PropType.Scout:
                            AlertCtrl.Show(string.Format("你还有{0}个探子，不可再买", propData.Num));
                            break;
                        default:
                            break;
                    }
                    return;
                }
                MaiHandler.PayForProduct(productId);
            }
            else
            {
                MaiHandler.StartRewardedVideo(() => {
                    StartTimer();
                    SendRewards(propData.Type, UnityEngine.Random.Range(1, 3));
                });
            }
        }

        public static void SendRewards(PropType type, int num) {
            switch (type)
            {
                case PropType.NocturnalClothing:
                    AlertCtrl.Show(string.Format("获得了{0}件夜行衣", num));
                    break;
                case PropType.Bodyguard:
                    AlertCtrl.Show(string.Format("获得了{0}位镖师", num));
                    break;
                case PropType.LimePowder:
                    AlertCtrl.Show(string.Format("获得了{0}包石灰粉", num));
                    break;
                case PropType.Scout:
                    AlertCtrl.Show(string.Format("获得了{0}个探子", num));
                    break;
                default:
                    break;
            }
            Messenger.Broadcast<PropType, int>(NotifyTypes.AddProp, type, num);
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
                PlayerPrefs.SetString("PropFreeTimeStamp_For" + propData.Type.ToString(), Statics.GetNowTimeStamp().ToString());
                date = Statics.ConvertStringToDateTime(PlayerPrefs.GetString("PropFreeTimeStamp_For" + propData.Type.ToString()));
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

        public void UpdateData(PropData data) {
            propData = data;
            switch (propData.Type)
            {
                case PropType.NocturnalClothing:
                    productId = "com.courage2017.prop_1";
                    break;
                case PropType.Bodyguard:
                    productId = "com.courage2017.prop_3";
                    break;
                case PropType.LimePowder:
                    productId = "com.courage2017.prop_4";
                    break;
                case PropType.Scout:
                    productId = "com.courage2017.prop_5";
                    break;
                default:
                    break;
            }
        }

        public void RefreshView() {
            switch (propData.Type)
            {
                case PropType.NocturnalClothing:
                    IconImage.sprite = PropSprites[0];
                    break;
                case PropType.Bodyguard:
                    IconImage.sprite = PropSprites[1];
                    break;
                case PropType.LimePowder:
                    IconImage.sprite = PropSprites[2];
                    break;
                case PropType.Scout:
                    IconImage.sprite = PropSprites[3];
                    break;
                default:
                    break;
            }
            date = DateTime.MinValue;
            timing = false;
            if (IsFree)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString("PropFreeTimeStamp_For" + propData.Type.ToString())))
                {
                    date = Statics.ConvertStringToDateTime(PlayerPrefs.GetString("PropFreeTimeStamp_For" + propData.Type.ToString()));
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
    }
}
