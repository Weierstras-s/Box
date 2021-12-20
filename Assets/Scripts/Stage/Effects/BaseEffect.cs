using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;

namespace Stage.Effects {
    public class BaseEffect : MonoBehaviour {
        public virtual void OnEnter(Item enter, Dictionary<string, string> args) { }
        public virtual void OnExit(Item exit, Dictionary<string, string> args) { }
        public virtual void OnSwitch(Item enter, Item exit, Dictionary<string, string> args) { }
    }
}