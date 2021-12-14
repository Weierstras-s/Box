using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.Objects;

namespace Stage.GameStates {
    public class SwitchLevel : FSMState<LevelManager> {
        /// <summary>
        /// 状态 1: 原关卡中的方块消失
        /// </summary>
        private class State1 : FSMState<SwitchLevel> {
            private LevelManager manager {
                get => self.self;
            }

            public override void Update() {
                fsm.Translate<State2>();
            }
            public override void Enter(object obj) {
                foreach (var (_, item) in self.self.map.items) {
                    Object.Destroy(item.instance);
                }
                manager.player = null;
            }
            public override void Exit(object obj) {

            }
        }

        /// <summary>
        /// 状态 2: 生成新关卡中的方块
        /// </summary>
        private class State2 : FSMState<SwitchLevel> {
            private LevelManager manager {
                get => self.self;
            }

            public override void Update() {
                fsm.ExitState();
            }
            public override void Enter(object obj) {
                var level = LevelData.Level.FromJson(TestScript.Level2());
                manager.LoadLevel(level);
                manager.camera.center.Init();
                manager.camera.SetView(self.self.map.view);
            }
            public override void Exit(object obj) {
                self.fsm.Translate<Idle>();
            }

        }

        private readonly FSM<SwitchLevel> subFsm;
        public SwitchLevel() {
            subFsm = new(this);
            subFsm.AddState<State1>();
            subFsm.AddState<State2>();
        }

        public override void Update() {
            subFsm.Update();
        }
        public override void Enter(object obj) {
            subFsm.EnterState<State1>();
        }
        public override void Exit(object obj) {

        }
    }
}
