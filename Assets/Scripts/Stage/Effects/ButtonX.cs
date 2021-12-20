using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using static Stage.StageManager;

namespace Stage.Effects {
    public class ButtonX : BaseEffect {
        

        public override void OnEnter(Item enter, Dictionary<string, string> args) {
            bool active = manager.controlledItems[args["id"]].active;
            manager.SetItemActive(args["id"], !active);
        }
    }
}