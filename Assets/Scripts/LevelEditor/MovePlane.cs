using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor {
    public class MovePlane : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                Vector3 newplane = transform.position;
                newplane.y += 1f;
                transform.position = newplane;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                Vector3 newplane = transform.position;
                newplane.y -= 1f;
                transform.position = newplane;
            }
        }
    }
}
