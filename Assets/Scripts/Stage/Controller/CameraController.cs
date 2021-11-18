using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Stage.LevelManager;

namespace Stage {
    /// <summary>
    /// 摄像机控制类
    /// <para> 可改变摄像机角度, 调节摄影机距离 </para>
    /// <para> 挂在摄像机上 </para>
    /// </summary>
    public class CameraController : MonoBehaviour {
        public class ProjectionMatrix {
            private const float camNear = 1f;
            private const float camFar = 1000f;
            private const float camField = 20f;

            /// <summary> 透视率 </summary>
            public float x = 1f;

            public Matrix4x4 Get(Rect rect, float camDist) {
                float n = camNear, f = camFar;
                float alpha = Mathf.Deg2Rad * camField;

                float D = camDist, R = rect.height / rect.width;
                float cot = 1 / Mathf.Tan(alpha);
                float k = Mathf.Tan(alpha * x) * cot;
                float A = (k * (n + f) + 2 * D * (1 - k)) / (n - f);
                float B = (2 * k * n * f + D * (n + f) * (1 - k)) / (n - f);
                Vector4 v0 = new(cot * R, 0, 0, 0);
                Vector4 v1 = new(0, cot, 0, 0);
                Vector4 v2 = new(0, 0, A, -k);
                Vector4 v3 = new(0, 0, B, (1 - k) * D);
                return new(v0, v1, v2, v3);
            }
        }

        public static CameraController cameraController { get; private set; }

        private const float nearDist = 10f;
        private const float farDist = 30f;
        private const float defaultDist = 15f;
        private const float scrollSpeed = 2f;

        /// <summary> 视角吸附允许误差范围 (角度) </summary>
        private const float magTolerance = 45f;

        /// <summary> 摄像机角度 </summary>
        public Quaternion rotation;

        private float camDist = defaultDist;
        private float curDist;

        private Vector3 center { get => manager.player.transform.position; }
        private Vector3 curCenter = new();

        private readonly ProjectionMatrix projMatrix = new();


        private void Awake() {
            cameraController = this;
            rotation = transform.rotation;
            curDist = camDist;
        }

        public Views.BaseView GetTargetView(bool isPersp = false) {
            Views.BaseView ret = new Views.Perspective();
            if (isPersp) return ret;
            float minW = magTolerance;

            foreach (var view in manager.views) {
                float w = Quaternion.Angle(transform.rotation, view.direction);
                if (w > minW) continue;
                minW = w; ret = view;
            }
            return ret;
        }

        public void UpdateDist(float delta) {
            delta *= -scrollSpeed;
            camDist = Mathf.Clamp(camDist + delta, nearDist, farDist);
        }

        private void Update() {
            // 改投影矩阵
            float persp = manager.map.view is Views.Orthogonal ? 0f : 1f;
            projMatrix.x = Mathf.Lerp(projMatrix.x, persp, Common.Lerp());
            var camera = GetComponent<Camera>();
            camera.projectionMatrix = projMatrix.Get(camera.pixelRect, curDist);

            // 改旋转角
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Common.Lerp());

            // 改位置
            curCenter = Vector3.Lerp(curCenter, center, Common.Lerp());
            curDist = Mathf.Lerp(curDist, camDist, Common.Lerp());
            transform.position = curCenter + transform.rotation * new Vector3(0, 0, -curDist);
        }
    }
}
