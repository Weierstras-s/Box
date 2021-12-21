using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Templates.FSM;
using Stage.GameStates;
using Stage.Objects;
using Stage.Views;

namespace Stage {
    public class TestScript : MonoBehaviour {
        public static string Level2() {
            LevelData.Level data = new();

            void Add(object item) {
                data.Add("Objects", item);
            }
            
            Add(new Goal() { position = new(100000, 100000, 100000) });
            Add(new Player() { position = new(0, 1, 0) });

            Add(new Floor() { position = new(-3, 0, 0) });
            Add(new Floor() { position = new(-4, 0, 0) });
            Add(new Effect() {
                position = new(-4, 1, 0),
                script = "SwitchScene",
                args = new() {
                    { "name", "Editor" }
                }
            });
            for (int i = -2; i <= 2; ++i) {
                for (int j = -2; j <= 2; ++j) {
                    Add(new Floor() { position = new(i, 0, j) });
                }
            }
            for(int i = 3; i <= 30; ++i) {
                Add(new Floor() { position = new(i, 0, 0) });
            }

            for(int i = 1; i <= 5; ++i) {
                int x = 3 * i + 2;
                Add(new Box() { position = new(x, 1, 1) });
                Add(new Effect() {
                    position = new(x, 1, 2),
                    script = "SelectLevel",
                    args = new() {
                        { "name", $"{i}" },
                    }
                });
                Add(new Floor() { position = new(x, 0, 1) });
                Add(new Floor() { position = new(x, 0, 2) });
                Add(new Effect() {
                    position = new(x, 1, 0),
                    script = "ShowInfo",
                    args = new() {
                        { "msg", $"Level {i}" },
                        { "pos", $"{x},1,2" },
                        { "offset", "45" },
                    }
                });
            }

            data.Add("Objects.Controlled", new ControlledFloor() { position = new(0, 0, 3), id = "0" });
            data.Add("Objects.Controlled", new ControlledFloor() { position = new(1, 0, 3), id = "1", active = true });
            Add(new Effect() {
                position = new(1, 1, 0),
                script = "ButtonX",
                args = new() {
                    { "id", "1" },
                }
            });
            Add(new Effect() {
                position = new(-1, 1, 0),
                script = "ButtonO",
                args = new() {
                    { "id", "0" },
                }
            });

            data.Add("Views", new Face(0));
            data.Add("Views", new Face(1));
            data.Add("Views", new Face(2));
            data.Add("Views", new Face(3));

            data.Add("Views.Default", new Face(0));

            string s = data.ToJson();
            Debug.Log(s);
            return s;
        }
    }
}
