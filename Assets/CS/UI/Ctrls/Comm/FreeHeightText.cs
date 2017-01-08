using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game {
    public class FreeHeightText : MonoBehaviour {
        public float Height;
        public Text Text;
        // Use this for initialization
        void Awake () {
            Text = GetComponent<Text>();
        }

        public void SetValue(string context) {
            Text.text = context;
        }
    }
}
