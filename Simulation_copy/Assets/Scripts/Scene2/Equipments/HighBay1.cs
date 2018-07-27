using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public struct HighBay_Parameter
{
    public int ColumnsNum;
    public float ColumnWidth;
    public int FloorsNum;
    public float[] FloorsHigh;
    public float[] HorizontalStanchionSize;
    public float SlopeAngle;
    public float[] HighBaySize;
}


public class HighBay1 : MonoBehaviour
{
    
    void Start()
    {
        string path = @"D:\桌面\高架库参数表.txt";
        HighBay_Parameter HP;
        HighBay_ReadTxt(ref path, out HP);
        GameObject HighBay = new GameObject();
        HighBay.name = "HighBay";
        Create_HighBay_Update(HP,HighBay);

    }
    

    #region HighBay_ReadTxt
    public void HighBay_ReadTxt(ref string path, out HighBay_Parameter HP)
    {
        //Debug.Log(path);
        string[] strs1 = System.IO.File.ReadAllLines(path);
        //读取高架库列数
        int i1 = strs1[1].IndexOf("= ") + 1; string str1 = strs1[1].Remove(0, i1);
        int columns_num = int.Parse(str1); //Debug.Log(columns_num);
        //读取高架库列宽
        int i2 = strs1[2].IndexOf("= ") + 1; string str2 = strs1[2].Remove(0, i2);
        float column_width = float.Parse(str2); //Debug.Log(column_width);
        //读取高架库层数
        int i3 = strs1[3].IndexOf("= ") + 1; string str3 = strs1[3].Remove(0, i3);
        int floors_num = int.Parse(str3); //Debug.Log(floors_num);
        //读取高架库首层距地高度
        int i4 = strs1[4].IndexOf("= ") + 1; string str4 = strs1[4].Remove(0, i4);
        float AltitudeAgl = float.Parse(str4); //Debug.Log(AltitudeAgl);
        //读取高架库每层高度
        int i5 = strs1[5].IndexOf("【") + 1; int j5 = strs1[5].IndexOf("】");
        string str5 = strs1[5].Substring(i5, j5 - i5); string[] str5_1 = str5.Split(' ');
        float[] FloorsHigh = new float[floors_num + 1];
        FloorsHigh[0] = AltitudeAgl;
        float High = FloorsHigh[0];
        for (int i = 1; i <= floors_num; i++)
        {
            FloorsHigh[i] = float.Parse(str5_1[i - 1]);
            High = High + FloorsHigh[i];
        } //Debug.Log(High);
        //读取高架库仓位深度
        int i6 = strs1[6].IndexOf("= ") + 1; string str6 = strs1[6].Remove(0, i6);
        float HighBay_Depth = float.Parse(str6); //Debug.Log(HighBay_Depth);
        //读取正面水平支柱尺寸
        int i7 = strs1[7].IndexOf("【") + 1; int j7 = strs1[7].IndexOf("】");
        string str7 = strs1[7].Substring(i7, j7 - i7); string[] str7_1 = str7.Split(' ');
        float[] HorizontalStanchion_size = { float.Parse(str7_1[0]), float.Parse(str7_1[1]), float.Parse(str7_1[2]) };//高架库尺寸，【0】、【1】、【2】分别是x,z,y方向尺寸
        //读取侧面支柱倾斜角度
        int i8 = strs1[8].IndexOf("= ") + 1; string str8 = strs1[8].Remove(0, i8);
        float slope_angle = float.Parse(str8); //Debug.Log(slope_angle);

        //HighBay_Parameter HP;
        HP.ColumnsNum = columns_num;
        HP.ColumnWidth = column_width;
        HP.FloorsNum = floors_num;
        HP.FloorsHigh = FloorsHigh;
        //HP.HighBay_Depth = HighBay_Depth;
        HP.HorizontalStanchionSize = HorizontalStanchion_size;
        HP.SlopeAngle = slope_angle;
        float[] size = { (columns_num + 2) * column_width, HighBay_Depth, High };
        HP.HighBaySize = size;
        //return HP;
    }
    #endregion
    
    #region Create_HorizontalStanchion
    public void Create_HorizontalStanchion(HighBay_Parameter HP, GameObject OBJ) {

        float[] HS_Size = HP.HorizontalStanchionSize;//HS_size为水平支架的尺寸；
        //Debug.Log(HS_Size);
        float Depth = HP.HighBaySize[1];//depth为高架库宽度，用来确定前后水平支架的位置；
        float[] FloorsHigh = HP.FloorsHigh;//FloorsHigh为每层高度

        GameObject obj1 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube1");//正面水平支架
        obj1.transform.localScale = new Vector3(HS_Size[0], HS_Size[2], HS_Size[1]);
        // 构件水平支撑，horizontal_support,由一前一后2个水平的支撑组成
        GameObject horizontal_support = new GameObject();
        horizontal_support.name = "horizontal_support";
        Vector3 hs_position = horizontal_support.transform.position;
        GameObject obj1_1 = Instantiate(obj1);
        GameObject obj1_2 = Instantiate(obj1);
        obj1_1.transform.parent = horizontal_support.transform;
        obj1_2.transform.parent = horizontal_support.transform;
        // 确定2个水平支撑的位置
        obj1_1.transform.localPosition = new Vector3(-HS_Size[0] / 2, -HS_Size[2] / 2, (Depth - HS_Size[1]) / 2);
        obj1_2.transform.localPosition = new Vector3(-HS_Size[0] / 2, -HS_Size[2] / 2, -(Depth - HS_Size[1]) / 2);
        //确定每层水平支架位置
        //GameObject HorizontalStanchion = new GameObject();
        //HorizontalStanchion.name = "HorizontalStanchion";
        float high = 0;
        for (int i = 0; i < FloorsHigh.Length; i++) {
            high = high + FloorsHigh[i];
            GameObject obj = Instantiate(horizontal_support);
            obj.name = horizontal_support.name + (i + 1).ToString();
            obj.transform.parent = OBJ.transform;
            obj.transform.localPosition = new Vector3(0, high, 0);
        }
        DestroyImmediate(horizontal_support);
    }
    #endregion

    #region Create_Upright
    public void Create_Upright(HighBay_Parameter HP, GameObject OBJ) {
        Vector3 Upright_Size = new Vector3();//竖直立柱的尺寸
        Upright_Size.x = (HP.ColumnWidth - HP.HorizontalStanchionSize[0]) / 2;
        Upright_Size.y = HP.HighBaySize[2];
        Upright_Size.z = HP.HorizontalStanchionSize[1];
        float Depth = HP.HighBaySize[1];
        float Width = HP.ColumnWidth;

        GameObject obj1 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube3");//竖直方向支撑柱
        obj1.transform.localScale = Upright_Size;

        // 构件纵向支撑，vertical_support，由四根立柱组成
        //GameObject vertical_support = new GameObject();
        //vertical_support.name = "vertical_support";

        GameObject obj1_1 = Instantiate(obj1);
        GameObject obj1_2 = Instantiate(obj1);
        GameObject obj1_3 = Instantiate(obj1);
        GameObject obj1_4 = Instantiate(obj1);
        obj1_1.transform.parent = OBJ.transform; obj1_2.transform.parent = OBJ.transform;
        obj1_3.transform.parent = OBJ.transform; obj1_4.transform.parent = OBJ.transform;
        obj1_1.transform.localPosition = new Vector3(-Upright_Size.x / 2, Upright_Size.y / 2, (Depth - Upright_Size.z) / 2);
        obj1_2.transform.localPosition = new Vector3(Upright_Size.x / 2 - Width, Upright_Size.y / 2, (Depth - Upright_Size.z) / 2);
        obj1_3.transform.localPosition = new Vector3(Upright_Size.x / 2 - Width, Upright_Size.y / 2, -(Depth - Upright_Size.z) / 2);
        obj1_4.transform.localPosition = new Vector3(-Upright_Size.x / 2, Upright_Size.y / 2, -(Depth - Upright_Size.z) / 2);
        //GameObject.Find("Main Camera").GetComponent<MyClass>().Create(vertical_support);
    }
    #endregion

    #region Create_SideSlantStanchion
    public void Create_SideSlantStanchion(HighBay_Parameter HP, GameObject OBJ) {

        GameObject obj1 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube2");//加载侧面固定杆
        float Width = HP.ColumnWidth;
        float Angle1 = HP.SlopeAngle;//侧面倾斜支柱的倾斜角
        Vector3 Size = new Vector3();//侧面固定杆尺寸(水平向)
        Size.x = (HP.ColumnWidth - HP.HorizontalStanchionSize[0]) / 2;
        Size.y = Size.x; Size.z = HP.HighBaySize[1] - 2 * HP.HorizontalStanchionSize[1];
        Vector3 Size1 = new Vector3(Size.x, Size.y, Size.z / Mathf.Cos(Mathf.PI * Angle1 / 180));//侧面固定杆尺寸(倾斜向)

        float High_Temp = Size.z * Mathf.Tan(Mathf.PI * Angle1 / 180);//单个倾斜固定杆所需Y方向高度
        float High0 = HP.FloorsHigh[0] / 3;//侧面固定杆的起始高度(包括固定杆厚度)定义为首层距地高度的1/3
        float HighAll = HP.HighBaySize[2] - High0;//侧面倾斜固定杆需要固定的范围长度
        int sd_num = (int)Mathf.Floor(HighAll / High_Temp);// 计算出所需要的固定杆的个数


        GameObject SidePin = new GameObject();//侧面所有固定杆的父物体
        Vector3 sp_position = SidePin.transform.position;
        SidePin.name = "SidePin";
        GameObject obj1_0 = Instantiate(obj1);//侧面起始固定杆（水平杆）
        obj1_0.name = SidePin.name + 0.ToString(); obj1_0.transform.localScale = Size; obj1_0.transform.parent = SidePin.transform;
        obj1_0.transform.localPosition = new Vector3(-Size.x / 2, Size.y / 2 + High0, 0);

        for (int i = 1; i <= sd_num; i++)//侧面倾斜固定杆
        {
            if (i % 2 == 1)
            {
                GameObject clone = Instantiate(obj1);
                clone.transform.parent = SidePin.transform; clone.transform.localScale = Size1;
                clone.transform.Rotate(Angle1, 0, 0);//固定杆1（角度angle）
                clone.transform.localPosition = new Vector3(-Size.x / 2, High0 + Size.y / 2 + High_Temp / 2 + (i - 1) * High_Temp, 0);
                clone.transform.name = SidePin.name + i.ToString();
            }
            else
            {
                GameObject clone = Instantiate(obj1);
                clone.transform.parent = SidePin.transform;
                clone.transform.localScale = Size1;
                clone.transform.Rotate(0 - Angle1, 0, 0);//固定杆2（角度-angle）
                clone.transform.localPosition = new Vector3(-Size.x / 2, High0 + Size.y / 2 + High_Temp / 2 + (i - 1) * High_Temp, 0);
                clone.transform.name = SidePin.name + i.ToString();
            }
        }

        float High_Temp2 = HighAll % High_Temp;//计算出剩余高度
        GameObject obj1_1 = Instantiate(obj1);
        obj1_1.transform.parent = SidePin.transform;
        float Angle2_1 = Mathf.Atan(High_Temp2 / Size.z);//剩余高度采用的倾斜杆的角度(弧度)
        float Angle2 = 180 * Angle2_1 / Mathf.PI;//剩余高度采用的倾斜杆的角度（角度）
        obj1_1.transform.localScale = new Vector3(Size.x, Size.y, High_Temp2 / Mathf.Sin(Angle2_1));//剩余高度采用的倾斜杆的长度
        if (sd_num % 2 == 1)//通过已知数目判断剩余区域所采用的的倾斜杆的倾斜状态
        {
            obj1_1.transform.Rotate(0 - Angle2, 0, 0); //Debug.Log(1);
        }
        else
        {
            obj1_1.transform.Rotate(Angle2, 0, 0); //Debug.Log(0);
        }
        obj1_1.transform.localPosition = new Vector3(-Size.x / 2, HighAll - Size.y / 2 - High_Temp2 / 2 + High0, 0);
        obj1_1.name = SidePin.name + (sd_num + 1).ToString();
        GameObject obj1_2 = Instantiate(obj1);
        obj1_2.name = SidePin.name + (sd_num + 2).ToString();
        obj1_2.transform.localScale = Size;
        obj1_2.transform.parent = SidePin.transform;
        obj1_2.transform.localPosition = new Vector3(-Size.x / 2, HighAll - Size.y / 2 + High0, 0);
        //GameObject.Find("Main Camera").GetComponent<MyClass>().Create(SidePin);
        GameObject SidePin2 = Instantiate(SidePin);
        SidePin2.name = SidePin.name + 2.ToString();
        SidePin.transform.parent = OBJ.transform; SidePin2.transform.parent = OBJ.transform;
        SidePin.transform.localPosition = new Vector3(0, 0, 0);
        SidePin2.transform.localPosition = new Vector3(-Width + Size.x, 0, 0);

    }
    #endregion

    #region Create_SingleColumn
    public void Create_SingleColumn(HighBay_Parameter HP, GameObject OBJ) {
        float ColumnsNum = HP.ColumnsNum;
        GameObject HorizontalStanchion = new GameObject(); HorizontalStanchion.name = "HorizontalStanchion";
        Create_HorizontalStanchion(HP, HorizontalStanchion);
        GameObject Upright = new GameObject(); Upright.name = "Upright";
        Create_Upright(HP, Upright);
        GameObject SideSlantStanchion = new GameObject(); SideSlantStanchion.name = "SideSlantStanchion";
        Create_SideSlantStanchion(HP, SideSlantStanchion);
        HorizontalStanchion.transform.parent = OBJ.transform;
        HorizontalStanchion.transform.localPosition = new Vector3(-(HP.ColumnWidth - HP.HorizontalStanchionSize[0]) / 2, 0, 0);
        Upright.transform.parent = OBJ.transform; Upright.transform.localPosition = new Vector3(0, 0, 0);
        SideSlantStanchion.transform.parent = OBJ.transform; SideSlantStanchion.transform.localPosition = new Vector3(0, 0, 0);
    }
    #endregion

    #region Create_HighBay
    public void Create_HighBay(HighBay_Parameter HP, GameObject OBJ) {
        GameObject SingleColumn = new GameObject();
        SingleColumn.name = "SingleColumn";
        Create_SingleColumn(HP, SingleColumn);
        int ColumnsNum = HP.ColumnsNum;
        float ColumnWidth = HP.ColumnWidth;
        for (int i = 0; i < ColumnsNum; i++) {
            GameObject clone = Instantiate(SingleColumn);
            clone.transform.parent = OBJ.transform;
            clone.transform.localPosition = new Vector3(-ColumnWidth * i, 0, 0);
            clone.name = SingleColumn.name + (i + 1).ToString();
        }
        DestroyImmediate(SingleColumn);
    }
    #endregion

    #region Create_Ornament
    public void Create_Ornament(HighBay_Parameter HP, GameObject OBJ) {
        //竖直立柱
        Vector3 Upright_Size = new Vector3();//竖直立柱的尺寸
        Upright_Size.x = (HP.ColumnWidth - HP.HorizontalStanchionSize[0]) / 2;
        Upright_Size.y = HP.HighBaySize[2]; Upright_Size.z = HP.HorizontalStanchionSize[1];
        GameObject obj1 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube3");//竖直方向支撑柱
        GameObject obj1_1 = Instantiate(obj1); obj1.transform.localScale = Upright_Size; obj1_1.name = "Upright";
        //水平支架
        GameObject obj2 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube1");//正面水平支架
        GameObject obj2_1 = Instantiate(obj2);
        obj2_1.transform.localScale = new Vector3(HP.ColumnWidth-Upright_Size.x, HP.HorizontalStanchionSize[2], HP.HorizontalStanchionSize[1]);
        //倾斜支架
        float angle1 = Mathf.Atan(HP.ColumnWidth / HP.FloorsHigh[HP.FloorsNum]); //Debug.Log(angle1);
        GameObject obj1_2 = Instantiate(obj1); GameObject obj12 = new GameObject(); 
        obj1_2.transform.localScale = new Vector3(Upright_Size.x, HP.ColumnWidth / Mathf.Sin(angle1), Upright_Size.z);
        obj1_2.transform.parent = obj12.transform;
        obj1_2.transform.localPosition = new Vector3(0, (HP.ColumnWidth / Mathf.Sin(angle1)) / 2, 0);
        obj12.transform.Rotate(0, 0, 180 * angle1 / Mathf.PI); //Debug.Log(180 * angle1 / Mathf.PI);

        GameObject obj1_3 = Instantiate(obj1); GameObject obj13 = new GameObject(); obj1_3.transform.parent = obj13.transform;
        obj1_3.transform.localScale = new Vector3(Upright_Size.x, HP.ColumnWidth * Mathf.Cos(angle1), Upright_Size.z);
        obj1_3.transform.localPosition = new Vector3(0, HP.ColumnWidth * Mathf.Cos(angle1) / 2, 0);
        obj13.transform.Rotate(0, 0, 90+180 * angle1 / Mathf.PI); 
        float l2 = HP.ColumnWidth * Mathf.Cos(angle1);
        GameObject OBJ1 = new GameObject();
        obj2_1.transform.parent = OBJ1.transform;//水平的
        obj2_1.transform.localPosition = new Vector3(-(HP.ColumnWidth-Upright_Size.x) / 2, -HP.HorizontalStanchionSize[2] / 2, 0);
        obj12.transform.parent = OBJ1.transform;//倾斜（长）
        obj12.transform.localPosition = new Vector3(Upright_Size.x/4, -HP.FloorsHigh[HP.FloorsNum] - HP.HorizontalStanchionSize[2] / 2, 0);
        obj13.transform.parent = OBJ1.transform;//倾斜（短）
        obj13.transform.localPosition = new Vector3(0, -HP.HorizontalStanchionSize[2] / 2, 0);
        //
        GameObject OBJ11 = new GameObject(); obj1_1.transform.parent = OBJ11.transform; OBJ1.transform.parent = OBJ11.transform;
        obj1_1.transform.localPosition = new Vector3(-Upright_Size.x / 2, Upright_Size.y / 2, 0);
        OBJ1.transform.localPosition = new Vector3(-Upright_Size.x / 2, Upright_Size.y, 0);
        //
        GameObject obj3 = (GameObject)Resources.Load("Scene/Elevated_Warehouse/Cube2");//加载侧面固定杆
        GameObject obj3_1 = Instantiate(obj3);
        obj3_1.transform.localScale = new Vector3(Upright_Size.x, HP.HorizontalStanchionSize[2], HP.HighBaySize[1] - 2 * HP.HorizontalStanchionSize[1]);
        GameObject clone = Instantiate(OBJ11);
        OBJ11.transform.parent = OBJ.transform; clone.transform.parent = OBJ.transform; obj3_1.transform.parent = OBJ.transform;
        OBJ11.transform.localPosition = new Vector3(0, 0, HP.HighBaySize[1] / 2 - Upright_Size.z / 2);
        clone.transform.localPosition = new Vector3(0, 0, -HP.HighBaySize[1] / 2 + Upright_Size.z / 2);
        obj3_1.transform.localPosition = new Vector3(-HP.ColumnWidth + Upright_Size.x / 2, Upright_Size.y - HP.HorizontalStanchionSize[2] / 2, 0);
        MyClass.Create2(OBJ1);
    }
    #endregion
   
    #region Create_HighBay_Update
    public void Create_HighBay_Update(HighBay_Parameter HP, GameObject HighBay)
    {

        GameObject OBJ = new GameObject();
        Create_HighBay(HP, OBJ);
        GameObject OBJ1 = new GameObject(); OBJ1.name = "加固架1";
        Create_Ornament(HP, OBJ1);
        GameObject OBJ2 = Instantiate(OBJ1); OBJ2.transform.Rotate(0, 180, 0);
        GameObject HighBay1 = new GameObject();
        HighBay1.name = HighBay.name;
        OBJ.transform.parent = HighBay1.transform; OBJ1.transform.parent = HighBay1.transform; OBJ2.transform.parent = HighBay1.transform;
        OBJ.transform.localPosition = new Vector3(-HP.ColumnWidth, 0, 0);
        OBJ1.transform.localPosition = new Vector3(-HP.HighBaySize[0] + HP.ColumnWidth, 0, 0);
        OBJ2.transform.localPosition = new Vector3(-HP.ColumnWidth, 0, 0);
        HighBay1.transform.parent = HighBay.transform;
        HighBay1.transform.localPosition = new Vector3(0, 0, 0);
        HighBay1.transform.Rotate(0, -90, 0);
        //GameObject.Find("ScriptsContainer").GetComponent<MyClass>().Create2(HighBay);
    }
    #endregion
}
