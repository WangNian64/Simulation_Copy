using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTransferMove1 : MonoBehaviour {
    public float speed;
    public Vector3 direction;
    public LiftTransferState lts;
    // Use this for initialization
    void Start () {
        lts = this.gameObject.GetComponent<ShowEquipState>().equipmentState as LiftTransferState;
        speed = lts.deliverSpeed;
        direction = lts.deliverDirection;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        lts = this.gameObject.GetComponent<ShowEquipState>().equipmentState as LiftTransferState;
        speed = lts.deliverSpeed;
        direction = lts.deliverDirection;
        //让该设备上所有的货物都运动
        List<GameObject> cargoList = new List<GameObject>();
        FindExtension.FindGameObjectsWithTagRecursive(this.gameObject, "Cargo", ref cargoList);
        if (lts.workState == State.On)
        {
            foreach (GameObject cargo in cargoList)
            {
                cargo.transform.localPosition += direction * speed * Time.deltaTime;
            }
        }
    }
}
