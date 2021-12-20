using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stage.LevelData;
using Stage.Objects;
using UnityEngine.EventSystems;
using System.IO;

namespace LevelEditor {
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
        Vector3 position;   //�½������λ��
        Vector3 postransform;   //position�ļ�ȥ1/4scale�İ汾
        Vector3 deltaRay;  //���߷��䷽��
        private Vector3 target;
        private int MouseState = 0;    //0:�ޣ�1����⵽��һ�ε������£���ʱ����2����⵽��һ�ε���̧�𣨼�ʱ����3����⵽�ڶ��ε������£���ʱ��
        private bool rightclick = false;
        private bool createcube = false;
        private bool destroycube = false;
        private float mousetimer = 0f;
        private float mousetimer2 = 0f;
        private float mouseinterval = 8f;
        private float fixupdatetime = 0.02f;
        private float heightmin = 0f;   //��ǰ��͵�
        private Dictionary<Vector3Int, BaseObj> items = new();
        private Dictionary<Vector3Int, GameObject> gameobjects = new(); //position��ת�������ɢλ��
        private Level currLevel_l = new();
        private string currLevel_s;

        void Start() {
            position.x = 0;
            position.y = 0.5f;
            position.z = 5;
            Precube = Floor;    //Ĭ�ϳ�ʼ����Floor
        }


/*        public void WriteFileByLine(string file_path, string file_name, string str_info) {
        StreamWriter sw;
            File.CreateText(file_path + "//" + file_name);//����һ������д�� UTF-8 ������ı�
            Debug.Log("�ļ������ɹ���");
            sw = File.AppendText(file_path + "//" + file_name);//������ UTF-8 �����ı��ļ��Խ��ж�ȡ
            sw.WriteLine(str_info);//����Ϊ��λд���ַ���
            sw.Close();
            sw.Dispose();//�ļ����ͷ�
        }*/

        public void ChangeToBox() {
            Precube = Box;
        }

        public void ChangeToFloor() {
            Precube = Floor;
        }

        public void ChangeToPlayer() {
            Precube = Player;
        }

        public void SaveToJson() {  //Save����
            Level level = new();
            foreach (KeyValuePair<Vector3Int, BaseObj> item in items) {
               level.Add("", item.Value);
            }
            currLevel_l = level;
            currLevel_s = level.ToJson();
/*            WriteFileByLine("", "level"+"123", currLevel_s);*/
            Debug.Log(currLevel_s);
        }

        public void LoadFromJson() {    //Load����
            Debug.Log("���عؿ�");
            /* ��������ǰobj */
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

                Debug.Log("Load��Box�������Ϊ��" + position);
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

                Debug.Log("Load��Box�������Ϊ��" + position);
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

                Debug.Log("Load��Box�������Ϊ��" + position);
                GameObject currItem = Instantiate(Precube, position, Quaternion.identity);
                gameobjects.Add(floorlist[i].position, currItem);
                items.Add(floorlist[i].position, floorlist[i]);
            }


        }

        void Update() {
            /* ������¼�״̬���������߼�� */
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //��Ļ����ת����
            RaycastHit hit;                                                     //���߶����ǣ��ṹ�����ͣ��洢�������Ϣ��
            bool isHit = Physics.Raycast((Ray)ray, out hit);             //�������߼�⵽����ײ   isHit���ص��� һ��boolֵ
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            deltaRay.x = (float)((float)Math.Sign(ray.direction.x) * -0.01);
            deltaRay.y = (float)((float)Math.Sign(ray.direction.y) * -0.01);
            deltaRay.z = (float)((float)Math.Sign(ray.direction.z) * -0.01);
            //Debug.Log("delta��" + deltaRay);
            if (!EventSystem.current.IsPointerOverGameObject()) {
                if (MouseState == 0 && Input.GetMouseButtonDown(0))    //��������һ������
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
                if (rightclick == false && Input.GetMouseButtonDown(1)) {   //����Ҽ���һ������
                    rightclick = true;
                    mousetimer2 = (float)(fixupdatetime * mouseinterval);
                }
            }

            if (createcube) {
                if (isHit) {
                    Debug.Log("����Ϊ��" + hit.point);
                    target = hit.point; //��⵽��ײ���ͰѼ�⵽�ĵ��¼����
                    position.x = (int)Math.Floor(target.x + deltaRay.x) / (int)scale * scale + (float)scale / 2;
                    position.y = (int)Math.Floor(target.y + deltaRay.y) / (int)scale * scale + (float)scale / 2;
                    position.z = (int)Math.Floor(target.z + deltaRay.z) / (int)scale * scale + (float)scale / 2;
                    
                    Debug.Log("�������Ϊ��" + position);
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

                        Debug.Log("�½���Floor����������Ϊ��" + Vector3Int.RoundToInt(postransform));
                    }
                    else if(Precube == Box) {
                        Box newitem = new() {
                            position = Vector3Int.RoundToInt(postransform)
                        };

                        items.Add(newitem.position, newitem);

                        Debug.Log("�½���Box����������Ϊ��" + Vector3Int.RoundToInt(postransform));

                    }
                    else if (Precube == Player) {
                        Player newitem = new() {
                            position = Vector3Int.RoundToInt(postransform)
                        };

                        items.Add(newitem.position, newitem);

                        Debug.Log("�½���Player����������Ϊ��" + Vector3Int.RoundToInt(postransform));
                    } else {

                    }
                    
                }
                createcube = false;
            }

            if (destroycube || rightclick) {
                if (isHit) {
                    Debug.Log("˫������Ϊ��" + hit.point);
                    target = hit.point; //��⵽��ײ���ͰѼ�⵽�ĵ��¼����
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