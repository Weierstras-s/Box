using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.Config.Input.Keyboard;

namespace Stage.GameStates {
    public class Idle : FSMState<StageManager> {
        public Idle() {
            // �����������ƶ������
            AddTransition<AdjustCamera>((ref object enter, ref object exit) => {
                if (!Input.GetMouseButtonDown(0)) return false;
                enter = new AdjustCamera.Param {
                    mousePosition = Input.mousePosition,
                    camRotation = self.camera.transform.rotation,
                    beginView = self.map.view,
                };
                return true;
            });

            // ������ƶ����
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
            // �����ֵ������������
            self.camera.UpdateDist(Input.mouseScrollDelta.y);

            // ����л�
            if (Input.GetKeyDown(keySwitchPlayer)) {
                self.player = self.player.nextPlayer;
            }

            // ���عؿ�
            if (Input.GetKeyDown(keyRestart)) {
                var level = LevelData.LevelManager.LoadFromFile(self.currentLevel.name);
                self.SwitchLevel(level);
            }

            // �˳��ؿ�
            if (Input.GetKeyDown(keyExit)) {
                if (self.currentLevel.name == "main") {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
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
