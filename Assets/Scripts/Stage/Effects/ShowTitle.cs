using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using static Utils.Common;

namespace Stage.Effects {
    public class ShowTitle : BaseEffect {
        private GameObject titlePrefab {
            get => Resources.Load<GameObject>("UI/Title/Title");
        }
        private GameObject title;

        public override void OnEnter(Item enter, Dictionary<string, string> args) {
            if (title != null) return;
            title = Instantiate(titlePrefab, UI.UIManager.canvas);
        }
        public override void OnExit(Item exit, Dictionary<string, string> args) {
            if (title == null) return;
            Destroy(title);
        }
    }
}