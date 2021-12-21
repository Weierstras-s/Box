using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LevelEditor {
    public class Button : MonoBehaviour {
        public Button btn;
        public Text txt;
        public void Save() {
            Debug.Log("Hello");
        }

        public void Back() {
            SceneManager.LoadScene("Stage");
        }
    }
}
