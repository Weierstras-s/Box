using System;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;

namespace Stage.Config.Input {
    public class Keyboard {
        public static KeyCode keyMoveR = KeyCode.D;
        public static KeyCode keyMoveU = KeyCode.W;
        public static KeyCode keyMoveL = KeyCode.A;
        public static KeyCode keyMoveD = KeyCode.S;
        public static KeyCode keyRollback = KeyCode.Z;
        public static KeyCode keyRestart = KeyCode.R;
        public static KeyCode keySwitchPlayer = KeyCode.Space;
        public static KeyCode keyExit = KeyCode.Escape;
    }
    public class Mouse {
        public static float camRotateSpeed = 0.8f;
        public static float camScrollSpeed = 2f;
    }
}
