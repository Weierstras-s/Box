using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MoveCamera : MonoBehaviour {
    //观察目标
    public Transform Target;
    //观察距离
    public float Distance = 10F;
    public float Distance0 = 10F;   //用于判断是否有滚轮事件产生
    //观察初始位置
    public Vector3 InitPosition = new Vector3(30f, 45f, 0f);
    //分层射线信息
    
    //旋转速度
    private float SpeedX = 240;
    private float SpeedY = 120;

    //角度限制
    private float MinLimitY = -45;
    private float MaxLimitY = 180;

    //旋转角度
    private float mX = 0.0F;
    private float mY = 0.0F;

    //鼠标缩放距离最值
    private float MaxDistance = 20F;
    private float MinDistance = 1.5F;
    //鼠标缩放速率
    private float ZoomSpeed = 2F;

    //是否启用差值
    public bool isNeedDamping = true;
    //速度
    public float Damping = 10F;

    private Quaternion mRotation = Quaternion.identity;

    void Start() {
        //初始化旋转角度
        mX = transform.eulerAngles.x;
        mY = transform.eulerAngles.y;

        transform.position = new Vector3(0f, 10f, 0f);
        mRotation.eulerAngles = InitPosition;
        transform.rotation = mRotation;
    }

    void LateUpdate() {
        if (!EventSystem.current.IsPointerOverGameObject()) {   //射线不在UI上
            // 获取射线位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //屏幕坐标转射线
            RaycastHit hit;                                                     //射线对象是：结构体类型（存储了相关信息）
            bool isHit = Physics.Raycast((Ray)ray, out hit, 1000, 1 << LayerMask.NameToLayer("Ground"));             //发出射线检测到了碰撞   isHit返回的是 一个bool值
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            /*Debug.Log("地面交点：" + hit.point);*/

            //鼠标左键旋转
            if (Target != null && Input.GetMouseButton(0)) {

                //获取鼠标输入
                mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
                mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
                //范围限制
                mY = ClampAngle(mY, MinLimitY, MaxLimitY);
                //计算旋转
                //我们可以通过Quaternion.Euler()方法将一个Vector3类型的值转化为一个四元数
                //，进而通过修改Transform.Rotation来实现相同的目的
                mRotation = Quaternion.Euler(mY, mX, 0);
                //根据是否插值采取不同的角度计算方式
                if (isNeedDamping) {
                    transform.rotation = Quaternion.Lerp(transform.rotation, mRotation, Time.deltaTime * Damping);
                } else {
                    transform.rotation = mRotation;
                }
                //处理同时按下鼠标右键和方向控制键
                /* if (Target.GetComponent<NoLockiVew_Player>().State == NoLockiVew_Player.PlayerState.Walk)
                 {
                     Target.rotation = Quaternion.Euler(new Vector3(0, mX, 0));
                 }*/
            }

            //鼠标滚轮缩放
            float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
            Vector3 mPosition;
            Distance -= scrollwheel * ZoomSpeed;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);

            //重新计算位置
            mPosition = mRotation * new Vector3(0.0F, 0.0F, -Distance) + Target.position;
            //设置相机的角度和位置
            if (isNeedDamping) {
                transform.position = Vector3.Lerp(transform.position, mPosition, Time.deltaTime * Damping);
            } else {
                transform.position = mPosition;
            }

        }
    }


    //角度限制
    private float ClampAngle(float angle, float min, float max) {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}


