using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.Objects;
using static Stage.Config.Static.Animation.Move;

namespace Stage.GameStates {
    public class SwitchLevel : FSMState<StageManager> {
        /// <summary>
        /// 状态 1: 原关卡中的方块消失
        /// </summary>
        private class State1 : FSMState<SwitchLevel> {
            private StageManager manager { get => self.self; }

            private float curTime;
            private readonly List<BaseObj> objs = new();

            public override void Update() {
                if ((curTime += Time.deltaTime) >= time * 2) {
                    fsm.Translate<State2>();
                }
                float x = 1 - Smooth(curTime / (time * 2));
                foreach (var item in objs) {
                    item.transform.localScale = new(x, x, x);
                }
            }
            public override void Enter(object obj) {
                curTime = 0;
                manager.player = null;

                objs.Clear();
                foreach (var (_, item) in manager.map.items) objs.Add(item);
                foreach (var (_, item) in manager.controlledItems) objs.Add(item);
                foreach (var (_, item) in manager.map.triggers) objs.Add(item);
            }
            public override void Exit(object obj) {
                foreach (var item in objs) {
                    item.DestroyInstance();
                }
            }
        }

        /// <summary>
        /// 状态 2: 生成新关卡中的方块
        /// </summary>
        private class State2 : FSMState<SwitchLevel> {
            private StageManager manager { get => self.self; }

            private float curTime;
            private readonly List<BaseObj> objs = new();

            public override void Update() {
                if ((curTime += Time.deltaTime) >= time * 2) {
                    fsm.ExitState();
                }
                float x = Smooth(curTime / (time * 2));
                foreach (var item in objs) {
                    item.transform.localScale = new(x, x, x);
                }
            }
            public override void Enter(object obj) {
                curTime = 0;
                manager.LoadLevel(self.level);

                objs.Clear();
                foreach (var (_, item) in manager.map.items) objs.Add(item);
                foreach (var (_, item) in manager.map.triggers) objs.Add(item);

                foreach (var item in objs) {
                    item.transform.localScale = new(0, 0, 0);
                }

                manager.camera.center.Init();
                manager.camera.SetView(manager.map.view);
                manager.camera.rotation.Init();

            }
            public override void Exit(object obj) {
                self.fsm.Translate<Idle>();
                foreach (var item in objs) {
                    item.transform.localScale = new(1, 1, 1);
                }
            }

        }

        private readonly FSM<SwitchLevel> subFsm;
        public LevelData.Level level;
        public SwitchLevel() {
            subFsm = new(this);
            subFsm.AddState<State1>();
            subFsm.AddState<State2>();
        }

        public override void Update() {
            subFsm.Update();
        }
        public override void Enter(object obj) {
            level = obj as LevelData.Level;
            subFsm.EnterState<State1>();
        }
        public override void Exit(object obj) {

        }
    }
}
