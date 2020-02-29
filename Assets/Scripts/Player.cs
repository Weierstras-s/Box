using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {
    public Point curpos;

    private readonly int[] dx = { 1, 0, -1, 0 };
    private readonly int[] dz = { 0, 1, 0, -1 };

    //移动控制
    private readonly float speed = 3.5f;

    public static bool isMoving = false;

    //玩家
    private Vector3 movePos;
    private Vector3 oriPos;
    private Vector3 goalPos;

    //箱子
    private Vector3 boxMovePos;
    private Vector3 boxOriPos;
    private Vector3 boxGoalPos;

    private float moveDis = 0;
    private int boxHeight = 0;

    private void Start() {

    }

    private int Getkey() {
        if (Input.GetKeyDown(key: KeyCode.RightArrow)) return 0;
        if (Input.GetKeyDown(key: KeyCode.UpArrow)) return 1;
        if (Input.GetKeyDown(key: KeyCode.LeftArrow)) return 2;
        if (Input.GetKeyDown(key: KeyCode.DownArrow)) return 3;
        return -1;
    }

    private void SetMove(Point p, int dir) {
        isMoving = true;
        boxHeight = 0;
        moveDis = 0;

        oriPos = transform.position;
        goalPos = p.Vec;
        Vector3 delta = new Vector3(dx[(dir + 4 - Launcher.map.direction) % 4], 0, dz[(dir + 4 - Launcher.map.direction) % 4]);
        if (oriPos.y >= goalPos.y) {
            movePos = oriPos + delta;
        } else {
            oriPos = goalPos - delta;
            movePos = goalPos;
        }
    }
    private void SetMove(Point p, int dir, int h, Point boxS, Point boxT) {
        SetMove(p, dir);
        boxHeight = h;

        boxOriPos = boxS.Vec;
        boxGoalPos = boxT.Vec;
        Vector3 delta = new Vector3(dx[(dir + 4 - Launcher.map.direction) % 4], 0, dz[(dir + 4 - Launcher.map.direction) % 4]);
        if (boxOriPos.y >= boxGoalPos.y) {
            boxMovePos = boxOriPos + delta;
        } else {
            boxOriPos = boxGoalPos - delta;
            boxMovePos = boxGoalPos;
        }
    }
    private void Move() {
        //移动玩家
        transform.position = oriPos + (movePos - oriPos) * moveDis;
        //移动箱子
        for (int i = 0; i < boxHeight; i++) {
            Vector3 np = boxOriPos + new Vector3(0, i, 0);
            Vector3 nbp = boxGoalPos + new Vector3(0, i, 0);
            GameObject box = GameObject.Find("Box " + Point.VecToPoint(nbp).ToString());
            box.transform.position = np + (boxMovePos - boxOriPos) * moveDis;
        }

        moveDis += speed * Time.deltaTime;

        if (moveDis >= 1) {
            transform.position = goalPos;
            for (int i = 0; i < boxHeight; i++) {
                Vector3 nbp = boxGoalPos + new Vector3(0, i, 0);
                GameObject box = GameObject.Find("Box " + Point.VecToPoint(nbp).ToString());
                box.transform.position = nbp;
            }
            isMoving = false;
        }
    }

    private Stack<int> movements = new Stack<int>();
    private void CalcPath(Point S, Point T) {
        movements.Clear();
        Queue<Point> q = new Queue<Point>();
        Dictionary<Point, int> dis = new Dictionary<Point, int>();
        Dictionary<Point, Point> pre = new Dictionary<Point, Point>();
        Dictionary<Point, int> dir = new Dictionary<Point, int>();
        List<Point> vis = new List<Point>();
        q.Enqueue(S);
        vis.Add(S);
        dis.Add(S, 0);

        while (q.Count > 0) {
            Point cur = q.Dequeue();
            int curdis = dis[cur];
            for (int i = 0; i < 4; i++) {
                if (!Launcher.map.NextPoint(i, cur, out Point nxt)) continue;
                if (Launcher.map.GetBox(nxt)) continue;
                if (!dis.TryGetValue(nxt, out int val) || val > curdis + 1) {
                    dis[nxt] = curdis + 1;
                    pre[nxt] = cur;
                    dir[nxt] = i;
                    if (!vis.Contains(nxt)) {
                        vis.Add(nxt);
                        q.Enqueue(nxt);
                    }
                }
                vis.Remove(cur);
            }
        }

        if (dis.TryGetValue(T, out _)) {
            Point p = T;
            while (pre.TryGetValue(p, out Point tmp)) {
                movements.Push(dir[p]);
                p = tmp;
            }
        }
    }
    private bool Judge(int key) {
        if (Launcher.map.NextPoint(key, curpos, out Point nextPoint)) {
            if (!Launcher.map.GetBox(nextPoint)) {
                //前方无箱子直接推
                return true;
            } else {
                //前方有箱子
                if (Launcher.map.NextPoint(key, nextPoint, out Point nextBoxPoint)) {
                    int h = 0;
                    for (; Launcher.map.GetBox(new Point(0, h, 0) + nextPoint); h++) {
                        Point nbup = nextBoxPoint + new Point(0, h, 0);
                        if (Launcher.map.GetPos(nbup)) {
                            //箱子的前面有箱子
                            return false;
                        }
                    }
                    //可以推
                    return true;
                }
            }
        }
        return false;
    }
    private void StartMove(int key) {
        if (Launcher.map.NextPoint(key, curpos, out Point nextPoint)) {
            if (!Launcher.map.GetBox(nextPoint)) {
                //前方无箱子直接推
                curpos = nextPoint;
                SetMove(nextPoint, key);
            } else {
                //前方有箱子
                if (Launcher.map.NextPoint(key, nextPoint, out Point nextBoxPoint)) {
                    int h = 0;
                    for (; Launcher.map.GetBox(new Point(0, h, 0) + nextPoint); h++) ;
                    for (int i = 0; i < h; i++) {
                        Point np = nextPoint + new Point(0, i, 0);
                        Point nbp = nextBoxPoint + new Point(0, i, 0);
                        Launcher.map.DelBox(np);
                        Launcher.map.SetBox(nbp);
                        //更新场景位置
                        GameObject box = GameObject.Find("Box " + np.ToString());
                        box.name = "Box " + nbp.ToString();
                    }
                    
                    curpos = nextPoint;
                    SetMove(curpos, key, h, nextPoint, nextBoxPoint);
                }
                Launcher.map.AddEdge();
            }
        }
    }
    private void Update() {

        /*=====================================         Input         =====================================*/
        //点击移动
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                string name = hit.collider.name;
                if (name[0] != 'C' && name[0] != 'B') return;
                Point position = new Point(name.Split(' ')[1]);

                CalcPath(curpos, position.Up);
            }
        }
        //通过键盘移动
        int key = Getkey();
        if (!Input.GetKey(key: KeyCode.LeftShift) && !Input.GetKey(key: KeyCode.RightShift) && key != -1) {
            if (Judge(key)) {
                movements.Clear();
                movements.Push(key);
            }
        }

        /*=====================================       Animation       =====================================*/
        //移动动画
        if (isMoving) {
            Move();
            return;
        }

        /*=====================================    Queued Movements    =====================================*/
        //如果序列中有动作继续移动
        if (movements.Count > 0) {
            int next = movements.Pop();
            StartMove(next);
            return;
        }
        //静止
        transform.position = curpos.Vec;
    }
}
