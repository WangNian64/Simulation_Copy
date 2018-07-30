using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//顶升移栽机抬升或下降的脚本
public class LiftTransferMove : MonoBehaviour {
    public enum Pattern
    {
        up, down,off
    }

    private GameObject LiftPart;//抬升组件
    public GameObject Cargo;//抬升的货物
    public float Speed;//速度
    // private float High2;
    public Pattern pattern;//抬升模式
    private Vector3 TargetPosition1;//未抬升的高度
    private Vector3 TargetPosition2;//抬升后的高度
    public bool Finish1;//抬升结束标志
    public bool Finish2;//下落结束标志
	void Start () {
        string name = this.name + "/LiftPart";
        LiftPart = GameObject.Find(name);
        //temphigh是LiftPart抬升的高度
        float temphigh = GlobalVariable.KPD.HighValues[0] - GlobalVariable.KPD.HighValues[1];
        
        TargetPosition1 = LiftPart.transform.localPosition;
        TargetPosition2 = TargetPosition1;
        //抬升后的高度
        TargetPosition2.y = TargetPosition1.y + temphigh;
        Speed = 0.8f;
        Finish1 = false;
        Finish2 = false;
	}
	
	// Update is called once per frame
	void Update () {
        //抬升（入库）
        if (pattern == Pattern.up)
        {   
            //抬升到1
            LiftPart.transform.localPosition = Vector3.MoveTowards(LiftPart.transform.localPosition, TargetPosition2, Speed * Time.deltaTime);
            if (LiftPart.transform.localPosition == TargetPosition2)
            {
                Finish1 = true;//抬升停止
                pattern = Pattern.off;
            }
        }
        //下降（出库）
        else if (pattern == Pattern.down)
        {
            LiftPart.transform.localPosition = Vector3.MoveTowards(LiftPart.transform.localPosition, TargetPosition1, Speed * Time.deltaTime);
            if (LiftPart.transform.localPosition == TargetPosition1)
            {
                Finish2 = true;//下降停止
                pattern = Pattern.off;
            }
        }
	}
}
