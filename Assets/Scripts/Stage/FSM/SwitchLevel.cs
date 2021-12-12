using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.Objects;
using static Stage.CameraController;

namespace Stage.GameStates {
    public class SwitchLevel : FSMState<LevelManager> {
        private class State1 : FSMState<SwitchLevel> {
            private LevelManager levelManager;

            public override void Update() {
                fsm.Translate<State2>();
            }
            public override void Enter(object obj) {
                levelManager = self.self;
                foreach (var (_, item) in self.self.map.items) {
                    Object.Destroy(item.instance);
                }
                levelManager.player = null;
            }
            public override void Exit(object obj) {

            }
        }
        private class State2 : FSMState<SwitchLevel> {
            private LevelManager levelManager;

            public override void Update() {
                fsm.ExitState();
            }
            public override void Enter(object obj) {
                levelManager = self.self;
                var level = LevelData.Level.FromJson(TestScript.Level2());
                levelManager.LoadLevel(level);
                levelManager.cameraController.center.Init();
                levelManager.cameraController.SetView(self.self.map.view);
            }
            public override void Exit(object obj) {
                self.fsm.Translate<Idle>();
            }

        }

        public FSM<SwitchLevel> subFsm;
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
