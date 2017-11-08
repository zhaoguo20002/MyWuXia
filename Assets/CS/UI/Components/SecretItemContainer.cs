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
                    Debug.Log("遗忘");
                    break;
                case "MixBtn":
                    Debug.Log("融合");
                    break;
                default:
                    break;
            }
        }

        public void RefreshView() {
            IconImage.sprite = Statics.GetIconSprite(secretData.IconId);
            NameText.text = string.Format("<color=\"{0}\">{1}</color>", Statics.GetQualityColorString(secretData.Quality), secretData.Name);
            string desc = "";
            switch(secretData.Type) {
                case SecretType.IncreaseMaxHP:
                    desc = string.Format("气血上限点数+{0}", secretData.IntValue);
                    break;
                case SecretType.IncreaseMaxHPRate:
                    desc = string.Format("基础气血上限比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreasePhysicsAttack:
                    desc = string.Format("外功点数+{0}", secretData.IntValue);
                    break;
                case SecretType.IncreasePhysicsAttackRate:
                    desc = string.Format("基础外功比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreasePhysicsDefense:
                    desc = string.Format("外防点数+{0}", secretData.IntValue);
                    break;
                case SecretType.IncreasePhysicsDefenseRate:
                    desc = string.Format("基础外防比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreaseMagicAttack:
                    desc = string.Format("内功点数+{0}", secretData.IntValue);
                    break;
                case SecretType.IncreaseMagicAttackRate:
                    desc = string.Format("基础内功比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreaseMagicDefense:
                    desc = string.Format("内防点数+{0}", secretData.IntValue);
                    break;
                case SecretType.IncreaseMagicDefenseRate:
                    desc = string.Format("基础内防比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreaseFixedDamage:
                    desc = string.Format("固定伤害+{0}", secretData.IntValue);
                    break;
                case SecretType.IncreaseDamageRate:
                    desc = string.Format("伤害比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreaseHurtCutRate:
                    desc = string.Format("减伤比例+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.IncreaseDodge:
                    desc = string.Format("轻功+{0}", secretData.IntValue);
                    break;
                case SecretType.DrugResistance:
                    desc = string.Format("中毒抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.DisarmResistance:
                    desc = string.Format("缴械抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.VertigoResistance:
                    desc = string.Format("眩晕抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.CanNotMoveResistance:
                    desc = string.Format("定身抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.SlowResistance:
                    desc = string.Format("迟缓抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.ChaosResistance:
                    desc = string.Format("混乱抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.AlarmedResistance:
                    desc = string.Format("惊慌抵抗+{0}", secretData.IntValue);
                    break;
                case SecretType.CutCD:
                    desc = string.Format("减少所有侠客的武功招式CD时间{0}秒", secretData.FloatValue);
                    break;
                case SecretType.Immortal:
                    desc = string.Format("抵御{0}次阵亡效果", secretData.IntValue);
                    break;
                case SecretType.Killed:
                    desc = string.Format("{0}%概率秒杀敌方(对Boss无效)", (secretData.FloatValue * 100d) / 100);
                    break;
                case SecretType.MakeAFortune:
                    desc = string.Format("掉落概率+{0}%", (secretData.FloatValue * 100d) / 100);
                    break;
                default:
                    break;
            }
            DescText.text = desc;
            StudyBtn.gameObject.SetActive(string.IsNullOrEmpty(secretData.BelongToBookId));
            ForgetBtn.gameObject.SetActive(!string.IsNullOrEmpty(secretData.BelongToBookId));
        }
    }
}
