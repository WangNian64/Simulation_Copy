using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static partial class FindExtension
{
    /// <summary>  
    /// 获取子对象变换集合  
    /// </summary>  
    /// <param name="obj"></param>  
    /// <returns></returns>  
    public static List<Transform> GetChildCollection(this Transform obj)
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < obj.childCount; i++)
        {
            list.Add(obj.GetChild(i));
        }
        return list;
    }

    /// <summary>  
    /// 获取子对象集合  
    /// </summary>  
    /// <param name="obj"></param>  
    /// <returns></returns>  
    public static List<GameObject> GetChildCollection(this GameObject obj)
    {
        var list = obj.transform.GetChildCollection();
        return list.ConvertAll(T => T.gameObject);
    }

    public static Transform GetRootParent(this Transform obj)
    {
        Transform Root = obj.parent;
        while (Root.parent != null)
        {
            //Root = Root.root;   //transform.root,方法可以直接获取最上父节点。  
            Root = Root.parent;
        }
        return Root;
    }

    /// <summary>  
    /// 把源对象身上的所有组件，添加到目标对象身上  
    /// </summary>  
    /// <param name="origin">源对象</param>  
    /// <param name="target">目标对象</param>  
    public static void CopyComponent(GameObject origin, GameObject target)
    {
        var originComs = origin.GetComponents<Component>();
        foreach (var item in originComs)
        {
            target.AddComponent(item.GetType());
        }
    }

    /// <summary>  
    /// 改变游戏脚本  
    /// </summary>  
    /// <param name="origin"></param>  
    /// <param name="target"></param>  
    public static void ChangeScriptTo(this MonoBehaviour origin, MonoBehaviour target)
    {
        target.enabled = true;
        origin.enabled = false;
    }


    /// <summary>  
    /// 从当前对象的子对象中查找，返回一个用tag做标识的活动的游戏物体的链表.如果没有找到则为空.   
    /// </summary>  
    /// <param name="obj">对象Transform</param>  
    /// <param name="tag">标签</param>  
    /// <param name="transList">结果Transform集合</param> 
    /// 对一个父对象进行递归遍历，如果有子对象的tag和给定tag相符合时，则把该子对象存到 链表数组中  
    public static void FindGameObjectsWithTagRecursive(this Transform obj, string tag, ref List<Transform> transList)
    {
        foreach (var item in obj.transform.GetChildCollection())
        {
            // 如果子对象还有子对象，则再对子对象的子对象进行递归遍历  
            if (item.childCount > 0)
            {
                item.FindGameObjectsWithTagRecursive(tag, ref transList);
            }

            if (item.tag == tag)
            {
                transList.Add(item);
            }
        }
    }

    //根据tag查找某个物体的子物体
    public static void FindChildWithTag(GameObject obj, string tag, ref List<GameObject> objList)
    {
        foreach (GameObject item in obj.GetChildCollection())
        {
            // 如果子对象还有子对象，则再对子对象的子对象进行递归遍历 
            //if (item.GetChildCollection().Count > 0)
            //{
            //    FindChildWithTag(item, tag, ref objList);
            //}
            if (item.tag.Equals(tag))
            {
                objList.Add(item);
            }
        }
    }

    public static void FindGameObjectsWithTagRecursive(this GameObject obj, string tag, ref List<GameObject> objList)
    {
        List<Transform> list = new List<Transform>();
        obj.transform.FindGameObjectsWithTagRecursive(tag, ref list);

        objList.AddRange(list.ConvertAll(T => T.gameObject));
    }

    /// <summary>  
    /// 从父对象中查找组件  
    /// </summary>  
    /// <typeparam name="T">组件类型</typeparam>  
    /// <param name="com">物体组件</param>  
    /// <param name="parentLevel">向上查找的级别，使用 1 表示与本对象最近的一个级别</param>  
    /// <param name="searchDepth">查找深度</param>  
    /// <returns>查找成功返回相应组件对象，否则返回null</returns>  
    public static T GetComponentInParent<T>(this Component com, int parentLevel = 1, int searchDepth = int.MaxValue) where T : Component
    {
        searchDepth--;

        if (com != null && searchDepth > 0)
        {
            var component = com.transform.parent.GetComponent<T>();
            if (component != null)
            {
                parentLevel--;
                if (parentLevel == 0)
                {
                    return component;
                }
            }

            return com.transform.parent.GetComponentInParent<T>(parentLevel, searchDepth);
        }

        return null;
    }
}