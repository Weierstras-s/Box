using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using UnityEngine.SceneManagement;

namespace Stage.Effects {
    public class SceneSwitcher : BaseEffect {
        public override void OnEnter(Item enter, Dictionary<string, string> args) {
            SceneManager.LoadScene(args["name"]);
        }
    }
}