using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
    public class SecretItemContainer : MonoBehaviour {
        public Image IconImage;
        public Text NameText;
        public Text DescText;
        public Button studyBtn;
        public Button forgetBtn;

        SecretData secretData;
        public void UpdateData(SecretData data) {
            secretData = data;
        }

        public void RefreshView() {
            
        }
    }
}
