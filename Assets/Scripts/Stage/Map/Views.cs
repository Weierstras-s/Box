using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Utils;

namespace Stage.Views {
    /// <summary> 视图基类 </summary>
    public class BaseView {
        public Dictionary<Vector3Int, Item> nodes;
        public Dictionary<Item, Item[]> edges;

        /// <summary> 建图并存入 edges </summary>
        public virtual void Build() { }
    }

    /// <summary> 透视 </summary>
    public class Perspective : BaseView {
        public override void Build() {
            // 相邻方块连边
            foreach (var (ori, item) in nodes) {
                Vector3Int delta = new(1, 0, 0);
                for (int i = 0; i < 4; ++i, delta.Set(-delta.z, 0, delta.x)) {
                    var adj = ori + delta;
                    if (!nodes.TryGetValue(adj, out var to)) continue;
                    if (nodes.TryGetValue(adj + Vector3Int.up, out var up)) {
                        if (up is Floor) continue;
                    }
                    edges[item][i] = to;
                }
            }
        }
    }

    /// <summary> 正交基类 </summary>
    public class Orthogonal : BaseView {
        /// <summary> 方向 id </summary>
        public int directionId = 0;

        /// <summary> 摄影机方向 </summary>
        public virtual Quaternion direction {
            get { return new(); }
        }

        public Orthogonal(int directionId = 0) {
            this.directionId = directionId;
        }

        /// <summary> 变换所有点坐标使得方向为 0 </summary>
        protected void Rotate(int dir) {
            foreach (var (pos, item) in nodes) {
                var (x, y, z) = (pos.x, pos.y, pos.z);
                item.position = dir switch {
                    1 => new(-z, y, x),
                    2 => new(-x, y, -z),
                    3 => new(z, y, -x),
                    _ => new(x, y, z),
                };
            }
        }
    }

    /// <summary> 俯视图 </summary>
    public class Face : Orthogonal {
        public override Quaternion direction {
            get {
                return directionId switch {
                    1 => Quaternion.Euler(90, 90, 0),
                    2 => Quaternion.Euler(90, 180, 0),
                    3 => Quaternion.Euler(90, -90, 0),
                    _ => Quaternion.Euler(90, 0, 0),
                };
            }
        }

        public Face(int directionId = 0) : base(directionId) { }

        private Vector2Int Plain(Vector3Int p) {
            return new(p.x, p.z);
        }

        public override void Build() {
            Dictionary<Vector2Int, (Item, int)> proj = new();
            Dictionary<Vector2Int, (Item, int)> box = new();
            Rotate(directionId);

            void Insert(Dictionary<Vector2Int, (Item, int)> dict, Item item) {
                var (plain, y) = (Plain(item.position), item.position.y);
                if (!dict.ContainsKey(plain)) dict[plain] = (new(), int.MinValue);
                if (y > dict[plain].Item2) dict[plain] = (item, y);
            }

            // 投影到二维平面
            foreach (var (_, item) in nodes) {
                if (item is Floor) Insert(proj, item);
                Insert(box, item);
            }

            // 建边
            foreach (var (_, item) in nodes) {
                var ori = Plain(item.position);
                Vector2Int delta = new(1, 0);
                for (int i = 0; i < 4; ++i, delta.Set(-delta.y, delta.x)) {
                    var adj = ori + delta;
                    var target = item is Floor ? proj : box;
                    if (!target.TryGetValue(adj, out var to)) continue;
                    edges[item][i] = to.Item1;
                }
            }

            Rotate(0);
        }
    }

    /// <summary> 体对角线方向 </summary>
    public class Vertex : Orthogonal {
        public override Quaternion direction {
            get {
                float deg = Mathf.Rad2Deg * Mathf.Atan(Mathf.Sqrt(.5f));
                return directionId switch {
                    1 => Quaternion.Euler(deg, 135, 0),
                    2 => Quaternion.Euler(deg, -135, 0),
                    3 => Quaternion.Euler(deg, -45, 0),
                    _ => Quaternion.Euler(deg, 45, 0),
                };
            }
        }

        public Vertex(int directionId = 0) : base(directionId) { }

        private Vector2Int Plain(Vector3Int p) {
            return new(p.x + p.y, p.z + p.y);
        }

        public override void Build() {
            Dictionary<Vector2Int, List<Item>> proj = new();
            Rotate(directionId);

            // 投影到二维平面
            foreach (var (_, item) in nodes) {
                var plain = Plain(item.position);
                if (!proj.ContainsKey(plain)) proj[plain] = new();
                proj[plain].Add(item);
            }

            // 判断是否连通
            bool Blocked(Vector3Int from, Vector3Int to, Vector2Int delta) {
                var (dx, dy) = (delta.x, delta.y);
                if (dx < 0 || dy < 0) {
                    dx *= -1; dy *= -1;
                    Common.Swap(ref from, ref to);
                }
                if (to.y > from.y) return true;
                if (dx != 0) {
                    Vector2Int p1 = Plain(from) + new Vector2Int(1, 1);
                    Vector2Int p2 = p1 + new Vector2Int(1, 0);
                    Vector2Int p3 = Plain(from) + new Vector2Int(1, 0);
                    if (proj.TryGetValue(p1, out var ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            //if (item is not Floor) continue;
                            //if (pos.y > to.y && pos.y <= from.y + 1) return true;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                        }
                    }
                    if (proj.TryGetValue(p2, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            //if (item is not Floor) continue;
                            //if (pos.y > to.y && pos.y <= from.y + 1) return true;
                            if (pos.y > to.y + 1 && pos.y <= from.y + 1) return true;
                        }
                    }
                    if (proj.TryGetValue(p3, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                        }
                    }
                } else if (dy != 0) {
                    Vector2Int p1 = Plain(from) + new Vector2Int(1, 1);
                    Vector2Int p2 = p1 + new Vector2Int(0, 1);
                    Vector2Int p3 = Plain(from) + new Vector2Int(0, 1);
                    if (proj.TryGetValue(p1, out var ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                        }
                    }
                    if (proj.TryGetValue(p2, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y + 1 && pos.y <= from.y + 1) return true;
                        }
                    }
                    if (proj.TryGetValue(p3, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                        }
                    }
                }
                return false;
            }

            // 建边
            foreach (var (_, item) in nodes) {
                var ori = Plain(item.position);
                Vector2Int delta = new(1, 0);
                for (int i = 0; i < 4; ++i, delta.Set(-delta.y, delta.x)) {
                    var adj = ori + delta;
                    if (!proj.TryGetValue(adj, out var list)) continue;
                    int maxY = int.MinValue;
                    foreach (var to in list) {
                        if (Blocked(item.position, to.position, delta)) continue;
                        if (to.position.y <= maxY) continue;
                        maxY = to.position.y;
                        edges[item][i] = to;
                    }
                }
            }

            Rotate(0);
        }
    }

    /// <summary> 面对角线方向 </summary>
    public class Edge : Orthogonal {
        public override Quaternion direction {
            get {
                return directionId switch {
                    1 => Quaternion.Euler(45, 90, 0),
                    2 => Quaternion.Euler(45, 180, 0),
                    3 => Quaternion.Euler(45, -90, 0),
                    _ => Quaternion.Euler(45, 0, 0),
                };
            }
        }

        public Edge(int directionId = 0) : base(directionId) { }

        private Vector2Int Plain(Vector3Int p) {
            return new(p.x, p.z + p.y);
        }

        public override void Build() {
            Dictionary<Vector2Int, List<Item>> proj = new();
            Rotate(directionId);

            // 投影到二维平面
            foreach (var (_, item) in nodes) {
                var plain = Plain(item.position);
                if (!proj.ContainsKey(plain)) proj[plain] = new();
                proj[plain].Add(item);
            }

            // 判断是否连通
            bool Blocked(Vector3Int from, Vector3Int to, Vector2Int delta) {
                var (dx, dy) = (delta.x, delta.y);
                if (dy < 0) {
                    dy *= -1;
                    Common.Swap(ref from, ref to);
                }
                if (dx > 0) {
                    Vector2Int p1 = Plain(from) + new Vector2Int(1, 1);
                    Vector2Int p2 = Plain(from) + new Vector2Int(1, 0);
                    Vector2Int p3 = Plain(from);
                    if (proj.TryGetValue(p1, out var ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            //if (item is not Floor) continue;
                            //if (pos.y > to.y && pos.y <= from.y + 1) return true;
                            if (pos.y > to.y + 1) return true;
                        }
                    }
                    if (proj.TryGetValue(p2, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                        }
                    }
                    if (proj.TryGetValue(p3, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y <= to.y && pos.y > from.y) return true;
                        }
                    }
                } else if (dx < 0) {
                    Vector2Int p1 = Plain(from) + new Vector2Int(-1, 1);
                    Vector2Int p2 = Plain(from) + new Vector2Int(-1, 0);
                    Vector2Int p3 = Plain(from);
                    if (proj.TryGetValue(p1, out var ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y + 1) return true;
                        }
                    }
                    if (proj.TryGetValue(p2, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                        }
                    }
                    if (proj.TryGetValue(p3, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y <= to.y && pos.y > from.y) return true;
                        }
                    }
                } else if (dy != 0) {
                    if (to.y > from.y) return true;
                    Vector2Int p1 = Plain(from) + new Vector2Int(0, 2);
                    Vector2Int p2 = Plain(from) + new Vector2Int(0, 1);
                    if (proj.TryGetValue(p1, out var ls)) {
                        foreach (Item item in ls) {
                            if (item is not Floor) continue;
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y + 1) return true;
                        }
                    }
                    if (proj.TryGetValue(p2, out ls)) {
                        foreach (Item item in ls) {
                            var pos = item.position;
                            if (pos.y > to.y && pos.y <= from.y) return true;
                            if (pos.y == from.y + 1 && item is Floor) return true;
                        }
                    }
                }
                return false;
            }

            // 建边
            foreach (var (_, item) in nodes) {
                var ori = Plain(item.position);
                Vector2Int delta = new(1, 0);
                for (int i = 0; i < 4; ++i, delta.Set(-delta.y, delta.x)) {
                    var adj = ori + delta;
                    if (!proj.TryGetValue(adj, out var list)) continue;
                    int maxY = int.MinValue;
                    foreach (var to in list) {
                        if (Blocked(item.position, to.position, delta)) continue;
                        if (to.position.y <= maxY) continue;
                        maxY = to.position.y;
                        edges[item][i] = to;
                    }
                }
            }

            Rotate(0);
        }
    }

}
