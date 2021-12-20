using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class UIManager : MonoBehaviour {
        private static Transform m_canvas;
        public static Transform canvas {
            get {
                if (m_canvas == null) {
                    m_canvas = GameObject.Find("Canvas").GetComponent<Transform>();
                }
                return m_canvas;
            }
        }

    }
}
