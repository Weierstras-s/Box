using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour {
    public Point curpos;

    private readonly int[] dx = { 1, 0, -1, 0 };
    private readonly int[] dz = { 0, 1, 0, -1 };

    //移动控制
    private readonly float speed = 4;

    public static bool isMoving = false;
    private Vector3 movePos;
    private Vector3 oriPos;
    private Vector3 boxPos;
    private Vector3 oriBoxPos;
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

    private void StartMove(Point p, int h, Point boxS, Point boxT) {
        isMoving = true;
        oriPos = transform.position;
        movePos = p.PointToVec();
        oriBoxPos = boxS.PointToVec();
        boxPos = boxT.PointToVec();
        moveDis = 0;
        boxHeight = h;
    }
    private void StartMove(Point p) {
        isMoving = true;
        oriPos = transform.position;
        movePos = p.PointToVec();
        moveDis = 0;
        boxHeight = 0;
    }
    private void Move() {
        //移动玩家
        transform.position = oriPos + (movePos - oriPos) * moveDis;
        //移动箱子
        for (int i = 0; i < boxHeight; i++) {
            Vector3 np = oriBoxPos + new Vector3(0, i, 0);
            Vector3 nbp = boxPos + new Vector3(0, i, 0);
            GameObject box = GameObject.Find("Box " + new Point().VecToPoint(nbp).ToString());
            box.transform.position = np + (nbp - np) * moveDis;
        }

        moveDis += speed * Time.deltaTime;

        if (moveDis >= 1) {
            transform.position = movePos;
            for (int i = 0; i < boxHeight; i++) {
                Vector3 nbp = boxPos + new Vector3(0, i, 0);
                GameObject box = GameObject.Find("Box " + new Point().VecToPoint(nbp).ToString());
                box.transform.position = nbp;
            }
            isMoving = false;
        }
    }

    private Stack<Point> movements = new Stack<Point>();
    private void CalcPath(Point S, Point T) {
        movements.Clear();
        Queue<Point> q = new Queue<Point>();
        Dictionary<Point, int> dis = new Dictionary<Point, int>();
        Dictionary<Point, Point> pre = new Dictionary<Point, Point>();
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
                if (dis.TryGetValue(nxt, out int val)) {
                    if (val > curdis + 1) {
                        dis[nxt] = curdis + 1;
                        pre[nxt] = cur;
                        if (!vis.Contains(nxt)) {
                            vis.Add(nxt);
                            q.Enqueue(nxt);
                        }
                    }
                } else {
                    dis.Add(nxt, curdis + 1);
                    pre.Add(nxt, cur);
                    if (!vis.Contains(nxt)) {
                        vis.Add(nxt);
                        q.Enqueue(nxt);
                    }
                }
                vis.Remove(cur);
            }
        }
        if (dis.TryGetValue(T, out int dist)) {
            Point p = T;
            while (pre.TryGetValue(p, out Point tmp)) {
                movements.Push(p);
                p = tmp;
            }
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
                string name = hit.collider.name;
                if (name[0] != 'C' && name[0] != 'B') return;
                Point position = new Point(name.Split(' ')[1]);
                
                CalcPath(curpos, position.Up());
            }
        }

        int key = Getkey();
        if (!Input.GetKey(key: KeyCode.LeftShift) && !Input.GetKey(key: KeyCode.RightShift) && key != -1) {
            if (Launcher.map.NextPoint(key, curpos, out Point nxt)) {
                if (!Launcher.map.GetBox(nxt)) {
                    //无箱子直接推
                    movements.Clear();
                    movements.Push(nxt);
                }
            }
        }

        if (isMoving) {
            Move();
            return;
        }
        if (movements.Count > 0) {
            Point next = movements.Pop();
            StartMove(next);
            curpos = next;
            return;
        }
        transform.position = curpos.PointToVec();

        if (Input.GetKey(key: KeyCode.LeftShift) || Input.GetKey(key: KeyCode.RightShift)) return;
        if (key == -1) return;
        if (Launcher.map.NextPoint(key, curpos, out Point nextPoint)) {
            if (Launcher.map.GetBox(nextPoint)) {
                //前方有箱子
                if (Launcher.map.NextPoint(key, nextPoint, out Point nextBoxPoint)) {
                    int h = 0;
                    for (; Launcher.map.GetBox(new Point(0, h, 0) + nextPoint); h++) {
                        Point nbup = nextBoxPoint + new Point(0, h, 0);
                        if (Launcher.map.GetPos(nbup)) {
                            //箱子的前面有箱子
                            return;
                        }
                    }

                    //可以推
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
                    StartMove(curpos, h, nextPoint, nextBoxPoint);
                }
                Launcher.map.AddEdge();
            }
        }
    }
}
