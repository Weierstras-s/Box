using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public class Common {
        public static void Swap<T>(ref T x, ref T y) {
            T temp = x; x = y; y = temp;
        }
        public static float Lerp(float time = 0.2f) {
            // v = 1 - (eps^(1/T))^(dt) => approx: v = -log(eps)/T*dt
            const float eps = 5f; // -ln(0.0067);
            return eps / time * Time.deltaTime;
        }
        public static float ClampAngle(float deg) {
            while (deg > 180) deg -= 360;
            while (deg <= -180) deg += 360;
            return deg;
        }
    }
}