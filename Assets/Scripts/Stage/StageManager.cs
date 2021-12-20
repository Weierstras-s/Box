using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;
using Stage.LevelData;

namespace Stage {
    public class StageManager : MonoBehaviour {
        public static StageManager manager { get; private set; }
        private static FSM<StageManager> fsm;

        public new CameraController camera;

        /// <summary> 当前关卡 </summary>
        public Level currentLevel;

        /// <summary> 地图 </summary>
        public Map map;
        /// <summary> 受控物块 </summary>
        public Dictionary<string, ControlledFloor> controlledItems;

        /// <summary> 当前玩家 </summary>
        public Player player;

        /// <summary> 所有可行视角 </summary>
        public List<Orthogonal> views;

        private GameObject GetPrefab(BaseObj item) {
            string GetName() {
                if (item is Player) return "Player";
                if (item is Floor) return "Floor";
                if (item is Box) return "Box";
                if (item is Goal goal) {
                    if (goal.isPlayer) return "Goal-Player";
                    return "Goal-Box";
                }
                if (item is LevelSelector) return "LevelSelector";
                return "";
            }
            var obj = Resources.Load<GameObject>("Prefabs/" + GetName());
            return obj;
        }

        public void LoadLevel(Level level) {
            // 加载关卡数据
            currentLevel = level;
            map = new(itemData: level.Find<Item>("Objects"),
                      triggerData: level.Find<Trigger>("Objects"),
                      initView: level.FindOne<BaseView>("Views.Default"));
            controlledItems = new();
            foreach (var item in level.Find<ControlledFloor>("Objects.Controlled")) {
                controlledItems[item.id] = item;
            }
            player = map.players[0];
            views = level.Find<Orthogonal>("Views");

            // 加载实体
            foreach (var (pos, obj) in map.items) {
                obj.SetInstance(GetPrefab(obj), pos);
            }
            foreach (var (_, obj) in controlledItems) {
                obj.SetInstance(GetPrefab(obj), obj.position);
                SetItemActive(obj.id, obj.active);
            }
            foreach (var (pos, obj) in map.triggers) {
                obj.SetInstance(GetPrefab(obj), pos);
                if (map.items.TryGetValue(pos, out var item)) {
                    obj.OnEnter(item);
                }
            }
        }

        public void SetItemActive(string id, bool isActive) {
            var item = controlledItems[id];
            item.active = isActive;
            item.instance.SetActive(isActive);
            if (isActive) map.items.Add(item.position, item);
            else map.items.Remove(item.position);
            map.Build();
        }

        public void SwitchLevel(Level level) {
            fsm.ClearActions();
            fsm.Translate<SwitchLevel>(level);
        }

        private void Awake() {
            manager = this;
            fsm = new(this);
            fsm.AddState<Init>();
            fsm.AddState<Idle>();
            fsm.AddState<AdjustCamera>();
            fsm.AddState<Move>();
            fsm.AddState<SwitchLevel>();
            fsm.EnterState<Init>(LevelManager.MainLevel());

            camera = Camera.main.GetComponent<CameraController>();
        }
        private void Update() {
            fsm.Update();
            map.Debug();
        }
    }
}
