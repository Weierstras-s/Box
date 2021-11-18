using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Stage.Views;
using Stage.LevelData.Json;
using static Stage.LevelData.Json.Convert;

namespace Stage.LevelData {
    internal class PathManager {
        public static string GetPathByType(Type type, int level) {
            List<Type> ts = new() { type };
            string path = "";
            for (int i = 1; i < level; ++i) ts.Add(ts[^1].BaseType);
            ts.Reverse();
            foreach (var t in ts) path += t.Name + ".";
            return path[0..^1];
        }
        public static string GetPath<T>(int level) {
            return GetPathByType(typeof(T), level);
        }
        public static string GetPath(object obj, int level) {
            return GetPathByType(obj.GetType(), level);
        }
        public static object Transfer(string path, object obj) {
            string type = path.Split('.')[^1];
            string json = Serialize(obj);
            return type switch {
                // View
                "Face" => Deserialize<Face>(json),
                "Vertex" => Deserialize<Vertex>(json),
                "Edge" => Deserialize<Edge>(json),

                // Item
                "Floor" => Deserialize<Floor>(json),
                "Box" => Deserialize<Box>(json),
                "Player" => Deserialize<Player>(json),

                //Trigger
                "Goal" => Deserialize<Goal>(json),
                "Effect" => Deserialize<Effect>(json),
                _ => null,
            };
        }
    }

    public class Level {
        private JsonData objs = new();

        /// <summary> 添加物品 </summary>
        public void Add(object obj) {
            var path = PathManager.GetPath(obj, 2);
            objs[path].Add(obj);
        }

        /// <summary> 查找某种类型的物品 </summary>
        public List<T> Find<T>() where T : class {
            var ls = objs.FindByName(typeof(T).Name);
            return ls.ConvertAll<T>(new((object obj) => obj as T));
        }

        /// <summary> 序列化 </summary>
        public string ToJson() {
            return objs.ToJson();
        }

        /// <summary> 反序列化 </summary>
        public static Level FromJson(string json) {
            Level ret = new();
            ret.objs = JsonData.FromJson(json);
            return ret;
        }
    }
}
