using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using static Stage.CameraController;

namespace Stage.GameStates {
    public class Init : FSMState<LevelManager> {
        public Init() {

        }

        public override void Update() {

        }
        public override void Enter(object obj) {
            string json = TestScript.Level1();

            self.LoadLevel(LevelData.Level.FromJson(json));
            self.camera.SetView(self.map.view);
            self.camera.Init();

            fsm.Translate<Idle>();
        }
        public override void Exit(object obj) {

        }
    }
}
