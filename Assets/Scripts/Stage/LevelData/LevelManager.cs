using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.Objects;
using Stage.Views;

namespace Stage.LevelData {
    public class LevelManager {
        public static Level LoadFromFile(string name) {
            var level = Resources.Load<TextAsset>("Levels/" + name).text;
            return Level.FromJson(level, name);
        }

        public static Level MainLevel() {
            //return Level.FromJson(TestScript.Level2(), "main");
            return LoadFromFile("main");
        }
        public static Level NextLevel(Level current) {
            var file = current.FindOne<string>("General.NextLevel");
            if (file == null) return MainLevel();
            return LoadFromFile(file);
        }
        public static Level PrevLevel(Level current) {
            var file = current.FindOne<string>("General.PrevLevel");
            if (file == null) return MainLevel();
            return LoadFromFile(file);
        }
    }
}
