using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Direction: X+:0; Z+:1; X-:2; Z-:3;

public class Cons {
    public const int maxH = 100;
    public const int maxV = 50;
}
public class Point2 {
    public int x, y;
    public Point2() { }
    public Point2(int px, int py) {
        x = px; y = py;
    }

    public static Point2 operator +(Point2 a, Point2 b) {
        return new Point2(a.x + b.x, a.y + b.y);
    }
    public static bool operator <(Point2 a, Point2 b) {
        return a.x < b.x || a.x == b.x && a.y < b.y;
    }
    public static bool operator >(Point2 a, Point2 b) {
        return !(a < b);
    }
    public static bool operator ==(Point2 a, Point2 b) {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Point2 a, Point2 b) {
        return !(a == b);
    }
    public override bool Equals(object obj) {
        return this == (Point2)obj;
    }
    public override int GetHashCode() {
        return x * Cons.maxV * 2 + y;
    }
    public override string ToString() {
        return x.ToString() + "," + y.ToString();
    }
}
public class Point {
    public int x, y, z;
    public Point() {
        x = 0; y = 0; z = 0;
    }
    public Point(int px, int py, int pz) {
        x = px; y = py; z = pz;
    }
    public Point(string str) {
        string[] vs = str.Split(',', ' ');
        x = int.Parse(vs[0]);
        y = int.Parse(vs[1]);
        z = int.Parse(vs[2]);
    }

    public static Point operator +(Point a, Point b) {
        return new Point(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static bool operator <(Point a, Point b) {
        if (a.x < b.x) return true;
        if (a.z < b.z) return true;
        return a.y < b.y;
    }
    public static bool operator >(Point a, Point b) {
        return !(a < b);
    }
    public static bool operator ==(Point a, Point b) {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    public static bool operator !=(Point a, Point b) {
        return !(a==b);
    }
    public override bool Equals(object obj) {
        return this == (Point)obj;
    }
    public override int GetHashCode() {
        return x * Cons.maxH * Cons.maxV * 4 + z * Cons.maxV * 2 + y;
    }
    public override string ToString() {
        return x.ToString() + "," + y.ToString() + "," + z.ToString();
    }

    //取该点上方或下方的点
    public Point Up() {
        return this + new Point(0, 1, 0);
    }
    public Point Down() {
        return this + new Point(0, -1, 0);
    }

    //点与向量转换
    public Point VecToPoint(Vector3 vec) {
        x = Mathf.RoundToInt(vec.x); y = Mathf.RoundToInt(vec.y); z = Mathf.RoundToInt(vec.z);
        return new Point(x, y, z);
    }
    public Vector3 PointToVec() {
        return new Vector3(x, y, z);
    }
    //点与字符串转换
    public void StrToPoint(string str) {
        string[] vs = str.Split(',', ' ');
        x = int.Parse(vs[0]);
        y = int.Parse(vs[1]);
        z = int.Parse(vs[2]);
    }

    //不同视角下点的转换
    public Point Transfer(int dir) {
        if (dir == 3) return new Point(z, y, -x);
        if (dir == 2) return new Point(-x, y, -z);
        if (dir == 1) return new Point(-z, y, x);
        return new Point(x, y, z);
    }
    //点的平面坐标
    public Point2 Plain() {
        return new Point2(x + y, z + y);
    }
}
class PointCompare : IComparer<Point> {
    public int Compare(Point x, Point y) {
        return x.ToString().CompareTo(y.ToString());
    }
}
public class PointsS {
    private const int h = Cons.maxH * 2, v = Cons.maxV * 2;
    private bool[,,] map = new bool[h * 2, v * 2, h * 2];
    public List<Point> list = new List<Point>();
    public void Add(int x, int y, int z) {
        map[x + h, y + v, z + h] = true;
        list.Add(new Point(x, y, z));
    }
    public void Add(Point p) {
        Add(p.x, p.y, p.z);
    }
    public void Remove(int x, int y, int z) {
        map[x + h, y + v, z + h] = false;
        list.Remove(new Point(x, y, z));
    }
    public void Remove(Point p) {
        Remove(p.x, p.y, p.z);
    }
    public bool Contains(int x, int y, int z) {
        return map[x + h, y + v, z + h];
    }
    public bool Contains(Point p) {
        return Contains(p.x, p.y, p.z);
    }
    public bool ContainsT(int x, int y, int z, int dir) {
        return Contains(new Point(x, y, z).Transfer((4 - dir) % 4));
    }
    public bool ContainsT(Point p,int dir) {
        return Contains(p.Transfer((4 - dir) % 4));
    }
    public void Clear() {
        list.Clear();
        for (int i = 0; i < h * 2; i++) {
            for (int j = 0; j < v * 2; j++) {
                for (int k = 0; k < h * 2; k++) {
                    map[i, j, k] = false;
                }
            }
        }
    }
}
public class Map {
    public int direction;

    PointsS points = new PointsS();
    public Dictionary<Point, Dictionary<int, Point>> edges = new Dictionary<Point, Dictionary<int, Point>>();
    public List<Point> boxes = new List<Point>();
    public List<Point> goals = new List<Point>();

    public void Init() {
        direction = 0;
        points.Clear();
        edges.Clear();
        boxes.Clear();
        goals.Clear();
    }

    //增减地面
    public void SetPos(int x, int y, int z) {
        points.Add(new Point(x, y, z));
    }
    public void SetPos(Point p) {
        SetPos(p.x, p.y, p.z);
    }
    public void DelPos(int x, int y, int z) {
        points.Remove(new Point(x, y, z));
    }
    public void DelPos(Point p) {
        DelPos(p.x, p.y, p.z);
    }
    //增减箱子
    public void SetBox(int x, int y, int z) {
        SetPos(x, y, z);
        boxes.Add(new Point(x, y, z));
    }
    public void SetBox(Point p) {
        SetBox(p.x, p.y, p.z);
    }
    public void DelBox(int x, int y, int z) {
        DelPos(x, y, z);
        boxes.Remove(new Point(x, y, z));
    }
    public void DelBox(Point p) {
        DelBox(p.x, p.y, p.z);
    }
    //增加目标
    public void SetGoal(int x, int y, int z) {
        goals.Add(new Point(x, y, z));
    }
    public void SetGoal(Point p) {
        SetGoal(p.x, p.y, p.z);
    }

    //查找元素
    public bool GetPos(int x, int y, int z) {
        return points.Contains(new Point(x, y, z));
    }
    public bool GetPos(Point p) {
        return GetPos(p.x, p.y, p.z);
    }
    public bool GetBox(int x, int y, int z) {
        return boxes.Contains(new Point(x, y, z));
    }
    public bool GetBox(Point p) {
        return GetBox(p.x, p.y, p.z);
    }

    //是否能走
    private bool Walkable(Point p) {
        if (points.Contains(p.Up())) return boxes.Contains(p.Up());
        return true;
    }

    //判断是否遮挡
    public bool Blocked(Point ori, Point to, int dir) {
        ori = ori.Transfer(direction);
        to = to.Transfer(direction);
        if (dir > 1) {
            //dir -= 2;
            Point tmp = to;
            to = ori;
            ori = tmp;
        }
        if (ori.y == to.y) return false;
        int x = ori.x, y = ori.y, z = ori.z;
        if (dir % 2 == 0) {
            for (int i = 0; i < ori.y - to.y; i++) {
                if (points.ContainsT(x + 1 + i, y - i, z + 1 + i, direction)) return true;
                if (points.ContainsT(x + 1 + i, y - i, z + i, direction)) return true;
                if (points.ContainsT(x + 2 + i, y - i, z + 1 + i, direction) && i != ori.y - to.y - 1) return true;
            }
        } else {
            for (int i = 0; i < ori.y - to.y; i++) {
                if (points.ContainsT(x + 1 + i, y - i, z + 1 + i, direction)) return true;
                if (points.ContainsT(x + i, y - i, z + 1 + i, direction)) return true;
                if (points.ContainsT(x + 1 + i, y - i, z + 2 + i, direction) && i != ori.y - to.y - 1) return true;
            }
        }
        return false;
    }
    //建图
    public void AddEdge() {
        edges.Clear();
        /*
         * X+:(1,0); Z+:(0,1); X-:(-1,0) Z-:(0,-1)
         */

        foreach (Point p in points.list) {
            if (!Walkable(p)) continue;

            Point2 nxtp = new Point2();
            Dictionary<int, Point> d = new Dictionary<int, Point>();
            Point point = new Point();
            int maxy;
            bool b;

            //X+
            nxtp = p.Transfer(direction).Plain() + new Point2(1, 0);
            maxy = -Cons.maxV;
            b = false;
            foreach (Point q in points.list) {
                if (!Walkable(q)) continue;
                if (q.y <= p.y && q.y > maxy && nxtp == q.Transfer(direction).Plain()) {
                    if (Blocked(p, q, 0)) continue;
                    point = q;
                    maxy = q.y;
                    b = true;
                }
            }
            if (b) d.Add(0, point);

            //Z+
            nxtp = p.Transfer(direction).Plain() + new Point2(0, 1);
            maxy = -Cons.maxV;
            b = false;
            foreach (Point q in points.list) {
                if (!Walkable(q)) continue;
                if (q.y <= p.y && q.y > maxy && nxtp == q.Transfer(direction).Plain()) {
                    if (Blocked(p, q, 1)) continue;
                    point = q;
                    maxy = q.y;
                    b = true;
                }
            }
            if (b) d.Add(1, point);

            //X-
            nxtp = p.Transfer(direction).Plain() + new Point2(-1, 0);
            maxy = -Cons.maxV;
            b = false;
            foreach (Point q in points.list) {
                if (!Walkable(q)) continue;
                if (q.y >= p.y && q.y > maxy && nxtp == q.Transfer(direction).Plain()) {
                    if (Blocked(q, p, 0)) continue;
                    point = q;
                    maxy = q.y;
                    b = true;
                }
            }
            if (b) d.Add(2, point);

            //Z-
            nxtp = p.Transfer(direction).Plain() + new Point2(0, -1);
            maxy = -Cons.maxV;
            b = false;
            foreach (Point q in points.list) {
                if (!Walkable(q)) continue;
                if (q.y >= p.y && q.y > maxy && nxtp == q.Transfer(direction).Plain()) {
                    if (Blocked(q, p, 1)) continue;
                    point = q;
                    maxy = q.y;
                    b = true;
                }
            }
            if (b) d.Add(3, point);
            edges.Add(p, d);
        }
    }
    //下一步
    public bool NextPoint(int dir, Point cur, out Point point) {
        point = new Point();
        if (!Launcher.map.edges.TryGetValue(cur.Down(), out Dictionary<int, Point> d)) return false;
        if (!d.TryGetValue(dir, out point)) return false;
        point = point.Up();
        return true;
    }
    //判断结束
    public bool IsWin() {
        foreach (Point p in goals) {
            if (!boxes.Contains(p)) return false;
        }
        return true;
    }
}

public class Launcher : MonoBehaviour {
    public GameObject cubePrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    static public Map map = new Map();

    private readonly int maxLevel = 3;
    private int curLevel = 1;

    //编辑地图
    public void AddCube(int x, int y, int z) {
        GameObject g = Instantiate(cubePrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
        g.name = "Cube " + new Point(x, y, z).ToString();
        map.SetPos(x, y, z);
    }
    public void AddCube(Point p) {
        AddCube(p.x, p.y, p.z);
    }
    public void AddBox(int x, int y, int z) {
        GameObject g = Instantiate(boxPrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
        g.name = "Box " + new Point(x, y, z).ToString();
        map.SetBox(x, y, z);
    }
    public void AddBox(Point p) {
        AddBox(p.x, p.y, p.z);
    }
    public void AddGoal(int x,int y,int z) {
        GameObject g = Instantiate(goalPrefab, new Vector3(x, y - 0.5f, z), new Quaternion(0, 0, 0, 0));
        g.name = "Goal " + new Point(x, y, z).ToString();
        map.SetGoal(x, y, z);
    }
    public void AddGoal(Point p) {
        AddGoal(p.x, p.y, p.z);
    }
    public void SetStart(int x, int y, int z) {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<Player>().curpos = new Point(x, y, z);
    }
    public void SetStart(Point p) {
        SetStart(p.x, p.y, p.z);
    }

    //加载关卡
    public void LoadLevel(int level) {
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject g in boxes) Destroy(g);
        foreach (GameObject g in goals) Destroy(g);
        foreach (GameObject g in floors) Destroy(g);
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<MCamera>().currot = 45;
        camera.GetComponent<MCamera>().rotation = 45;
        
        map.Init();
        string line;
        string path = System.Environment.CurrentDirectory + @"\Levels\";
        System.IO.StreamReader file =
            new System.IO.StreamReader(path + level.ToString() + ".txt");
        int mode = 0;
        /* 
         * 0: SetStart
         * 1: AddCube
         * 2: AddBox
         * 3: AddGoal
         */
        while ((line = file.ReadLine()) != null) {
            if (line == string.Empty) continue;
            System.Console.WriteLine(line);
            switch (line[0]) {
                case '/': break;
                case 'S': mode = 0; break;
                case 'C': mode = 1; break;
                case 'B': mode = 2; break;
                case 'G': mode = 3; break;
                default:
                    Point p = new Point(line);
                    switch (mode) {
                        case 0: SetStart(p); break;
                        case 1: AddCube(p); break;
                        case 2: AddBox(p); break;
                        case 3: AddGoal(p); break;
                    }
                    break;
            }
        }
        map.AddEdge();
        file.Close();
    }

    private void Start() {
        LoadLevel(curLevel);
    }

    private void Update() {
        if(Input.GetKeyDown(key: KeyCode.R)) {
            LoadLevel(curLevel);
        }

        if (Player.isMoving) return;

        if (map.IsWin()) {
            //获胜
            curLevel++;
            if (curLevel > maxLevel) {
                //全部通关
                curLevel = 1;
            }
            LoadLevel(curLevel);
        }
    }
}
