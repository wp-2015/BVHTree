using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MakeMap : MonoBehaviour
{
    public List<Entity> lEntitys = new List<Entity>(1000);
    private MakeBVHHandle handle = new MakeBVHHandle();
    public int iXMax = 500;
    public int iYMax = 500;
    public int iEntityNum = 500;
    
    public List<Entity> lBoomEntitys = new List<Entity>();
    public int iBoomBum = 5;
    void Start()
    {
        MakeEntitys();
    }

    private void MakeEntitys()
    {
        lEntitys.Clear();
        for (int i = 0; i < iEntityNum; i++)
        {
            var x = Random.Range(0, iXMax);
            var y = Random.Range(0, iYMax);
            lEntitys.Add(new Entity(){id = i, pos = new Vector2(x, y), radius = Random.Range(1,3)});
        }
        // lEntitys.Add(new Entity(){id = 1, pos = new Vector2(0,50)});
        // lEntitys.Add(new Entity(){id = 1, pos = new Vector2(50,0)});
        // lEntitys.Add(new Entity(){id = 1, pos = new Vector2(100,50)});
        // lEntitys.Add(new Entity(){id = 1, pos = new Vector2(50,100)});
        // lEntitys.Add(new Entity(){id = 1, pos = new Vector2(100,100), radius = 20});
        MakeBVH();
    }

    private void MakeBVH()
    {
        handle.MakeTree(lEntitys);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("开关范围检测"))
        {
            bIsShowBVH = !bIsShowBVH;
        }
        if (GUILayout.Button("重新生成Entity"))
        {
            MakeEntitys();
        }
        if (GUILayout.Button("重新生成BVH"))
        {
            MakeBVH();
        }

        if (GUILayout.Button("随机爆炸"))
        {
            MakeBoom();
        }
    }

    private bool bIsShowBVH = false;

    private void MakeBoom()
    {
        lBoomEntitys.Clear();
        for (int i = 0; i < iBoomBum; i++)
        {
            var x = Random.Range(0, iXMax);
            var y = Random.Range(0, iYMax);
            var entity = new Entity(){id = i, pos = new Vector2(x, y), radius = Random.Range(10,30)};
            lBoomEntitys.Add(entity);

            var res= handle.FindCircleCloud(entity.pos, entity.radius);
            foreach (var romeveEntity in res)
            {
                lEntitys.Remove(romeveEntity);
            }
        }
    }

    private void DrawBoom()
    {
        foreach (var booms in lBoomEntitys)
        {
            DrawCircles(booms.pos, booms.radius);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white; //更改颜色
        foreach (var entity in lEntitys)
        {
            Gizmos.DrawSphere(entity.pos, entity.radius);//
        }
        if(bIsShowBVH)
            DrawNode(handle.root);

        Gizmos.color = Color.red; //更改颜色
        DrawBoom();
    }

    private void DrawNode(BVHNode node)
    {
        if(node == null) return;
        Gizmos.color = Color.green; //更改颜色
        DrawCircles(node.center, node.fRadius);
        var childs = node.childs;
        foreach (var child in childs)
            DrawNode(child);
    }
    private void DrawCircles(Vector2 pos, float radius)
    {
        Vector3 beginPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f)
        {
            Vector3 endPoint = new Vector3(radius * Mathf.Cos(theta) + pos.x, radius * Mathf.Sin(theta) + pos.y);
            if (theta == 0)
            {
                firstPoint = endPoint;
            }
            else
            {
                Gizmos.DrawLine(beginPoint, endPoint);
            }
            beginPoint = endPoint;
        }
        // 绘制最后一条线段
        Gizmos.DrawLine(firstPoint, beginPoint);
    }
}
