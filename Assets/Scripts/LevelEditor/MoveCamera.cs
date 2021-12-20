using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace LevelEditor {
    public class MoveCamera : MonoBehaviour {
        //�۲�Ŀ��
        public Transform Target;
        //�۲����
        public float Distance = 10F;
        public float Distance0 = 10F;   //�����ж��Ƿ��й����¼�����
                                        //�۲��ʼλ��
        public Vector3 InitPosition = new Vector3(30f, 45f, 0f);
        //�ֲ�������Ϣ

        //��ת�ٶ�
        private float SpeedX = 240;
        private float SpeedY = 120;

        //�Ƕ�����
        private float MinLimitY = -45;
        private float MaxLimitY = 180;

        //��ת�Ƕ�
        private float mX = 0.0F;
        private float mY = 0.0F;

        //������ž�����ֵ
        private float MaxDistance = 20F;
        private float MinDistance = 1.5F;
        //�����������
        private float ZoomSpeed = 2F;

        //�Ƿ����ò�ֵ
        public bool isNeedDamping = true;
        //�ٶ�
        public float Damping = 10F;

        private Quaternion mRotation = Quaternion.identity;

        void Start() {
            //��ʼ����ת�Ƕ�
            mX = transform.eulerAngles.x;
            mY = transform.eulerAngles.y;

            transform.position = new Vector3(0f, 10f, 0f);
            mRotation.eulerAngles = InitPosition;
            transform.rotation = mRotation;
        }

        void LateUpdate() {
            if (!EventSystem.current.IsPointerOverGameObject()) {   //���߲���UI��
                                                                    // ��ȡ����λ��
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //��Ļ����ת����
                RaycastHit hit;                                                     //���߶����ǣ��ṹ�����ͣ��洢�������Ϣ��
                bool isHit = Physics.Raycast((Ray)ray, out hit, 1000, 1 << LayerMask.NameToLayer("Ground"));             //�������߼�⵽����ײ   isHit���ص��� һ��boolֵ
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                /*Debug.Log("���潻�㣺" + hit.point);*/

                //��������ת
                if (Target != null && Input.GetMouseButton(0)) {

                    //��ȡ�������
                    mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
                    mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
                    //��Χ����
                    mY = ClampAngle(mY, MinLimitY, MaxLimitY);
                    //������ת
                    //���ǿ���ͨ��Quaternion.Euler()������һ��Vector3���͵�ֵת��Ϊһ����Ԫ��
                    //������ͨ���޸�Transform.Rotation��ʵ����ͬ��Ŀ��
                    mRotation = Quaternion.Euler(mY, mX, 0);
                    //�����Ƿ��ֵ��ȡ��ͬ�ĽǶȼ��㷽ʽ
                    if (isNeedDamping) {
                        transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);
                    } else {
                        transform.rotation = mRotation;
                    }
                    //����ͬʱ��������Ҽ��ͷ�����Ƽ�
                    /* if (Target.GetComponent<NoLockiVew_Player>().State == NoLockiVew_Player.PlayerState.Walk)
                     {
                         Target.rotation = Quaternion.Euler(new Vector3(0, mX, 0));
                     }*/
                }

                //����������
                float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
                Vector3 mPosition;
                Distance -= scrollwheel * ZoomSpeed;
                Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

                //���¼���λ��
                mPosition = mRotation * new Vector3(0.0F, 0.0F, -Distance) + Target.position;
                //��������ĽǶȺ�λ��
                if (isNeedDamping) {
                    transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
                } else {
                    transform.position = mPosition;
                }

            }
        }


        //�Ƕ�����
        private float ClampAngle(float angle, float min, float max) {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }

}
