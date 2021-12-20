using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using static Stage.StageManager;
using static Stage.Config.Static.Camera.Projection;
using static Stage.Config.Static.Camera.Controller;

namespace Stage {
    /// <summary>
    /// �����������
    /// <para> �ɸı�������Ƕ�, ������Ӱ������ </para>
    /// <para> ����������� </para>
    /// </summary>
    public class CameraController : MonoBehaviour {
        public class ProjectionMatrix {

            /// <summary> ����ͶӰ���� </summary>
            /// <param name="x"> ͸���� </param>
            public static Matrix4x4 Get(float x, Rect rect, float camDist) {
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

        /// <summary> ����ҵľ��� </summary>
        private Smoothing<float> dist;
        /// <summary> ��������� </summary>
        public Smoothing<Vector3> center;
        /// <summary> ������Ƕ� </summary>
        public Smoothing<Quaternion> rotation;
        /// <summary> ͸���� </summary>
        public Smoothing<float> persp;

        private void Awake() {
            dist = new() {
                Lerp = (float x, float y) => Mathf.Lerp(x, y, Common.Damping()),
            };
            dist.Init(defaultDist);

            center = new() {
                Lerp = (Vector3 x, Vector3 y) => Vector3.Lerp(x, y, Common.Damping()),
                GetDest = () => {
                    if (manager.player == null) return center.current;
                    return manager.player.transform.position;
                },
            };

            rotation = new() {
                Lerp = (Quaternion x, Quaternion y) => Quaternion.Slerp(x, y, Common.Damping()),
                GetCurrent = () => transform.rotation,
                SetCurrent = (Quaternion x) => transform.rotation = x,
            };

            persp = new() {
                Lerp = (float x, float y) => Mathf.Lerp(x, y, Common.Damping()),
                GetDest = () => {
                    if (manager.map is null) return persp.current;
                    return manager.map.view is Views.Orthogonal ? 0f : 1f;
                },
            };
        }

        public void Init() {
            center.Init();
            rotation.Init();
            persp.Init();
        }

        public void SetView(Views.BaseView view, Quaternion? def = null) {
            if (view is Views.Orthogonal ortho) {
                rotation.Set(ortho.direction);
            } else if (def.HasValue) {
                rotation.Set(def.Value);
            }
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
            delta *= -Config.Input.Mouse.camScrollSpeed;
            dist.Set(Mathf.Clamp(dist.dest + delta, nearDist, farDist));
        }

        private void Update() {
            // ��ͶӰ����
            persp.Update();
            var camera = GetComponent<Camera>();
            var mat = ProjectionMatrix.Get(persp.current, camera.pixelRect, dist.current);
            camera.projectionMatrix = mat;

            // ����ת��
            rotation.Update();

            // ��λ��
            center.Update();
            dist.Update();
            transform.position = center.current + transform.rotation * new Vector3(0, 0, -dist.current);
        }
    }
}
