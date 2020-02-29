using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCamera : MonoBehaviour {
    public Material mat;
    public void DrawLine(Point p1, Point p2) {
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        Vector2 v1 = Camera.main.WorldToScreenPoint(p1.Vec + new Vector3(0, 0.5f));
        Vector2 v2 = Camera.main.WorldToScreenPoint(p2.Vec + new Vector3(0, 0.5f));
        GL.Vertex3(v1.x / Screen.width, v1.y / Screen.height, 0);
        GL.Vertex3(v2.x / Screen.width, v2.y / Screen.height, 0);
        GL.End();
        GL.PopMatrix();
    }
    public void Paint(Map map) {
        Dictionary<int, Point> d = new Dictionary<int, Point>();
        foreach (KeyValuePair<Point, Dictionary<int, Point>> keyValuePair in map.edges) {
            d = keyValuePair.Value;
            foreach (KeyValuePair<int, Point> p in d) {
                DrawLine(keyValuePair.Key, p.Value);
                //Debug.Log(keyValuePair.Key.ToString() + "  " + p.Value.ToString() + " :" + p.Key.ToString());
            }
        }
    }
    private void OnPostRender() {
        Paint(Launcher.map);
    }

    public float currot;
    private Vector3 curpos;
    public float rotation;
    private Vector3 position;
    private readonly float dis = 20;

    private int Getkey() {
        if (Input.GetKeyDown(key: KeyCode.RightArrow)) return 0;
        if (Input.GetKeyDown(key: KeyCode.UpArrow)) return 1;
        if (Input.GetKeyDown(key: KeyCode.LeftArrow)) return 2;
        if (Input.GetKeyDown(key: KeyCode.DownArrow)) return 3;
        return -1;
    }

    private void Start() {
        currot = 45;
        rotation = 45;
    }
    private float Sin(float deg) {
        return Mathf.Sin(Mathf.Deg2Rad * deg);
    }
    private float Cos(float deg) {
        return Mathf.Cos(Mathf.Deg2Rad * deg);
    }
    private void Update() {
        Vector3 rot = transform.eulerAngles;
        currot += (rotation - currot) / 4;
        transform.eulerAngles = new Vector3(rot.x, currot, rot.z);

        rot = transform.eulerAngles;
        GameObject player = GameObject.Find("Player");
        position = player.transform.position;
        curpos += (position - curpos) / 5;
        transform.position = curpos + new Vector3(-dis * Sin(rot.y), dis / Mathf.Sqrt(2), -dis * Cos(rot.y));

        if (Player.isMoving) return;
        if (Input.GetKey(key: KeyCode.LeftShift) || Input.GetKey(key: KeyCode.RightShift)) {
            int key = Getkey();
            if (key == 0) {
                rotation += 90;
                Launcher.map.direction = (Launcher.map.direction + 1) % 4;
                Launcher.map.AddEdge();
            } else if (key == 2) {
                rotation -= 90;
                Launcher.map.direction = (Launcher.map.direction + 3) % 4;
                Launcher.map.AddEdge();
            }
        }
    }
}
