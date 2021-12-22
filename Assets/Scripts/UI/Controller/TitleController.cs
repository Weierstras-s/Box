using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class TitleController : MonoBehaviour {
        private void Start() {
            Update();
        }

        private void Update() {
            var parent = transform.parent.GetComponent<RectTransform>().rect;
            var self = GetComponent<RectTransform>();
            self.sizeDelta = parent.height * 0.7f * new Vector2(0.832f, 1f);
        }
    }
}
