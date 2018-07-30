using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftPartMove1 : MonoBehaviour {
    public float speed;
    public Vector3 direction;//传送带的运动方向
    private LiftPattern liftPattern;//抬升模式
    private LiftPartState lps;
    private GameObject LiftPart;
    private Vector3 TargetPosition1;//未抬升的位置
    private Vector3 TargetPosition2;//抬升后的高度
    // Use this for initialization
    void Start () {
        lps = this.gameObject.GetComponent<ShowEquipState>().equipmentState as LiftPartState;
        speed = lps.deliverSpeed;
        direction = lps.deliverDirection;

        LiftPart = this.gameObject;
        //temphigh是LiftPart需要抬升的高度
        float temphigh = GlobalVariable.KPD.HighValues[0] - GlobalVariable.KPD.HighValues[1];

        TargetPosition1 = LiftPart.transform.localPosition;
        TargetPosition2 = TargetPosition1;
        //抬升后的高度
        TargetPosition2.y = TargetPosition1.y + temphigh;
        speed = 0.8f;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        lps = this.gameObject.GetComponent<ShowEquipState>().equipmentState as LiftPartState;
        speed = lps.deliverSpeed;
        direction = lps.deliverDirection;
        liftPattern = lps.liftPattern;
        //让该设备上所有的货物都运动
        List<GameObject> cargoList = new List<GameObject>();
        FindExtension.FindGameObjectsWithTagRecursive(this.gameObject, "Cargo", ref cargoList);
        if (lps.workState == State.On)
        {
            //顶升已经抬升,移动货物到传送带
            if (LiftPart.transform.localPosition == TargetPosition2)
            {
                if (cargoList.Count > 0)
                {
                    foreach (GameObject cargo in cargoList)
                    {
                        cargo.transform.localPosition += direction * speed * Time.deltaTime;
                    }
                } else {
                    liftPattern = LiftPattern.down;//没有货物，顶升开始下降
                }
            }
            //抬升（入库）
            if (liftPattern == LiftPattern.up)
            {
                //抬升到1
                LiftPart.transform.localPosition = Vector3.MoveTowards(LiftPart.transform.localPosition, TargetPosition2, speed * Time.deltaTime);
                if (LiftPart.transform.localPosition == TargetPosition2)
                {
                    liftPattern = LiftPattern.off;
                }
            }
            //下降（出库）
            else if (liftPattern == LiftPattern.down)
            {
                LiftPart.transform.localPosition = Vector3.MoveTowards(LiftPart.transform.localPosition, TargetPosition1, speed * Time.deltaTime);
                if (LiftPart.transform.localPosition == TargetPosition1)
                {
                    liftPattern = LiftPattern.off;
                }
            }
        }
    }
}
