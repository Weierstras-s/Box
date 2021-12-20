using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.Config.Static.Camera.Controller;

namespace Stage.GameStates {
    public class AdjustCamera : FSMState<StageManager> {
        public class Param {
            /// <summary> ������� </summary>
            public Vector3 mousePosition;
            /// <summary> ��������� </summary>
            public Quaternion camRotation;
            /// <summary> �ӽ� </summary>
            public Views.BaseView beginView;
        }

        private Param param;
        private Vector3 beginRot;
        private Vector3 mousePos;
        private float movedDist;
        private float movedTime;

        public AdjustCamera() {
            // �ɿ����ʱ��������״̬
            AddTransition<Idle>((ref object enter, ref object exit) => {
                if (Input.GetMouseButton(0)) return false;
                return true;
            });
        }

        public override void Update() {
            // ��������ƶ��ľ������ƶ�ʱ��
            var mousePosRaw = Input.mousePosition;
            movedDist += (mousePosRaw - mousePos).magnitude;
            movedTime += Time.deltaTime;
            mousePos = mousePosRaw;

            // �������������
            float speed = Config.Input.Mouse.camRotateSpeed;
            var mouse = (mousePos - param.mousePosition) * speed;
            float newX = Mathf.Clamp(beginRot.x - mouse.y, -90, 90);
            param.mousePosition.y = mousePos.y - (beginRot.x - newX) / speed;
            float newY = beginRot.y + mouse.x;
            self.camera.rotation.Set(Quaternion.Euler(newX, newY, 0));
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
            // �ı�ģʽ (3D/2D)
            // ����: ����ƶ�������ƶ�ʱ���С
            bool isPersp = param.beginView is Views.Perspective;
            if (movedDist < switchDistTolerance ||
                movedTime < switchTimeTolerance) {
                isPersp = !isPersp;
            }

            // ����Ŀ���ӽ�
            var target = self.camera.GetTargetView(isPersp);
            self.map.view = target;
            // ���������
            self.camera.SetView(self.map.view);
        }
    }
}
