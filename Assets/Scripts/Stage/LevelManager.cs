using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;

namespace Stage {
    public class LevelManager : MonoBehaviour {
        public static LevelManager manager { get; private set; }
        public static FSM<LevelManager> fsm;

        public Map map;

        public List<Orthogonal> views;
        public Player player;

        private GameObject GetPrefab(Item item) {
            GameObject obj;
            if (item is Box) obj = boxPrefab;
            else if (item is Player) obj = playerPrefab;
            else obj = floorPrefab;
            return obj;
        }

        public void LoadLevel(LevelData.Level data) {
            map = new(data);
            player = map.players[0];
            views = data.Find<Orthogonal>();

            foreach (var (pos, item) in map.items) {
                var obj = GetPrefab(item);
                item.instance = Instantiate(obj, pos, new Quaternion());
            }
        }

        private void Awake() {
            manager = this;
            fsm = new(this);
            fsm.AddState<Idle>();
            fsm.AddState<AdjustCamera>();
            fsm.AddState<Move>();
            fsm.AddState<SwitchLevel>();
            fsm.Translate<Idle>();
        }
        private void Update() {
            fsm.Update();

            map.Debug();
        }


        public GameObject floorPrefab;
        public GameObject boxPrefab;
        public GameObject playerPrefab;

        private void Start() {
            string json = TestScript.Level1();

            LoadLevel(LevelData.Level.FromJson(json));
        }
    }
}
