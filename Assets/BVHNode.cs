using System.Collections.Generic;
using UnityEngine;

public class BVHNode
{
    public List<Entity> leafs = new List<Entity>();
    public List<BVHNode> childs = new List<BVHNode>();
    public Vector2 center;
    public float fRadius;
    public void Init()
    {
        leafs.Clear();
        childs.Clear();
    }

    public void Clear()
    {

    }
}