using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Templates.FSM;

namespace Stage.GameStates {
    public class Move : FSMState<LevelManager> {
        public class Param {
            public int direction;
        }

        private const float time = 0.2f;
        private static float Smooth(float x) {
            return 1 - Mathf.Pow(1 - x, 2);
        }

        private Param param;
        private readonly Dictionary<Item, Vector3> oldPos = new();

        private float curTime;
        private int nextDirection;

        public override void Update() {
            if (Direction.Get() != Direction.None) {
                nextDirection = Direction.Get();
            }
            if ((curTime += Time.deltaTime) > time) {
                fsm.ExitState();
                return;
            }

            // ��������
            float moved = curTime / time;
            foreach (var (item, pos) in oldPos) {
                var curPos = Vector3.Lerp(pos, item.position, Smooth(moved));
                item.transform.position = curPos;
            }
        }
        public override void Enter(object obj) {
            param = obj as Param;
            // �ж��Ƿ��ܹ��ƶ�
            var moveData = self.map.Move(self.player, param.direction);
            if (moveData == null) {
                fsm.Translate<Idle>();
                return;
            }

            curTime = 0;
            nextDirection = Direction.None;
            oldPos.Clear();
            foreach (var (item, _) in moveData) {
                oldPos[item] = item.transform.position;
            }
        }
        public override void Exit(object obj) {
            foreach (var (item, _) in oldPos) {
                item.transform.position = item.position;
            }

            // ������������а������Ų���������ƶ�
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
