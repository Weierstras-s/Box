using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;

namespace Stage.GameStates {
    public class SwitchLevel : FSMState<LevelManager> {
        public SwitchLevel() {

        }

        public override void Update() {

        }
        public override void Enter(object obj) {
            foreach (var (_, item) in self.map.items) {
                Object.Destroy(item.instance);
            }


            var level = LevelData.Level.FromJson(TestScript.Level2());
            self.LoadLevel(level);

            fsm.Translate<Idle>();
        }
        public override void Exit(object obj) {

        }
    }
}
