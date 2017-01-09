using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game {
    public class FreeHeightGrid : ComponentCore {
        public float Height;

        ScrollRect scrollView;
        RectTransform rect;
        List<string> contexts;
        List<FreeHeightText> freeHeightTexts;
        Queue<string> queue;
        Object prefab;
        float date = -1;
        // Use this for initialization
        void Awake () {
            scrollView = GetComponentInParent<ScrollRect>();
            rect = GetComponent<RectTransform>();
            contexts = new List<string>();
            freeHeightTexts = new List<FreeHeightText>();
            queue = new Queue<string>();
            prefab = Statics.GetPrefab("Prefabs/UI/Comm/FreeHeightText");
            Height = 0;
        }

        void Update() {
            if (queue.Count > 0) {
                if (Time.fixedTime - date > 0.1f) {
                    date = Time.fixedTime;
                    string popContext = queue.Dequeue();
                    contexts.Add(popContext);
                    FreeHeightText freeHeightText = Statics.GetPrefabClone(prefab).GetComponent<FreeHeightText>();
                    MakeToParent(transform, freeHeightText.transform);
                    freeHeightText.Text.rectTransform.anchoredPosition = new Vector2(0, -Height);
                    freeHeightText.SetValue(popContext);
                    freeHeightTexts.Add(freeHeightText);
                    StartCoroutine(refresh());
                }
            }
        }

        public void PushContext(string context) {
            queue.Enqueue(context);
        }

        IEnumerator refresh() {
            yield return null;
            appendSize();
        }

        void appendSize() {
            Height += freeHeightTexts[freeHeightTexts.Count - 1].Text.rectTransform.sizeDelta.y;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, Height);
            scrollView.verticalNormalizedPosition = 0;
        }
    }
}
