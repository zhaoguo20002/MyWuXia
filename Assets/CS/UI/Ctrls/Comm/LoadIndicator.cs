using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game {
    public class LoadIndicator : MonoBehaviour {
        public List<Image> Points;
        List<float> angles;
        float direction = -1;
        float speed = 10;
        // Use this for initialization
        void Awake () {
            angles = new List<float>();
            for (int i = Points.Count - 1; i >= 0; i--) {
                angles.Add(i * 30);
            }
            ation();
        }

        void ation() {
            for (int i = 0, len = Points.Count; i < len; i++) {
                Points[i].rectTransform.anchoredPosition = Statics.GetCirclePoint(Vector2.zero, 40, direction * angles[i]);
                angles[i] += speed;
                if (angles[i] >= 360) {
                    angles[i] = 0;
                }
            }
        }
        
        // Update is called once per frame
        void Update () {
            ation();
        }
    }
}
