using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;

namespace Stage.GameStates {
    public class Init : FSMState<StageManager> {
        public Init() {

        }

        public override void Update() {

        }
        public override void Enter(object obj) {
            var level = obj as LevelData.Level;

            self.LoadLevel(level);
            self.camera.SetView(self.map.view);
            self.camera.Init();

            fsm.Translate<Idle>();
        }
        public override void Exit(object obj) {

        }
    }
}
