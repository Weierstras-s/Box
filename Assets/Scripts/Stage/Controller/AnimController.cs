using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Stage.Config.Static.Animation.Move;

namespace Stage {
    /// <summary>
    /// ����������
    /// </summary>
    public class AnimController : MonoBehaviour {
        private Vector3 from;
        private Vector3 dest;
        private Vector3 dir;

        /// <summary> ��ʼ���ƶ� </summary>
        /// <param name="dest"> Ŀ�� </param>
        public void Prepare(Vector3 dest) {
            from = transform.position;
            this.dest = dest;

            // ������ƽ���ϵ��ƶ�����
            static Vector3 Transform(Vector3 p) {
                var d = Camera.main.transform.rotation * Vector3.forward;
                var k = Vector3.Dot(p, Vector3.up) / Vector3.Dot(d, Vector3.up);
                if (Mathf.Abs(k) < float.Epsilon) return p;
                return Vector3Int.RoundToInt(p - d * k);
            }
            dir = Transform(dest - from);
        }

        public void Move(float x) {
            var moved = Vector3.Lerp(Vector3.zero, dir, Smooth(x));
            Vector3 curPos;
            if (from.y < dest.y) curPos = dest - dir + moved;
            else curPos = from + moved;
            transform.position = curPos;
        }
    }
}
