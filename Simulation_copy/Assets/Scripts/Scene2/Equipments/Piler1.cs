using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct PilerParameter {
    public float PilerHigh;
    public float PilerLength;
}

public class Piler1 : MonoBehaviour
{
    private void Start()
    {
        float length = 10.1f;
        float high = 10f;
        PilerParameter PP; PP.PilerHigh = high; PP.PilerLength = length;
        GameObject OBJ = new GameObject();
        OBJ.transform.name = "Piler";
        Create(PP, OBJ);
        
    }
    

    

    #region Create
    public static void Create(PilerParameter PP, GameObject OBJ)
    {
        //float High = high-0.1f;//堆垛机的高
        //float Length = length;//堆垛机轨道长度

        //GameObject OBJ1 = new GameObject();
        //OBJ1.name = OBJ.name;
        //OBJ1.transform.parent = OBJ.transform;
        CreatePiler(PP,OBJ);
        //OBJ1.transform.localPosition = new Vector3(0, 0, Length / 2);
    }
    #endregion

    #region CreatePathway
    public static void CreatePathway(PilerParameter PP,GameObject Pathway)
    {
        //high表示堆垛机总高度
        float High = PP.PilerHigh;
        float Length = PP.PilerLength;
        //定制上轨道
        GameObject obj1 = (GameObject)Resources.Load("Scene/Piler/上轨道");
        GameObject top_pathway = Instantiate(obj1); top_pathway.name = "top_pathway";
        float Scale1 = top_pathway.transform.localScale.x;
        Vector3 Size1 = top_pathway.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        top_pathway.transform.localScale = new Vector3(Scale1, Scale1, Length / Size1.z);
        //定制下轨道
        GameObject obj2 = (GameObject)Resources.Load("Scene/Piler/下轨道");
        GameObject buttom_pathway = Instantiate(obj2); buttom_pathway.name = "buttom_pathway";
        float Scale2 = buttom_pathway.transform.localScale.x;
        Vector3 Size2 = buttom_pathway.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        buttom_pathway.transform.localScale = new Vector3(Scale2, Scale2, Length / Size2.z);
        
        top_pathway.transform.parent = Pathway.transform;
        top_pathway.transform.localPosition = new Vector3(0, High / 2 - Scale1 * Size1.y / 2, 0);
        buttom_pathway.transform.parent = Pathway.transform;
        buttom_pathway.transform.localPosition = new Vector3(0, -High / 2 + Scale2 * Size2.y / 2, 0);
    }

    #endregion

    #region CreateBodyPart
    public static void CreateBodyPart(PilerParameter PP, GameObject BodyPart)
    {
        float temp_high = 0.15f;//上轨道与body主体之间的宽度
        // 上轨道信息
        GameObject obj0 = (GameObject)Resources.Load("Scene/Piler/上轨道");
        float Scale0 = obj0.transform.localScale.x;
        Vector3 Size0 = obj0.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        // 加载关键构件
        GameObject obj3 = (GameObject)Resources.Load("Scene/Piler/body1");//上滚轴
        GameObject obj4 = (GameObject)Resources.Load("Scene/Piler/body2");//下滚轴
        GameObject obj5 = (GameObject)Resources.Load("Scene/Piler/body0");//竖直机身
        GameObject obj6 = (GameObject)Resources.Load("Scene/Piler/ElectricBox");//电箱部分
        //上滚轴、下滚轴信息
        GameObject top_body = Instantiate(obj3); float Scale1 = top_body.transform.localScale.x;
        Vector3 topbody_size = top_body.GetComponent<MeshFilter>().sharedMesh.bounds.size;//上滚轴尺寸信息
        GameObject buttom_body = Instantiate(obj4); float Scale2 = buttom_body.transform.localScale.x;
        Vector3 buttombody_size = buttom_body.GetComponent<MeshFilter>().sharedMesh.bounds.size;//下滚轴尺寸信息
        //主体body尺寸
        float BodyHigh = PP.PilerHigh - Scale0 * Size0.y - Scale2 * buttombody_size.y - temp_high;//计算主体body高度值
        //Debug.Log(PP.PilerHigh); Debug.Log(Scale0 * Size0.y); Debug.Log(Scale2 * buttombody_size.y);
        GameObject middle_body = Instantiate(obj5); float Scale3 = middle_body.transform.localScale.x;
        Vector3 middlebody_size = middle_body.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        middle_body.transform.localScale = new Vector3(Scale3, BodyHigh / middlebody_size.y, Scale3);
        // 电箱部分
        GameObject ElectricBox = Instantiate(obj6);
        Vector3 ElectricBox_size = new Vector3(1.533f, 2.6f, 1.01f);//电箱尺寸
        //组合设计
        top_body.transform.parent = BodyPart.transform;
        middle_body.transform.parent = BodyPart.transform;
        buttom_body.transform.parent = BodyPart.transform;
        buttom_body.transform.localPosition = new Vector3(0, Scale2 * buttombody_size.y / 2, 0);
        middle_body.transform.localPosition = new Vector3(0, Scale2 * buttombody_size.y + BodyHigh / 2, 0);
        top_body.transform.localPosition = new Vector3(0, Scale2 * buttombody_size.y + BodyHigh + Scale1 * topbody_size.y / 2 - 0.062f, 0);
        Vector3 EB_position = new Vector3(0, 0, 0); EB_position.y = ElectricBox_size.y / 2 + Scale2 * buttombody_size.y + 0.09f;
        EB_position.z = middle_body.transform.localPosition.z + Scale3 * middlebody_size.z / 2 + ElectricBox_size.z / 2;
        ElectricBox.transform.parent = BodyPart.transform; ElectricBox.transform.localPosition = EB_position;
        //增加上下移动部分
        GameObject obj7 = (GameObject)Resources.Load("Scene/Piler/UpPart");//上下移动部分
        GameObject UpPart = Instantiate(obj7); UpPart.name = "UpPart"; float Scale7 = UpPart.transform.localScale.x;
        UpPart.transform.parent = BodyPart.transform;
        Vector3 UpPart_size = new Vector3(0, 0, 0); //Debug.Log(2);
        MyClass.MeshSize(UpPart, ref UpPart_size);
        //Debug.Log(UpPart_size);
        Vector3 Up_position = new Vector3(0, 0, 0); Up_position.y = Scale2 * buttombody_size.y + Scale7 * UpPart_size.y / 2;
        Up_position.z = middle_body.transform.localPosition.z - Scale3 * middlebody_size.z/2 - Scale7 * UpPart_size.z / 2 + 0.12f;
        //Debug.Log(Up_position);
        UpPart.transform.localPosition = Up_position; //Debug.Log(1);
    }
    #endregion

    #region CreatePiler
    public static void CreatePiler(PilerParameter PP, GameObject OBJ) {
        float High = PP.PilerHigh;
        float Length = PP.PilerLength;
        //float temp_high = High - 0.45f - 0.49f;//根据给定高度计算机身竖直部分长度
        // 轨道部分
        GameObject OBJ1 = new GameObject();
        OBJ1.name = "Pathway";
        CreatePathway(PP, OBJ1);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(OBJ1);
        // 主体机身部分
        GameObject obj = (GameObject)Resources.Load("Scene/Piler/body2");
        //GameObject obj_1 = Instantiate(obj);
        float Scale = obj.transform.localScale.z;
        Vector3 obj_size = obj.GetComponent<MeshFilter>().sharedMesh.bounds.size; //Debug.Log(obj_size);
        //DestroyImmediate(obj)

        GameObject OBJ2 = new GameObject();
        OBJ2.name = "BodyPart";
        CreateBodyPart(PP, OBJ2);

        OBJ1.transform.parent = OBJ.transform;
        OBJ2.transform.parent = OBJ.transform;
        OBJ1.transform.localPosition = new Vector3(0, High / 2, -Length / 2);
        OBJ2.transform.localPosition = new Vector3(0, 0, -Scale * obj_size.z / 2);

        //OBJ.AddComponent<OperatingState>().state = State.Off;
    }

    #endregion
    
    #region CreateUpPart
    public static void CreateUpPart(GameObject OBJ) {
        // 上下移动部分
        GameObject obj1 = (GameObject)Resources.Load("Scene/Piler/UpPart");
        GameObject OBJ1 = Instantiate(obj1);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(OBJ1);
        Vector3 OBJ1_size = OBJ1.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        float Scale = OBJ1.transform.localScale.x;
        OBJ1.name = "UpPart";
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(OBJ3);
        // 叉手部分
        GameObject obj2 = (GameObject)Resources.Load("Scene/Piler/叉手");
        GameObject OBJ2 = Instantiate(obj2);
        MyClass.Create2(OBJ2);
        Vector3 OBJ2_size = OBJ2.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        OBJ2.name = "Fork";
        //构造竖直移动部件
        OBJ1.transform.parent = OBJ.transform;
        OBJ2.transform.parent = OBJ.transform;
        OBJ1.transform.localPosition = new Vector3(0, Scale*OBJ1_size.y/2, 0-Scale*OBJ1_size.z/2);
        float s1 = 0.270f;float s2 = 0.48f;
        OBJ2.transform.localPosition = new Vector3(0, s1-Scale*OBJ2_size.y/2, 0-OBJ1_size.z+s2+OBJ2_size.z/2);
    }
    #endregion

    #region CreateMovePart
    public static void CreateMovePart(GameObject OBJ, float high) {

        GameObject OBJ1 = new GameObject();
        OBJ1.name = "UpPart";
        CreateUpPart(OBJ1);//竖直升降部分
        GameObject OBJ2 = new GameObject();
        OBJ2.name = "BodyPart";
        //CreateBodyPart(OBJ2, high);//机身主体部分
        //Debug.Log(0);
        float distance1 = 2.13f;float distance2 = 0.1f;
        OBJ1.transform.parent = OBJ.transform;
        OBJ2.transform.parent = OBJ.transform;
        //
        GameObject obj = (GameObject)Resources.Load("Scene/Piler/body2");
        GameObject obj_1 = Instantiate(obj);
        float Scale = obj_1.transform.localScale.z;
        Vector3 obj_size = obj_1.GetComponent<MeshFilter>().sharedMesh.bounds.size; //Debug.Log(obj_size);
        //Vector3 obj_size = obj_1.GetComponent<MeshFilter>().mesh.bounds.size;
        DestroyImmediate(obj_1);
        float length = Scale * obj_size.z;
        //GameObject obj2 = (GameObject)Resources.Load("Scene/Piler/UpPart");
        GameObject obj2_1 = Instantiate(OBJ1);
        MyClass.Create2(obj2_1);
        Vector3 obj2_size = obj2_1.GetComponent<MeshFilter>().sharedMesh.bounds.size; //Debug.Log(obj2_size);
        DestroyImmediate(obj2_1);
        OBJ1.transform.localPosition = new Vector3(0, distance2 + Scale * obj_size.y, obj2_size.z);
        OBJ2.transform.localPosition = new Vector3(0, 0, Scale * obj_size.z);
        //Debug.Log(length-distance1);
    }
    #endregion

   

   
}