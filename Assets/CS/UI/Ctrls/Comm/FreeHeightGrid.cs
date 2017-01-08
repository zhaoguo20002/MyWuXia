using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
    public class FreeHeightGrid : ComponentCore {
        public float Height;

        List<string> contexts;
        List<FreeHeightText> freeHeightTexts;
        Queue<string> queue;
        Object prefab;
        bool canRefresh;
        // Use this for initialization
        void Awake () {
            contexts = new List<string>();
            freeHeightTexts = new List<FreeHeightText>();
            queue = new Queue<string>();
            prefab = Statics.GetPrefab("Prefabs/UI/Comm/FreeHeightText");
            Height = 0;
            canRefresh = false;
        }

        void Update() {
            if (canRefresh && queue.Count > 0) {
                canRefresh = false;
                string popContext = queue.Dequeue();
                contexts.Add(popContext);
                FreeHeightText freeHeightText = Statics.GetPrefabClone(prefab).GetComponent<FreeHeightText>();
                MakeToParent(transform, freeHeightText.transform);
                freeHeightTexts[freeHeightTexts.Count - 1].Text.rectTransform.anchoredPosition = new Vector2(0, Height);
                freeHeightText.SetValue(popContext);
                freeHeightTexts.Add(freeHeightText);
                StartCoroutine(refresh());
            }
        }

        public void PushContext(string context) {
            canRefresh = true;
            queue.Enqueue(context);
        }

        IEnumerator refresh() {
            yield return null;
            appendSize();
        }

        void appendSize() {
            Height += freeHeightTexts[freeHeightTexts.Count - 1].Text.rectTransform.sizeDelta.y;
            canRefresh = true;
        }
    }
}
