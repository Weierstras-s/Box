using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Stage.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stage.LevelData.Json.Converters;

namespace Stage.LevelData.Json {
    namespace Converters {
        internal class Vector3IntConverter : JsonConverter<Vector3Int> {
            public override Vector3Int ReadJson(JsonReader reader, Type objectType,
                    Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer) {
                var ls = (reader.Value as string).Split(',');
                if (ls.Length != 3) throw new FormatException();
                int x = int.Parse(ls[0]);
                int y = int.Parse(ls[1]);
                int z = int.Parse(ls[2]);
                return new(x, y, z);
            }

            public override void WriteJson(JsonWriter writer,
                    Vector3Int value, JsonSerializer serializer) {
                var str = value.x + "," + value.y + "," + value.z;
                writer.WriteValue(str);
            }
        }

        internal class Vector3Converter : JsonConverter<Vector3> {
            public override Vector3 ReadJson(JsonReader reader, Type objectType,
                    Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer) {
                var ls = (reader.Value as string).Split(',');
                if (ls.Length != 3) throw new FormatException();
                float x = float.Parse(ls[0]);
                float y = float.Parse(ls[1]);
                float z = float.Parse(ls[2]);
                return new(x, y, z);
            }

            public override void WriteJson(JsonWriter writer,
                    Vector3 value, JsonSerializer serializer) {
                var str = value.x + "," + value.y + "," + value.z;
                writer.WriteValue(str);
            }
        }

        internal class OrthogonalConverter : JsonConverter<Orthogonal> {
            public override Orthogonal ReadJson(JsonReader reader, Type objectType,
                    Orthogonal existingValue, bool hasExistingValue, JsonSerializer serializer) {
                int dir = (int)(reader.Value as long?);
                if (objectType == typeof(Face)) return new Face(dir);
                if (objectType == typeof(Vertex)) return new Vertex(dir);
                if (objectType == typeof(Edge)) return new Edge(dir);
                return new(dir);
            }

            public override void WriteJson(JsonWriter writer,
                    Orthogonal value, JsonSerializer serializer) {
                writer.WriteValue(value.directionId);
            }
        }

        internal class JsonDataConverter : JsonConverter<JsonData> {
            public override JsonData ReadJson(JsonReader reader, Type objectType,
                    JsonData existingValue, bool hasExistingValue, JsonSerializer serializer) {
                JObject obj = JObject.Load(reader);
                JsonData data = new();
                foreach (var (name, value) in obj) {
                    if (value.Type == JTokenType.Array) {
                        data.data[name] = value.ToObject<List<object>>(serializer);
                    } else {
                        data.nodes[name] = value.ToObject<JsonData>(serializer);
                    }
                }
                return data;
            }

            public override void WriteJson(JsonWriter writer,
                    JsonData value, JsonSerializer serializer) {
                JObject obj = new();
                foreach (var (name, ls) in value.nodes) {
                    obj.Add(name, JToken.FromObject(ls, serializer));
                }
                foreach (var (name, ls) in value.data) {
                    obj.Add(name, JToken.FromObject(ls, serializer));
                }
                obj.WriteTo(writer);
            }
        }
    }

    internal class Convert {
        private static JsonSerializerSettings m_settings = null;
        private static JsonSerializerSettings settings {
            get {
                if (m_settings != null) return m_settings;
                m_settings = new();
                m_settings.Converters = new List<JsonConverter>() {
                    new OrthogonalConverter(),
                    new Vector3Converter(),
                    new Vector3IntConverter(),
                    new JsonDataConverter(),
                };
                m_settings.Formatting = Formatting.Indented;
                return m_settings;
            }
        }
        public static string Serialize(object obj) {
            return JsonConvert.SerializeObject(obj, settings);
        }
        public static T Deserialize<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }

    internal class JsonData {
        public Dictionary<string, JsonData> nodes = new();
        public Dictionary<string, List<object>> data = new();
        private List<object> GetAll(bool recursive = false) {
            List<object> ret = new();
            void Search(JsonData data) {
                if (recursive) foreach (var (_, node) in data.nodes) Search(node);
                foreach (var (_, obj) in data.data) ret.AddRange(obj);
            }
            Search(this);
            return ret;
        }
        public void Clear() {
            nodes.Clear(); data.Clear();
        }
        public JsonData Find(string path, bool create = false) {
            string[] ls = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            JsonData cur = this;
            foreach (var name in ls) {
                if (!cur.nodes.ContainsKey(name)) {
                    if (!create) return null;
                    cur.nodes[name] = new();
                }
                cur = cur.nodes[name];
            }
            return cur;
        }
        public List<T> FindObjs<T>(string path, bool recursive = false)
                where T : class {
            JsonData cur = Find(path, false);
            List<T> ret = new();
            if (cur == null) return ret;
            foreach (var obj in cur.GetAll(recursive)) {
                if (obj is T) ret.Add(obj as T);
            }
            return ret;
        }
        public void AddObj<T>(string path, T obj) {
            JsonData cur = Find(path, true);
            string type = obj.GetType().Name;
            if (!cur.data.ContainsKey(type)) cur.data[type] = new();
            cur.data[type].Add(obj);
        }
        public void Transfer(string path = "") {
            foreach (var (name, node) in nodes) {
                node.Transfer(path + name + ".");
            }
            foreach (var (name, ls) in data) {
                string cur = path + name;
                for (int i = 0; i < ls.Count; ++i) {
                    ls[i] = PathManager.Transfer(cur, ls[i]);
                }
            }
        }
        public string ToJson() {
            return Convert.Serialize(this);
        }
        public static JsonData FromJson(string json) {
            var ret = Convert.Deserialize<JsonData>(json);
            ret.Transfer();
            return ret;
        }
    }
}
