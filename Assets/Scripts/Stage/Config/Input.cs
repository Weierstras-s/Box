using System;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;

namespace Stage.Config.Input {
    public class Keyboard {
        public static KeyCode keyMoveR = KeyCode.RightArrow;
        public static KeyCode keyMoveU = KeyCode.UpArrow;
        public static KeyCode keyMoveL = KeyCode.LeftArrow;
        public static KeyCode keyMoveD = KeyCode.DownArrow;
        public static KeyCode keyRollback = KeyCode.Z;
        public static KeyCode keyRestart = KeyCode.R;
    }
    public class Mouse {
        public static float camRotateSpeed = 0.8f;
        public static float camScrollSpeed = 2f;
    }
}
