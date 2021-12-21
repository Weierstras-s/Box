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
                "ControlledFloor" => Deserialize<ControlledFloor>(json),
                "Box" => Deserialize<Box>(json),
                "Player" => Deserialize<Player>(json),

                // Trigger
                "Goal" => Deserialize<Goal>(json),
                "Effect" => Deserialize<Effect>(json),

                // Other types
                "String" => Deserialize<string>(json),
                _ => null,
            };
        }
    }

    public class Level {
        public string name { get; private set; }
        private JsonData objs = new();

        /// <summary> 添加物品 </summary>
        public void Add(string path, object obj) {
            objs.AddObj(path, obj);
        }

        /// <summary> 查找指定路径下的所有物品 </summary>
        public List<T> FindAll<T>(string path) where T : class {
            return objs.FindObjs<T>(path, true);
        }

        /// <summary> 查找指定路径下的所有物品 (不包括子目录) </summary>
        public List<T> Find<T>(string path) where T : class {
            return objs.FindObjs<T>(path, false);
        }

        /// <summary> 查找指定路径下的一个物品, 不存在返回 null </summary>
        public T FindOne<T>(string path) where T : class {
            var ls = objs.FindObjs<T>(path, false);
            if (ls.Count == 0) return null;
            return ls[0];
        }

        /// <summary> 序列化 </summary>
        public string ToJson() {
            return objs.ToJson();
        }

        /// <summary> 反序列化 </summary>
        public static Level FromJson(string json, string name = "") {
            Level ret = new();
            ret.objs = JsonData.FromJson(json);
            ret.name = name;
            return ret;
        }
    }
}
