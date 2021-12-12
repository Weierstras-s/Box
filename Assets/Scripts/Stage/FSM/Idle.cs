using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.CameraController;

namespace Stage.GameStates {
    public class Idle : FSMState<LevelManager> {
        public Idle() {
            // 通关
            AddTransition<SwitchLevel>((ref object enter, ref object exit) => {
                if (!self.map.IsWin()) return false;
                return true;
            });

            // 单击鼠标左键移动摄像机
            AddTransition<AdjustCamera>((ref object enter, ref object exit) => {
                if (!Input.GetMouseButtonDown(0)) return false;
                enter = new AdjustCamera.Param {
                    mousePosition = Input.mousePosition,
                    camRotation = self.cameraController.transform.rotation,
                    beginView = self.map.view,
                };
                return true;
            });

            // 方向键移动玩家
            AddTransition<Move>((ref object enter, ref object exit) => {
                int dir = Direction.Get();
                if (dir == Direction.None) return false;
                enter = new Move.Param() {
                    direction = dir,
                };
                return true;
            });
        }

        public override void Update() {
            // 鼠标滚轮调节摄像机距离
            self.cameraController.UpdateDist(Input.mouseScrollDelta.y);
        }
        public override void Enter(object obj) {

        }
        public override void Exit(object obj) {

        }
    }
}
