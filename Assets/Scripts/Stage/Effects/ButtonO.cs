using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;

namespace Stage.Effects {
    public class ButtonO : BaseEffect {
        public override void OnEnter(Item enter, Dictionary<string, string> args) {
            StageManager.manager.SetItemActive(args["id"], true);
        }
        public override void OnExit(Item exit, Dictionary<string, string> args) {
            StageManager.manager.SetItemActive(args["id"], false);
        }
    }
}