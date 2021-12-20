using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.LevelData;
using Stage.Objects;
using UnityEngine.EventSystems;

namespace Editor {
    public class CreateCube : MonoBehaviour {
        // Start is called before the first frame update
        public GameObject Box;
        public GameObject Floor;
        public GameObject Player;
/*        public GameObject BoxButton;
        public GameObject FloorButton;
        public GameObject PlayerButton;*/

        /*public GameObject Plane;*/
        private GameObject Precube;
        int scale = 1;
        Vector3 position;   //新建物体的位置
        Vector3 postransform;   //position的减去1/4scale的版本
        Vector3 deltaRay;  //射线反射方向
        private Vector3 target;
        private int MouseState = 0;    //0:无，1：检测到第一次单击落下（计时），2：检测到第一次单击抬起（计时），3：检测到第二次单击落下（计时）
        private bool rightclick = false;
        private bool createcube = false;
        private bool destroycube = false;
        private float mousetimer = 0f;
        private float mousetimer2 = 0f;
        private float mouseinterval = 8f;
        private float fixupdatetime = 0.02f;
        private float heightmin = 0f;   //当前最低点
        private Dictionary<Vector3Int, BaseObj> items = new();
        private Dictionary<Vector3Int, GameObject> gameobjects = new(); //position是转换后的离散位置
        private Level currLevel_l = new();
        private string currLevel_s;

        void Start() {
            position.x = 0;
            position.y = 0.5f;
            position.z = 5;
            Precube = Floor;    //默认初始化是Floor
        }
        public void ChangeToBox() {
            Precube = Box;
        }

        public void ChangeToFloor() {
            Precube = Floor;
        }

        public void ChangeToPlayer() {
            Precube = Player;
        }

        public void SaveToJson() {  //Save操作
            Level level = new();
            foreach (KeyValuePair<Vector3Int, BaseObj> item in items) {
               level.Add("", item.Value);
            }
            currLevel_l = level;
            currLevel_s = level.ToJson();
            Debug.Log("输出结果：" + currLevel_s);
        }

        public void LoadFromJson() {    //Load操作
            Debug.Log("加载关卡");
            /* 先消除当前obj */
            foreach (KeyValuePair<Vector3Int, BaseObj> item in items) {
                Destroy(gameobjects[item.Key].gameObject);
            }
            gameobjects.Clear();
            items.Clear();

            currLevel_l = Level.FromJson(currLevel_s);

            List < Box > boxlist = currLevel_l.Find<Box>("");
            ChangeToBox();
            for(int i = 0; i < boxlist.Count; i++) {
                position.x = boxlist[i].position.x + (float)scale / 2;
                position.y = boxlist[i].position.y + (float)scale / 2;
                position.z = boxlist[i].position.z + (float)scale / 2;

                Debug.Log("Load的Box规格化坐标为：" + position);
                GameObject currItem = Instantiate(Precube, position, Quaternion.identity);
                gameobjects.Add(boxlist[i].position, currItem);
                items.Add(boxlist[i].position, boxlist[i]);
            }


            List<Player> playerlist = currLevel_l.Find<Player>("");
            ChangeToPlayer();
            for (int i = 0; i < playerlist.Count; i++) {
                position.x = playerlist[i].position.x + (float)scale / 2;
                position.y = playerlist[i].position.y + (float)scale / 2;
                position.z = playerlist[i].position.z + (float)scale / 2;

                Debug.Log("Load的Box规格化坐标为：" + position);
                GameObject currItem = Instantiate(Precube, position, Quaternion.identity);
                gameobjects.Add(playerlist[i].position, currItem);
                items.Add(playerlist[i].position, playerlist[i]);
            }

            List<Floor> floorlist = currLevel_l.Find<Floor>("");
            ChangeToFloor();
            for (int i = 0; i < floorlist.Count; i++) {
                position.x = floorlist[i].position.x + (float)scale / 2;
                position.y = floorlist[i].position.y + (float)scale / 2;
                position.z = floorlist[i].position.z + (float)scale / 2;

                Debug.Log("Load的Box规格化坐标为：" + position);
                GameObject currItem = Instantiate(Precube, position, Quaternion.identity);
                gameobjects.Add(floorlist[i].position, currItem);
                items.Add(floorlist[i].position, floorlist[i]);
            }


        }

        void Update() {
            /* 鼠标点击事件状态机——射线检测 */
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
            RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //发出射线检测到了碰撞   isHit返回的是 一个bool值
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            deltaRay.x = (float)((float)Math.Sign(ray.direction.x) * -0.01);
            deltaRay.y = (float)((float)Math.Sign(ray.direction.y) * -0.01);
            deltaRay.z = (float)((float)Math.Sign(ray.direction.z) * -0.01);
            //Debug.Log("delta：" + deltaRay);
            if (!EventSystem.current.IsPointerOverGameObject()) {
                if (MouseState == 0 && Input.GetMouseButtonDown(0))    //鼠标左键第一次落下
                {
                    MouseState = 1;
                    mousetimer = (float)(fixupdatetime * mouseinterval);
                }
                if (MouseState == 1 && Input.GetMouseButtonUp(0)) {
                    MouseState = 2;
                    mousetimer = (float)(fixupdatetime * mouseinterval / 100);
                }
                if (MouseState == 2 && Input.GetMouseButtonDown(0)) {
                    MouseState = 3;
                    mousetimer = (float)(fixupdatetime * mouseinterval);
                }
                if (MouseState == 3 && Input.GetMouseButtonUp(0)) {
                    MouseState = 0;
                    destroycube = true;
                }
                if (rightclick == false && Input.GetMouseButtonDown(1)) {   //鼠标右键第一次落下
                    rightclick = true;
                    mousetimer2 = (float)(fixupdatetime * mouseinterval);
                }
            }

            if (createcube) {
                if (isHit) {
                    Debug.Log("坐标为：" + hit.point);
                    target = hit.point; //检测到碰撞，就把检测到的点记录下来
                    position.x = (int)Math.Floor(target.x + deltaRay.x) / (int)scale * scale + (float)scale / 2;
                    position.y = (int)Math.Floor(target.y + deltaRay.y) / (int)scale * scale + (float)scale / 2;
                    position.z = (int)Math.Floor(target.z + deltaRay.z) / (int)scale * scale + (float)scale / 2;
                    
                    Debug.Log("规格化坐标为：" + position);
                    GameObject currItem = Instantiate(Precube, position, Quaternion.identity);

                    postransform.x = position.x - (float)scale / 4;
                    postransform.y = position.y - (float)scale / 4;
                    postransform.z = position.z - (float)scale / 4;

                    gameobjects.Add(Vector3Int.RoundToInt(postransform), currItem);

                   /* heightmin = Math.Min(heightmin, position.y - (float)scale / 2);*/

                    if(Precube == Floor) {
                        Floor newitem = new() {
                            position = Vector3Int.RoundToInt(postransform)
                        };

                        items.Add(newitem.position, newitem);

                        Debug.Log("新建了Floor：保存坐标为：" + Vector3Int.RoundToInt(postransform));
                    }
                    else if(Precube == Box) {
                        Box newitem = new() {
                            position = Vector3Int.RoundToInt(postransform)
                        };

                        items.Add(newitem.position, newitem);

                        Debug.Log("新建了Box：保存坐标为：" + Vector3Int.RoundToInt(postransform));

                    }
                    else if (Precube == Player) {
                        Player newitem = new() {
                            position = Vector3Int.RoundToInt(postransform)
                        };

                        items.Add(newitem.position, newitem);

                        Debug.Log("新建了Player：保存坐标为：" + Vector3Int.RoundToInt(postransform));
                    } else {

                    }
                    
                }
                createcube = false;
            }

            if (destroycube || rightclick) {
                if (isHit) {
                    Debug.Log("双击坐标为：" + hit.point);
                    target = hit.point; //检测到碰撞，就把检测到的点记录下来
                    if(hit.collider.gameObject.tag == Precube.tag)
                        Destroy(hit.collider.gameObject);
                    position.x += 1;
                }
                postransform.x = hit.collider.gameObject.transform.position.x - (float)scale / 4;
                postransform.y = hit.collider.gameObject.transform.position.y - (float)scale / 4;
                postransform.z = hit.collider.gameObject.transform.position.z - (float)scale / 4;

                Vector3Int rmPosition = Vector3Int.RoundToInt(postransform);
                items.Remove(rmPosition);

                destroycube = false;
                rightclick = false;
            }


        }

        private void FixedUpdate() {
            if(MouseState == 1) {
                mousetimer -= 0.02f;
                if(mousetimer < 0f) {
                    mousetimer = 0f;
                    MouseState = 0;
                }
            }
            if(MouseState == 2) {
                mousetimer -= 0.02f;
                if(mousetimer < 0f) {
                    mousetimer = 0f;
                    MouseState = 0;
                    createcube = true;
                }
            }
            if(MouseState == 3) {
                mousetimer -= 0.02f;
                if (mousetimer < 0f) {
                    mousetimer = 0f;
                    MouseState = 0;
                }
            }
            if (rightclick) {
                mousetimer2 -= 0.02f;
                if(mousetimer2 < 0f) {
                    mousetimer2 = 0;
                    rightclick = false;
                }
            }
        }

    }
}