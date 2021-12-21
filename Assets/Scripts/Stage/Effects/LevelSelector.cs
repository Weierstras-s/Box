using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;

namespace Stage.Effects {
    public class LevelSelector : BaseEffect {
        public override void OnEnter(Item enter, Dictionary<string, string> args) {
            var level = LevelData.LevelManager.LoadFromFile(args["name"]);
            StageManager.manager.SwitchLevel(level);
        }
    }
}