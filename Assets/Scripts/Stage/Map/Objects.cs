using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Stage.Effects;
using static UnityEngine.Object;

namespace Stage.Objects {
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseObj {
        [JsonProperty] public Vector3Int position;

        public GameObject instance { get; private set; }
        public Transform transform {
            get => instance.transform;
        }
        public virtual void SetInstance(GameObject prefab, Vector3 pos) {
            if (instance != null) Destroy(instance);
            if (prefab == null) return;
            instance = Instantiate(prefab);
            transform.position = pos;
        }
        public void DestroyInstance() {
            if (instance != null) Destroy(instance);
            instance = null;
        }
    }
    public class Item : BaseObj {
        [JsonProperty] public int mask = -1;
    }

    public class Floor : Item {

    }
    public class ControlledFloor : Floor {
        [JsonProperty] public string id = "";
        [JsonProperty] public bool active = false;
    }

    public class Box : Item {
        public override void SetInstance(GameObject prefab, Vector3 pos) {
            base.SetInstance(prefab, pos);
            instance.AddComponent<AnimController>();
        }
    }

    public class Player : Item {
        [JsonProperty] public int playerId = 0;

        public Player nextPlayer;
        public override void SetInstance(GameObject prefab, Vector3 pos) {
            base.SetInstance(prefab, pos);
            instance.AddComponent<AnimController>();
        }
    }

    public class Trigger : BaseObj {
        public virtual void OnEnter(Item enter) { }
        public virtual void OnSwitch(Item enter, Item exit) { }
        public virtual void OnExit(Item exit) { }
    }

    public class Goal : Trigger {
        [JsonProperty] public bool isPlayer = false;

        public override void OnEnter(Item enter) {
            if (isPlayer && enter is not Player) return;
            if (!isPlayer && enter is not Box) return;
            var anim = enter.instance.GetComponent<AnimController>();
            anim.isOnGoal = true;
        }
        public override void OnExit(Item exit) {
            if (isPlayer && exit is not Player) return;
            if (!isPlayer && exit is not Box) return;
            var anim = exit.instance.GetComponent<AnimController>();
            anim.isOnGoal = false;
        }
        public override void OnSwitch(Item enter, Item exit) {
            OnEnter(enter); OnExit(exit);
        }
    }

    public class Effect : Trigger {
        [JsonProperty] public string script;
        [JsonProperty] public Dictionary<string, string> args;

        private BaseEffect effect;

        private static GameObject LoadScript(string name) {
            var prefab = Resources.Load<GameObject>("Effects/" + name);
            return prefab;
        }
        public override void SetInstance(GameObject prefab, Vector3 pos) {
            base.SetInstance(LoadScript(script), pos);
            effect = instance.GetComponent<BaseEffect>();
        }
        public override void OnEnter(Item enter) {
            effect.OnEnter(enter, args);
        }
        public override void OnExit(Item exit) {
            effect.OnExit(exit, args);
        }
        public override void OnSwitch(Item enter, Item exit) {
            effect.OnSwitch(enter, exit, args);
        }
    }
}