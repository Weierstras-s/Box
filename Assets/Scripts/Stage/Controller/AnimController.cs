using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Stage {
    /// <summary>
    /// 动画控制类
    /// </summary>
    public class AnimController : MonoBehaviour {
        private const float epsilon = 1e-2f;

        private Vector3 from;
        private Vector3 dest;
        private Vector3 dir;
        bool rollback;

        private readonly List<AnimController> copies = new();

        /// <summary> 初始化移动 </summary>
        /// <param name="dest"> 目标 </param>
        public void Prepare(Vector3 dest, bool rollback) {
            from = transform.position;
            this.dest = dest;
            this.rollback = rollback;

            // 计算在平面上的移动方向
            static Vector3 Transform(Vector3 p) {
                var d = Camera.main.transform.rotation * Vector3.forward;
                var k = Vector3.Dot(p, Vector3.up) / Vector3.Dot(d, Vector3.up);
                if (Mathf.Abs(k) < float.Epsilon) return p;
                return Vector3Int.RoundToInt(p - d * k);
            }
            dir = Transform(dest - from);

            // 复制实例
            if (!rollback) {
                copies.Add(Instantiate(gameObject).GetComponent<AnimController>());
                copies[0].transform.position = dest - dir;
                SetDirection(dir);
                copies[0].SetDirection(-dir);
            } else {
                copies.Add(Instantiate(gameObject).GetComponent<AnimController>());
                copies.Add(Instantiate(gameObject).GetComponent<AnimController>());
                copies[0].transform.position = from;
                copies[1].transform.position = dest;
                SetHeight(-1);
                copies[0].SetHeight(1);
                copies[1].SetHeight(1);
            }
        }

        public void SetDirection(Vector3 dir) {
            GetComponent<MeshRenderer>().material.SetVector("_Direction", dir);
        }
        public void SetHeight(float x) {
            GetComponent<MeshRenderer>().material.SetFloat("_Height", x + epsilon);
        }

        public void Move(float x) {
            if (!rollback) {
                SetHeight(0.5f - x);
                copies[0].SetHeight(x - 0.5f);
                var moved = Vector3.Lerp(Vector3.zero, dir, x);
                transform.position = from + moved;
                copies[0].transform.position = dest - dir + moved;
            } else {
                transform.position = Vector3.Lerp(from, dest, x);
                copies[0].transform.localScale = (1 - x)*Vector3.one;
                copies[1].transform.localScale = x*Vector3.one;
                /*var moved = Vector3.down * x;
                transform.position = from + moved;
                copy.transform.position = dest - moved;*/
            }
        }

        public void Exit() {
            SetHeight(1);
            foreach (var obj in copies) {
                if (!obj) continue;
                Destroy(obj.gameObject);
            }
            copies.Clear();
        }
    }
}
