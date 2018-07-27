using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BinsPanel : MonoBehaviour
{

    public int ShelfNum;//货架数目
    public int FloorNum;//层数
    public int ColumnNum;//列数
    // Use this for initialization
    void Start()
    {
        ShelfNum = 8;
        FloorNum = 9;
        ColumnNum = 9;
        GameObject StorageStateInterface = new GameObject();
        Ajustment(ShelfNum, FloorNum, ColumnNum);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Ajustment(int ShelfNum, int FloorNum, int ColumnNum)
    {
        GameObject StorageStateInterface0 = (GameObject)Resources.Load("Scene/Simulation/StorageStateInterface0");
        GameObject StorageStateInterface = Instantiate(StorageStateInterface0);
        StorageStateInterface.name = "StorageStateInterface";
        GameObject Content = GameObject.Find("StorageStateInterface/MainBody/Scroll View/Viewport/Content");
        GameObject ShelfPanel = Content.transform.Find("ShelfPanel").gameObject;
        GameObject Panel = ShelfPanel.transform.Find("Scroll View/Viewport/Content/Panel").gameObject;
        GameObject FloorNumPanel = Panel.transform.Find("FloorNumPanel").gameObject;
        GameObject BinsPanel = Panel.transform.Find("BinsPanel").gameObject;
        GameObject FloorItem = BinsPanel.transform.Find("FloorItem").gameObject;

        #region 设计FloorItem尺寸布局信息
        int FloorItemHigh = 20 * 2 + 2 * 2 + 1; int FloorItemWidth = 20 * ColumnNum + 2 * 2 + 1 * (ColumnNum - 1);
        FloorItem.GetComponent<GridLayoutGroup>().padding.left = 0;
        FloorItem.GetComponent<GridLayoutGroup>().padding.right = 0;
        FloorItem.GetComponent<GridLayoutGroup>().padding.top = 2;
        FloorItem.GetComponent<GridLayoutGroup>().padding.bottom = 2;
        FloorItem.GetComponent<GridLayoutGroup>().cellSize = new Vector2(FloorItemWidth, 20);
        FloorItem.GetComponent<GridLayoutGroup>().spacing = new Vector2(0, 1);

        FloorItem.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FloorItemWidth);
        FloorItem.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, FloorItemHigh);
        GameObject APanel = FloorItem.transform.Find("APanel").gameObject;
        GameObject BPanel = FloorItem.transform.Find("BPanel").gameObject;
        APanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FloorItemWidth);
        BPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FloorItemWidth);

        //添加Bin
        GameObject Bin = (GameObject)Resources.Load("Scene/Simulation/Bin");
        for (int i = 0; i < ColumnNum; i++)
        {
            GameObject clone = Instantiate(Bin);
            clone.name = Bin.name + (i + 1).ToString() + "A";
            clone.transform.parent = APanel.transform;
        }
        for (int i = 0; i < ColumnNum; i++)
        {
            GameObject clone = Instantiate(Bin);
            clone.name = Bin.name + (i + 1).ToString() + "B";
            clone.transform.parent = BPanel.transform;
        }
        //
        #endregion

        #region 添加FloorItem
        int BinsPanelHigh = 2 * 2 + FloorItemHigh * FloorNum + 2 * (FloorNum - 1);
        int BinsPanelWidth = FloorItemWidth + 2 * 2;
        BinsPanel.GetComponent<VerticalLayoutGroup>().padding.left = 2;
        BinsPanel.GetComponent<VerticalLayoutGroup>().padding.right = 2;
        BinsPanel.GetComponent<VerticalLayoutGroup>().padding.top = 2;
        BinsPanel.GetComponent<VerticalLayoutGroup>().padding.bottom = 2;
        BinsPanel.GetComponent<VerticalLayoutGroup>().spacing = 2;
        //修改BinsPanel尺寸
        BinsPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BinsPanelWidth);
        BinsPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BinsPanelHigh);

        //添加FloorItem
        for (int i = 1; i < FloorNum; i++)
        {
            GameObject clone = Instantiate(FloorItem);
            clone.name = FloorItem.name + (i + 1).ToString();
            clone.transform.parent = BinsPanel.transform;
        }
        FloorItem.name = FloorItem.name + 1.ToString();
        #endregion

        #region 修改设计FloorNumPanel
        //FloorNumPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BinsPanelHigh);
        GameObject Num = FloorNumPanel.transform.Find("Num").gameObject;
        FloorNumPanel.GetComponent<VerticalLayoutGroup>().padding.left = 1;
        FloorNumPanel.GetComponent<VerticalLayoutGroup>().padding.right = 1;
        FloorNumPanel.GetComponent<VerticalLayoutGroup>().padding.top = 2;
        FloorNumPanel.GetComponent<VerticalLayoutGroup>().padding.bottom = 2;
        FloorNumPanel.GetComponent<VerticalLayoutGroup>().spacing = 2;
        for (int i = 1; i < FloorNum; i++)
        {
            GameObject clone = Instantiate(Num);
            clone.name = Num.name + (i + 1).ToString();
            clone.GetComponent<Text>().text = (i + 1).ToString();
            clone.transform.parent = FloorNumPanel.transform;
        }
        Num.name = Num.name + 1.ToString();
        #endregion

        #region 修改设计ColumnNumPanel
        GameObject ColumnNumPanel = ShelfPanel.transform.Find("ColumnNumPanel").gameObject;
        ColumnNumPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BinsPanelWidth);
        GameObject Num2 = ColumnNumPanel.transform.Find("Num").gameObject;
        ColumnNumPanel.GetComponent<HorizontalLayoutGroup>().padding.left = 2;
        ColumnNumPanel.GetComponent<HorizontalLayoutGroup>().padding.right = 2;
        ColumnNumPanel.GetComponent<HorizontalLayoutGroup>().padding.top = 1;
        ColumnNumPanel.GetComponent<HorizontalLayoutGroup>().padding.bottom = 1;
        ColumnNumPanel.GetComponent<HorizontalLayoutGroup>().spacing = 1;
        for (int i = 1; i < ColumnNum; i++)
        {
            GameObject clone = Instantiate(Num2);
            clone.name = Num2.name + (i + 1).ToString();
            clone.GetComponent<Text>().text = (i + 1).ToString();
            clone.transform.parent = ColumnNumPanel.transform;
        }
        Num2.name = Num2.name + 1.ToString();
        #endregion

        #region 修改各级Panel尺寸
        Panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BinsPanelWidth + 40);
        Panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BinsPanelHigh + 40);
        ShelfPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BinsPanelWidth + 40);
        ShelfPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BinsPanelHigh + 20);
        ShelfPanel.transform.Find("Scroll View").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BinsPanelWidth + 40);
        //ShelfPanel.transform.Find("Scroll View").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BinsPanelHigh + 20);
        #endregion

        #region 添加ShelfPanel
        for (int i = 1; i < ShelfNum; i++)
        {
            GameObject clone = Instantiate(ShelfPanel);
            clone.name = ShelfPanel.name + (i + 1).ToString();
            clone.transform.parent = Content.transform;
            clone.transform.Find("Title").GetComponent<Text>().text = (i + 1).ToString() + "号货架";
        }
        ShelfPanel.name = ShelfPanel.name + 1.ToString();
        #endregion

        #region 修改Button的name以方便查找
        for (int i = 0; i < ShelfNum; i++)
        {
            string ShelfName = "ShelfPanel" + (i + 1).ToString();
            GameObject shelf = Content.transform.Find(ShelfName).gameObject;
            for (int j = 0; j < FloorNum; j++)
            {
                string FloorItemName = "Scroll View/Viewport/Content/Panel/BinsPanel/FloorItem" + (j + 1).ToString();
                GameObject floorItem = shelf.transform.Find(FloorItemName).gameObject;
                GameObject apanel = floorItem.transform.Find("APanel").gameObject;
                GameObject bpanel = floorItem.transform.Find("BPanel").gameObject;
                for (int k = 0; k < ColumnNum; k++)
                {
                    string BinAName = "Bin" + (k + 1).ToString() + "A";
                    string BinBName = "Bin" + (k + 1).ToString() + "B";
                    string BinAName1 = "Bin_" + "Cargo_" + (i + 1).ToString() + "_" + (j + 1).ToString() + "_" + (k + 1).ToString() + "_A";
                    string BinBName1 = "Bin_" + "Cargo_" + (i + 1).ToString() + "_" + (j + 1).ToString() + "_" + (k + 1).ToString() + "_B";
                    apanel.transform.Find(BinAName).name = BinAName1;
                    bpanel.transform.Find(BinBName).name = BinBName1;
                }
            }
        }
        #endregion
        //保存为预制体
        MyClass.CreatePrefab(StorageStateInterface, "Assets/Resources/Scene/Simulation/" + StorageStateInterface.name);
    }

    public static void Ajustment2(int ShelfNum, int FloorNum, int ColumnNum)
    {
        GameObject CreateInterface0 = (GameObject)Resources.Load("Scene/Simulation/CreateInterface0");
        CreateInterface0.name = "CreateInterface";
        GameObject Panel = CreateInterface0.transform.Find("Panel").gameObject;
        Panel.transform.Find("HighBayNum").transform.Find("InputField").transform.Find("Placeholder").GetComponent<Text>().text = "输入1-" + ShelfNum.ToString() + "内任意数字";
        Panel.transform.Find("FloorNum").transform.Find("InputField").transform.Find("Placeholder").GetComponent<Text>().text = "输入1-" + FloorNum.ToString() + "内任意数字";
        Panel.transform.Find("ColumnNum").transform.Find("InputField").transform.Find("Placeholder").GetComponent<Text>().text = "输入1-" + ColumnNum.ToString() + "内任意数字";
        //保存为预制体
        MyClass.CreatePrefab(CreateInterface0, "Assets/Resources/Scene/Simulation/" + CreateInterface0.name);
    }
}
