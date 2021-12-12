using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.CameraController;

namespace Stage.GameStates {
    public class AdjustCamera : FSMState<LevelManager> {
        public class Param {
            /// <summary> 鼠标坐标 </summary>
            public Vector3 mousePosition;
            /// <summary> 摄像机方向 </summary>
            public Quaternion camRotation;
            /// <summary> 视角 </summary>
            public Views.BaseView beginView;
        }

        private const float switchDistTolerance = 5f;
        private const float switchTimeTolerance = 0.08f;

        private Param param;
        private Vector3 beginRot;
        private Vector3 mousePos;
        private float movedDist;
        private float movedTime;

        public AdjustCamera() {
            // 松开鼠标时返回闲置状态
            AddTransition<Idle>((ref object enter, ref object exit) => {
                if (Input.GetMouseButton(0)) return false;
                return true;
            });
        }

        public override void Update() {
            // 更新鼠标移动的距离与移动时间
            var mousePosRaw = Input.mousePosition;
            movedDist += (mousePosRaw - mousePos).magnitude;
            movedTime += Time.deltaTime;
            mousePos = mousePosRaw;

            // 更新摄像机方向
            float speed = Config.Input.Mouse.camRotateSpeed;
            var mouse = (mousePos - param.mousePosition) * speed;
            float newX = Mathf.Clamp(beginRot.x - mouse.y, -90, 90);
            param.mousePosition.y = mousePos.y - (beginRot.x - newX) / speed;
            float newY = beginRot.y + mouse.x;
            self.cameraController.rotation.Set(Quaternion.Euler(newX, newY, 0));
        }
        public override void Enter(object obj) {
            param = obj as Param;
            mousePos = param.mousePosition;
            beginRot = param.camRotation.eulerAngles;
            beginRot.x = Utils.Common.ClampAngle(beginRot.x);
            self.map.view = new Views.Perspective();
            movedDist = movedTime = 0;
        }
        public override void Exit(object obj) {
            // 改变模式 (3D/2D)
            // 条件: 鼠标移动距离或移动时间较小
            bool isPersp = param.beginView is Views.Perspective;
            if (movedDist < switchDistTolerance ||
                movedTime < switchTimeTolerance) {
                isPersp = !isPersp;
            }

            // 计算目标视角
            var target = self.cameraController.GetTargetView(isPersp);
            self.map.view = target;
            // 摄像机吸附
            self.cameraController.SetView(self.map.view);
        }
    }
}
