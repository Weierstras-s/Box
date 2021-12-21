using System;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;

namespace Stage.Config.Static {
    namespace Camera {
        public class Projection {
            public const float camNear = 1f;
            public const float camFar = 1000f;
            public const float camField = 20f;
        }

        public class Controller {
            public const float nearDist = 15f;
            public const float farDist = 40f;
            public const float defaultDist = 20f;

            /// <summary> �ӽ�����������Χ (�Ƕ�) </summary>
            public const float magTolerance = 180f;

            public const float switchDistTolerance = 5f;
            public const float switchTimeTolerance = 0.08f;
        }
    }

    namespace Animation {
        public class Move {
            public const float time = 0.2f;
            public static float Smooth(float x) {
                return 1 - Mathf.Pow(1 - x, 2);
            }
        }
    }
}
