using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;

namespace Stage {
    public class TestScript : MonoBehaviour {
        public static string Level1() {
            LevelData.Level data = new();

            void Add(object item) {
                data.Add("", item);
            }

            Add(new Player() { position = new(0,1,0) });

            Add(new Floor() { position = new(0, 0, 0) });
            Add(new Floor() { position = new(0, 0, -1) });
            Add(new Floor() { position = new(0, 0, 1) });
            Add(new Floor() { position = new(-1, 0, 0) });
            Add(new Floor() { position = new(-1, 0, -1) });
            Add(new Floor() { position = new(-1, 0, 1) });
            Add(new Floor() { position = new(1, 0, 0) });
            Add(new Floor() { position = new(1, 0, -1) });
            Add(new Floor() { position = new(1, 0, 1) });

            Add(new Floor() { position = new(0, 1, -3) });
            Add(new Floor() { position = new(0, 1, -4) });
            Add(new Floor() { position = new(0, 1, 3) });
            Add(new Floor() { position = new(0, 1, 4) });
            Add(new Floor() { position = new(-3, 1, 0) });
            Add(new Floor() { position = new(-4, 1, 0) });
            Add(new Floor() { position = new(3, 1, 0) });
            Add(new Floor() { position = new(4, 1, 0) });

            Add(new Box() { position = new(-1, 1, -1) });
            Add(new Box() { position = new(-1, 1, 1) });
            Add(new Box() { position = new(1, 1, -1) });
            Add(new Box() { position = new(1, 1, 1) });

            Add(new Goal() { position = new(0, 2, -4) });
            Add(new Goal() { position = new(0, 2, 4) });
            Add(new Goal() { position = new(-4, 2, 0) });
            Add(new Goal() { position = new(4, 2, 0) });

            Add(new Vertex(0));
            Add(new Vertex(1));
            Add(new Vertex(2));
            Add(new Vertex(3));
            Add(new Edge(0));
            Add(new Edge(1));
            Add(new Edge(2));
            Add(new Edge(3));
            Add(new Face(0));
            Add(new Face(1));
            Add(new Face(2));
            Add(new Face(3));

            data.Add("Views.Default", new Vertex(0));

            string s = data.ToJson();
            Debug.Log(s);
            return s;
        }
        public static string Level2() {
            LevelData.Level data = new();

            void Add(object item) {
                data.Add("", item);
            }
            
            Add(new Player() { position = new(-1, 1, -1) });

            Add(new Floor() { position = new(0, 0, 0) });
            Add(new Floor() { position = new(0, 0, -1) });
            Add(new Floor() { position = new(0, 0, 1) });
            Add(new Floor() { position = new(-1, 0, 0) });
            Add(new Floor() { position = new(-1, 0, -1) });
            Add(new Floor() { position = new(-1, 0, 1) });
            Add(new Floor() { position = new(1, 0, 0) });
            Add(new Floor() { position = new(1, 0, -1) });
            Add(new Floor() { position = new(1, 0, 1) });
            Add(new Floor() { position = new(0, 1, -3) });
            Add(new Floor() { position = new(0, 1, -4) });
            Add(new Floor() { position = new(0, 1, 3) });
            Add(new Floor() { position = new(0, 1, 4) });
            Add(new Floor() { position = new(-3, 1, 0) });
            Add(new Floor() { position = new(-4, 1, 0) });
            Add(new Floor() { position = new(3, 1, 0) });
            Add(new Floor() { position = new(4, 1, 0) });
            Add(new Goal() { position = new(-10, -10, -10) });

            Add(new Box() { position = new(0, 1, -1) });
            Add(new Box() { position = new(1, 1, -1) });
            Add(new Floor() { position = new(2, 0, -1) });

            Add(new Box() { position = new(3, 2, 0) });

            Add(new Floor() { position = new(2, 2, 4) });
            Add(new Floor() { position = new(0, 2, 5) });
            Add(new Floor() { position = new(1, 2, 3) });
            Add(new Floor() { position = new(-1, 0, -2) });

            Add(new Vertex(0));
            Add(new Vertex(1));
            Add(new Vertex(2));
            Add(new Vertex(3));
            Add(new Edge(0));
            Add(new Edge(1));
            Add(new Edge(2));
            Add(new Edge(3));
            Add(new Face(0));
            Add(new Face(1));
            Add(new Face(2));
            Add(new Face(3));

            data.Add("Views.Default", new Face(0));

            string s = data.ToJson();
            Debug.Log(s);
            return s;
        }
    }
}
