using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public class Common {
        public static void Swap<T>(ref T x, ref T y) {
            T temp = x; x = y; y = temp;
        }
        public static float Damping(float time = 0.2f) {
            // v = 1 - (eps^(1/T))^(dt) => approx: v = -log(eps)/T*dt
            const float eps = 5f; // -ln(0.0067);
            return eps / time * Time.deltaTime;
        }
        public static float ClampAngle(float deg) {
            while (deg > 180) deg -= 360;
            while (deg <= -180) deg += 360;
            return deg;
        }
        public static Vector3 ParseVector3(string s) {
            var ls = s.Split(',');
            if (ls.Length != 3) throw new FormatException();
            float x = float.Parse(ls[0]);
            float y = float.Parse(ls[1]);
            float z = float.Parse(ls[2]);
            return new(x, y, z);
        }
        public static Vector3Int ParseVector3Int(string s) {
            var ls = s.Split(',');
            if (ls.Length != 3) throw new FormatException();
            int x = int.Parse(ls[0]);
            int y = int.Parse(ls[1]);
            int z = int.Parse(ls[2]);
            return new(x, y, z);
        }
    }
    public class Smoothing<T> {
        public Action<T> SetCurrent = null;
        public Func<T> GetCurrent = null;
        public Action<T> SetDest = null;
        public Func<T> GetDest = null;
        public Func<T, T, T> Lerp = null;

        private T m_current;
        public T current {
            get {
                if (GetCurrent != null) return GetCurrent();
                return m_current;
            }
            private set {
                if (SetCurrent != null) SetCurrent(value);
                else m_current = value;
            }
        }

        private T m_dest;
        public T dest {
            get {
                if (GetDest != null) return GetDest();
                return m_dest;
            }
            private set {
                if (SetDest != null) SetDest(value);
                else m_dest = value;
            }
        }

        public void Set(T dest) { this.dest = dest; }
        public void Init() { current = dest; }
        public void Init(T dest) { this.dest = current = dest; }
        public void Update() { current = Lerp(current, dest); }
    }
}