using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Stage.Objects {
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseObj {
        [JsonProperty]
        public Vector3Int position;

        public GameObject instance;
        public Transform transform {
            get => instance.transform;
        }
    }

    public class Item : BaseObj {
        [JsonProperty]
        public int mask = -1;
    }


    public class Floor : Item {
        
    }

    public class Box : Item {

    }

    public class Player : Item {
        [JsonProperty]
        public int playerId = 0;
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