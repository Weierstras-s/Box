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
        private static FSM<LevelManager> fsm;

        public Map map;
        public new CameraController camera;

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
            map = new(itemData: data.Find<Item>(""),
                      goalData: data.Find<Goal>(""),
                      initView: data.Find<BaseView>("Views.Default")[0]);
            player = map.players[0];
            views = data.Find<Orthogonal>("");

            foreach (var (pos, item) in map.items) {
                item.SetInstance(GetPrefab(item), pos);
            }
        }

        private void Awake() {
            manager = this;
            fsm = new(this);
            fsm.AddState<Init>();
            fsm.AddState<Idle>();
            fsm.AddState<AdjustCamera>();
            fsm.AddState<Move>();
            fsm.AddState<SwitchLevel>();
            fsm.EnterState<Init>();

            camera = Camera.main.GetComponent<CameraController>();
        }
        private void Update() {
            fsm.Update();

            map.Debug();
        }


        public GameObject floorPrefab;
        public GameObject boxPrefab;
        public GameObject playerPrefab;
    }
}
