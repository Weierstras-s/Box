using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.Config.Input.Keyboard;

namespace Stage.GameStates {
    public class Idle : FSMState<StageManager> {
        public Idle() {
            // 单击鼠标左键移动摄像机
            AddTransition<AdjustCamera>((ref object enter, ref object exit) => {
                if (!Input.GetMouseButtonDown(0)) return false;
                enter = new AdjustCamera.Param {
                    mousePosition = Input.mousePosition,
                    camRotation = self.camera.transform.rotation,
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
            self.camera.UpdateDist(Input.mouseScrollDelta.y);

            // 玩家切换
            if (Input.GetKeyDown(keySwitchPlayer)) {
                self.player = self.player.nextPlayer;
            }

            // 重载关卡
            if (Input.GetKeyDown(keyRestart)) {
                var level = LevelData.LevelManager.LoadFromFile(self.currentLevel.name);
                self.SwitchLevel(level);
            }

            // 退出关卡
            if (Input.GetKeyDown(keyExit)) {
                if (self.currentLevel.name == "main") {
                    Debug.Log("exit");
                } else {
                    var level = LevelData.LevelManager.PrevLevel(self.currentLevel);
                    self.SwitchLevel(level);
                }
            }
        }
        public override void Enter(object obj) {

        }
        public override void Exit(object obj) {

        }
    }
}
