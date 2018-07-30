using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighBay2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    #region ConnectComponent
    public static void ConnectComponent(HighStoreShelf_Parameter HP, MultiHighBay_Parameter MHP, GameObject OBJ)
    {
        //增加高架库之间的连)接
        int Num1 = (MHP.Num - 1) / 2;//偶数-奇数连接
        int Num2 = (MHP.Num + 1) / 2;//奇数-偶数连接
        float Depth = HP.Size.x; float TunnelWidth = MHP.TunnelWidth; float HookupDistance = MHP.HookupDistance;
        //进行奇偶高架库之间的连接
        GameObject obj1 = new GameObject();//奇偶高架库之间的连接部分
        GameObject OBJ1 = new GameObject();//所有奇偶高架库之间的连接部分
        Odd2Even_Connect(HP, MHP, obj1);
        for (int i = 1; i <= Num2; i++)
        {
            GameObject clone = Instantiate(obj1); clone.transform.parent = OBJ1.transform;
            clone.transform.localPosition = new Vector3(-((2 * i - 1) * Depth + TunnelWidth / 2 + (i - 1) * (TunnelWidth + HookupDistance)), 0, 0);
        }
        //进行偶奇高架库之间的连接
        GameObject obj2 = new GameObject();//偶奇高架库之间的连接部分
        GameObject OBJ2 = new GameObject();//所有偶奇高架库之间的连接部分
        Even2Odd_Connect(HP, MHP, obj2);
        for (int i = 1; i <= Num1; i++)
        {
            GameObject clone = Instantiate(obj2); clone.transform.parent = OBJ2.transform;
            clone.transform.localPosition = new Vector3(-(HookupDistance / 2 + 2 * i * Depth + i * TunnelWidth + (i - 1) * HookupDistance), 0, 0);
        }
        OBJ1.transform.parent = OBJ.transform; OBJ2.transform.parent = OBJ.transform;
        OBJ1.transform.localPosition = new Vector3(0, HP.Size.y, 0);
        OBJ2.transform.localPosition = new Vector3(0, 0, 0);

        DestroyImmediate(obj1); DestroyImmediate(obj2);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(OBJ);
    }
    #endregion

    #region Odd2Even_Connect
    public static void Odd2Even_Connect(HighStoreShelf_Parameter HP, MultiHighBay_Parameter MHP, GameObject OBJ)
    {
        //奇数高架库与偶数高架库之间的连接
        float TunnelWidth = MHP.TunnelWidth;
        GameObject obj1 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube2");//加载侧面固定杆
        Vector3 size = new Vector3();
        size.x = TunnelWidth;
        size.y = HP.HorizontalStanchionThick;
        size.z = HP.VerticalStanchionWidth;
        GameObject obj1_1 = Instantiate(obj1); obj1_1.transform.localScale = size;
        //GameObject OBJ1 = new GameObject();
        for (int i = 0; i <= HP.ColumnsNum; i++)
        {
            GameObject clone = Instantiate(obj1_1);
            clone.transform.parent = OBJ.transform;
            clone.transform.localPosition = new Vector3(0, -size.y / 2, -size.z / 2 - i * (HP.ColumnWidth + HP.VerticalStanchionWidth));
        }
        //GameObject OBJ2 = new GameObject();
        GameObject obj1_2 = Instantiate(obj1); obj1_2.transform.localScale = new Vector3(size.x, size.y, size.z / 2);
        //obj1_2.transform.parent = OBJ.transform; obj1_2.transform.localPosition = new Vector3(size.x, size.y, size.z / 2);
        GameObject obj1_3 = Instantiate(obj1); obj1_3.transform.localScale = new Vector3(size.x, size.y, size.z / 2);
        //obj1_3.transform.parent = OBJ.transform; obj1_3.transform.localPosition = new Vector3(size.x, size.y, size.z / 2);

        //OBJ1.transform.parent = OBJ.transform; 
        obj1_2.transform.parent = OBJ.transform; obj1_3.transform.parent = OBJ.transform;
        obj1_2.transform.localPosition = new Vector3(0, -size.y / 2, HP.ColumnWidth + size.z / 4);
        //OBJ1.transform.localPosition = new Vector3(0, 0, -HP.ColumnWidth);
        obj1_3.transform.localPosition = new Vector3(0, -size.y / 2, -(HP.ColumnWidth + HP.VerticalStanchionWidth) * (HP.ColumnsNum + 1) - size.z / 4);
        //OBJ2.transform.parent = OBJ.transform;//重新调整坐标
        //OBJ2.transform.localPosition = new Vector3(0, 0, (HP.ColumnWidth * (HP.ColumnsNum + 2)) / 2);
        DestroyImmediate(obj1_1);
    }
    #endregion

    #region Even2Odd_Connect
    public static void Even2Odd_Connect(HighStoreShelf_Parameter HP, MultiHighBay_Parameter MHP, GameObject OBJ)
    {
        //构件偶数高架库与奇数高架库之间的连接
        float Width = HP.ColumnWidth+HP.VerticalStanchionWidth;
        GameObject obj = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube2");//加载侧面固定杆
        Vector3 size = new Vector3();
        size.x = MHP.HookupDistance; size.y = HP.HorizontalStanchionThick; size.z = HP.VerticalStanchionWidth;
        int Num1 = HP.FloorsNum / 2 + 1;//纵向个数
        int Num2 = HP.ColumnsNum + 1;//横向个数
        float High = HP.Size.y - HP.Height2Ground / 2;//首层距地高度的一半出开始纵向固定
        float Temp_High = High / Num1;//纵向两根之间的高度
        GameObject OBJ1 = new GameObject();
        for (int i = 0; i < Num2; i++)
        {
            for (int j = 0; j <= Num1; j++)
            {
                GameObject clone = Instantiate(obj); clone.transform.localScale = size; clone.transform.parent = OBJ1.transform;
                clone.transform.localPosition = new Vector3(0, HP.Height2Ground / 2 + Temp_High * j - size.y / 2, -i * Width - HP.VerticalStanchionWidth / 2);
            }
        }
        OBJ1.transform.parent = OBJ.transform; OBJ1.transform.localPosition = new Vector3(0, 0, 0);
        GameObject obj1 = Instantiate(obj); obj1.transform.localScale = new Vector3(size.x, size.y, size.z / 2);
        obj1.transform.parent = OBJ.transform;
        obj1.transform.localPosition = new Vector3(0, HP.Size.y - size.y / 2, HP.ColumnWidth + size.z / 4);
        GameObject obj2 = Instantiate(obj); obj2.transform.localScale = new Vector3(size.x, size.y, size.z / 2);
        obj2.transform.parent = OBJ.transform;
        obj2.transform.localPosition = new Vector3(0, HP.Size.y - size.y / 2, -(HP.ColumnWidth+HP.VerticalStanchionWidth)*(HP.ColumnsNum+1) - size.z / 4);

    }
    #endregion
    //创建高架库
    #region Create_HighBays
    public static void Create_HighBays(HighStoreShelf_Parameter HP, MultiHighBay_Parameter MHP, GameObject OBJ)
    {
        //HP单个高架库设计尺寸参数
        //HSP高架库组合设计的参数
        //根据单个高架库设计参数生成高架库
        GameObject HighBay = new GameObject();
        string name = "HighBay";
        HighBay.name = name;
        //
        Subassembly.Create_HighStoreShelf(HP,HighBay);
        HighBay.transform.Rotate(0, 180, 0);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(HighBay); //Debug.Log(00);
                                                                                      //根据高架库组合设计的参数生成高架库组合

        GameObject HighBayGroup = new GameObject(); HighBayGroup.name = "HighBayGroup";
        for (int i = 0; i < MHP.Num; i++)
        {
            GameObject clone = Instantiate(HighBay); clone.transform.parent = HighBayGroup.transform; clone.name = name + (i + 1).ToString();
            float TempValue = -(HP.Size.x / 2 + i * HP.Size.x + ((i + 1) / 2) * MHP.TunnelWidth + (i / 2) * MHP.HookupDistance);
            clone.transform.localPosition = new Vector3(TempValue, 0, 0);
        }
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(HighBayGroup);
        GameObject OBJ1 = new GameObject(); OBJ1.name = "ConnectPart";
        ConnectComponent(HP, MHP, OBJ1);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(OBJ1);
        HighBayGroup.transform.parent = OBJ.transform; HighBayGroup.transform.localPosition = new Vector3(0, 0, 0);
        OBJ1.transform.parent = OBJ.transform; OBJ1.transform.localPosition = new Vector3(0, 0, 0);

        DestroyImmediate(HighBay);
    }
    #endregion

}
