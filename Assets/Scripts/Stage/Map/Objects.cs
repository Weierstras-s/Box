using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static UnityEngine.Object;

namespace Stage.Objects {
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseObj {
        [JsonProperty]
        public Vector3Int position;
    }

    public class Item : BaseObj {
        [JsonProperty]
        public int mask = -1;

        public GameObject instance { get; private set; }
        public Transform transform {
            get => instance.transform;
        }
        public virtual void SetInstance(GameObject prefab, Vector3 pos) {
            instance = Instantiate(prefab);
            transform.position = pos;
        }
    }


    public class Floor : Item {
        
    }

    public class Box : Item {
        public override void SetInstance(GameObject prefab, Vector3 pos) {
            base.SetInstance(prefab, pos);
            instance.AddComponent<AnimController>();
        }
    }

    public class Player : Item {
        [JsonProperty]
        public int playerId = 0;

        public Player nextPlayer;
        public override void SetInstance(GameObject prefab, Vector3 pos) {
            base.SetInstance(prefab, pos);
            instance.AddComponent<AnimController>();
        }
    }

    public class Trigger : BaseObj {

    }

    public class Goal : Trigger {
        [JsonProperty]
        public bool isPlayer = false;
    }

    public class Effect : Trigger {

    }
}