using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Templates.FSM;
using static Stage.Config.Static.Animation.Move;

namespace Stage.GameStates {
    public class Move : FSMState<LevelManager> {
        public class Param {
            public int direction;
        }

        private Param param;
        private readonly List<Item> items = new();

        private float curTime;
        private int nextDirection;

        public override void Update() {
            // 读取下一步移动按键
            if (Direction.Get() != Direction.None) {
                nextDirection = Direction.Get();
            }
            if ((curTime += Time.deltaTime) > time) {
                fsm.ExitState();
                return;
            }

            // 更新坐标
            float moved = curTime / time;
            foreach (var item in items) {
                item.instance.GetComponent<AnimController>().Move(moved);
            }
        }
        public override void Enter(object obj) {
            param = obj as Param;
            // 判断是否能够移动
            var moveData = self.map.Move(self.player, param.direction);
            if (moveData == null) {
                fsm.Translate<Idle>();
                return;
            }

            curTime = 0;
            nextDirection = Direction.None;
            items.Clear();

            foreach (var (item, _) in moveData) {
                items.Add(item);
                var anim = item.instance.GetComponent<AnimController>();
                anim.Prepare(item.position);
            }
        }
        public override void Exit(object obj) {
            foreach (var item in items) {
                item.transform.position = item.position;
            }

            // 如果缓冲区内有按键或按着不放则继续移动
            if (nextDirection == Direction.None) {
                nextDirection = Direction.Get(Input.GetKey);
            }
            if (nextDirection != Direction.None) {
                fsm.EnterState<Move>(new Param() {
                    direction = nextDirection,
                });
            } else fsm.EnterState<Idle>();
        }
    }
}
