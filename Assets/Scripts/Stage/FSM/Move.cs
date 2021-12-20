using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Templates.FSM;
using static Stage.Config.Static.Animation.Move;

namespace Stage.GameStates {
    public class Move : FSMState<StageManager> {
        public class Param {
            public int direction;
        }

        private Param param;
        private readonly List<Item> items = new();

        private bool moveFailed;
        private float curTime;
        private int nextDirection;

        public override void Update() {
            // ��ȡ��һ���ƶ�����
            if (Direction.Get() != Direction.None) {
                nextDirection = Direction.Get();
            }
            if ((curTime += Time.deltaTime) > time) {
                fsm.ExitState();
                return;
            }

            // ��������
            float moved = Smooth(curTime / time);
            foreach (var item in items) {
                item.instance.GetComponent<AnimController>().Move(moved);
            }
        }
        public override void Enter(object obj) {
            param = obj as Param;

            // �ж��Ƿ��ܹ��ƶ�
            var moveData = self.map.Move(self.player, param.direction);
            moveFailed = moveData == null;
            if (moveFailed) {
                fsm.Translate<Idle>();
                return;
            }

            curTime = 0;
            nextDirection = Direction.None;
            items.Clear();

            foreach (var (item, _) in moveData) {
                items.Add(item);
                var anim = item.instance.GetComponent<AnimController>();
                anim.Prepare(item.position, param.direction == Direction.Rollback);
            }
        }
        public override void Exit(object obj) {
            // ���û�������ƶ�ֱ���˳�
            if (moveFailed) return;

            foreach (var item in items) {
                item.transform.position = item.position;
                item.instance.GetComponent<AnimController>().Exit();
            }
            items.Clear();

            // ������������а������Ų���������ƶ�
            if (nextDirection == Direction.None) {
                nextDirection = Direction.Get(Input.GetKey);
                // ��������������
                if (nextDirection == Direction.Rollback) {
                    nextDirection = Direction.None;
                }
            }
            fsm.EnterState<Idle>();
            if (nextDirection != Direction.None) {
                fsm.Translate<Move>(new Param() {
                    direction = nextDirection,
                });
            }

            // ���� trigger
            if (self.map.triggerAction != null) {
                self.map.triggerAction.Invoke();
                self.map.triggerAction = null;
            }

            // ͨ��
            if (self.map.IsWin()) {
                self.SwitchLevel(LevelData.LevelManager.NextLevel(self.currentLevel));
                return;
            }
        }
    }
}
