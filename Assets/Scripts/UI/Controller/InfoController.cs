using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class InfoController : MonoBehaviour {
        public Vector3 world;
        public float offset;
        public string msg;

        public void Remove() {
            Destroy(gameObject);
        }

        private void Start() {
            Update();
        }

        private void Update() {
            var text = transform.Find("Text");
            var textmesh = text.GetComponent<TMPro.TextMeshProUGUI>();
            textmesh.text = msg;
            var position = Camera.main.WorldToScreenPoint(world);
            position.y += offset;
            GetComponent<RectTransform>().position = position;
        }
    }
}
