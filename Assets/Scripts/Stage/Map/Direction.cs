using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stage.Config.Keyboard;

namespace Stage {
    class Direction {
        public const int None = -1;
        public const int Right = 0;
        public const int Up = 1;
        public const int Left = 2;
        public const int Down = 3;
        public const int Rollback = 4;

        private static readonly List<(KeyCode, int)> directions = new() {
            (keyMoveR, Right),
            (keyMoveU, Up),
            (keyMoveL, Left),
            (keyMoveD, Down),
            (keyRollback, Rollback),
        };
        /// <summary> 获取键盘点击的方向, 如果没有按下方向键返回 -1 </summary>
        public static int Get(Func<KeyCode, bool> inputMethod) {
            int res = None, count = 0;
            foreach (var (key, dir) in directions) {
                if (!inputMethod(key)) continue;
                res = dir; ++count;
            }
            if (count != 1) return None;
            return res;
        }
        public static int Get() {
            return Get(Input.GetKeyDown);
        }
    }
}