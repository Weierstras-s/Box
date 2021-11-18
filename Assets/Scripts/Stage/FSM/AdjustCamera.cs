using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.CameraController;

namespace Stage.GameStates {
    public class AdjustCamera : FSMState<LevelManager> {
        public class Param {
            /// <summary> ������� </summary>
            public Vector3 mousePosition;
            /// <summary> ��������� </summary>
            public Quaternion camRotation;
            /// <summary> �ӽ� </summary>
            public Views.BaseView beginView;
        }

        private const float switchDistTolerance = 2f;
        private const float switchTimeTolerance = 0.08f;

        private Param param;
        private Vector3 beginRot;
        private Vector3 mousePos;
        private float movedDist;
        private float movedTime;

        private bool isPersp = true;

        public AdjustCamera() {
            // �ɿ����ʱ��������״̬
            AddTransition<Idle>((ref object enter, ref object exit) => {
                if (Input.GetMouseButton(0)) return false;
                return true;
            });

        }

        public override void Update() {
            // ��������ƶ��ľ������ƶ�ʱ��
            movedDist += (Input.mousePosition - mousePos).magnitude;
            movedTime += Time.deltaTime;

            // �������������
            mousePos = Input.mousePosition;
            var mouse = mousePos - param.mousePosition;
            float newX = Mathf.Clamp(beginRot.x - mouse.y, -90, 90);
            float newY = beginRot.y + mouse.x;
            cameraController.rotation = Quaternion.Euler(newX, newY, 0);
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
            if (movedDist < switchDistTolerance ||
                movedTime < switchTimeTolerance) {
                isPersp = !isPersp;
            }
            Debug.Log((movedDist, movedTime));

            // ����Ŀ���ӽ�
            var target = cameraController.GetTargetView(isPersp);
            self.map.view = target;
            // ���������
            if (self.map.view is Views.Orthogonal view) {
                cameraController.rotation = view.direction;
            }
        }
    }
}
