using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using static Utils.Common;

namespace Stage.Effects {
    public class ShowInfo : BaseEffect {
        private GameObject infoPrefab {
            get => Resources.Load<GameObject>("UI/Info");
        }
        private UI.InfoController info;

        public override void OnEnter(Item enter, Dictionary<string, string> args) {
            if (info != null) Destroy(info.gameObject);
            var obj = Instantiate(infoPrefab, UI.UIManager.canvas);
            info = obj.AddComponent<UI.InfoController>();
            info.msg = args["msg"];
            info.world = ParseVector3(args["pos"]);
            info.offset = float.Parse(args["offset"]);
        }
        public override void OnExit(Item exit, Dictionary<string, string> args) {
            if (info == null) return;
            info.Remove();
        }
    }
}