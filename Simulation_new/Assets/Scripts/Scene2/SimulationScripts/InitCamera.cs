using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCamera : MonoBehaviour {

    //public GameObject Player;
    private Vector3 offset = new Vector3(2, 1, 2);//相机相对于玩家的位置
    private Transform target;
    private Vector3 pos;
    public float speed = 5;
    public bool Once;
    public Vector3 CameraPosition;//相机位置
    public Vector3 Rotation;

    void Start () {
        //主摄像机设置
        GameObject Camera = GameObject.Find("Main Camera");
        Camera.AddComponent<FiCameraControl>();
        //调整相机位置
        CameraPosition = GameObject.Find("WarehouseScene").transform.localPosition;
        CameraPosition.x = CameraPosition.x + 15;
        CameraPosition.y = CameraPosition.y + 5;
        CameraPosition.z = CameraPosition.z - 5;
        Camera.transform.localPosition = CameraPosition;
        //调整旋转视角
        Rotation = GameObject.Find("WarehouseScene").transform.localEulerAngles;
        Rotation.y = Rotation.y - 90f;
        Camera.transform.localEulerAngles = Rotation;

        GlobalVariable.FollowState = false;
        Once = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (GlobalVariable.FollowState == true)
        {
            Follow();
            //Debug.Log(0);
            Once = false;
        }
        if (GlobalVariable.FollowState == false && Once == false)
        {
            Init();
            Once = true;
        }
	}

    private void Init()
    {
        //主摄像机设置
        GameObject Camera = GameObject.Find("Main Camera");
        Camera.AddComponent<FiCameraControl>();
        Camera.transform.position = new Vector3(0, 5, 10);
        Vector3 Rotation = Camera.transform.localEulerAngles;
        Rotation.y = -180;
        Camera.transform.localEulerAngles = Rotation;
        //this.transform.position = Vector3.Lerp(this.transform.position, CameraPosition, speed * Time.deltaTime);
    }

    private void Follow()
    {
        target = GlobalVariable.FollowPlayer.transform;
        pos = target.position + offset;
        this.transform.position = Vector3.Lerp(this.transform.position, pos, speed * Time.deltaTime);//调整相机与玩家之间的距离
        Quaternion angel = Quaternion.LookRotation(target.position - this.transform.position);//获取旋转角度
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angel, speed * Time.deltaTime);
    }
}
