using System;
using System.Collections.Generic;
using UnityEngine;

public class MakeBVHHandle
{
    public BVHNode root;
    public void MakeTree(List<Entity> entities)
    {
        if(entities == null || entities.Count <= 0) return;
        root = MakeNode(entities);
    }

    public List<Entity> FindCircleCloud(Vector2 pos, float radiue)
    {
        List<Entity> res = new List<Entity>();
        TravelRoot(res, root, pos, radiue);
        return res;
    }

    private void TravelRoot(List<Entity> res, BVHNode node, Vector3 pos, float radius)
    {
        if(Vector3.Distance(pos, node.center) > (radius + node.fRadius))
            return;
        
        if (node.leafs.Count > 0 && node.childs.Count < 1)
        {
            foreach (var leaf in node.leafs)
            {
                if(Vector3.Distance(pos, leaf.pos) <= (radius + leaf.radius))
                    res.Add(leaf);
            }
        }
        else if(node.childs.Count > 0)
        {
            foreach (var child in node.childs)
            {
                TravelRoot(res, child, pos, radius);
            }
        }
    }
    
    
    private BVHNode MakeNode(List<Entity> targets)
    {
        FindMiddle(targets, out Vector2 middle, out float radius, out float maxRadius);
        BVHNode node = new BVHNode();
        node.Init();
        node.center = middle;
        node.fRadius = radius + maxRadius;
        if (targets.Count == 1)
        {
            node.leafs.Add(targets[0]);
            return node;
        }

        if (radius <= 1)
        {
            foreach (var target in targets)
            {
                node.leafs.Add(target);
            }
            return node;
        }

        var dicZoneEntities = new Dictionary<int, List<Entity>>();
        foreach (var target in targets)
        {
            var x = target.pos.x;
            var y = target.pos.y;
            if (x <= middle.x && y <= middle.y)
            {
                AddEntityToParentList(dicZoneEntities, 0, target);
            }
            else if(x > middle.x && y <= middle.y)
            {
                AddEntityToParentList(dicZoneEntities, 1, target);
            }
            else if (x > middle.x && y > middle.y)
            {
                AddEntityToParentList(dicZoneEntities, 2, target);
            }
            else if(x <= middle.x && y > middle.y)
            {
                AddEntityToParentList(dicZoneEntities, 3, target);
            }
        }
        
        foreach (var zoneEntity in dicZoneEntities)
        {
            node.childs.Add(MakeNode(zoneEntity.Value));
        }
        return node;
    }

    public void AddEntityToParentList(Dictionary<int, List<Entity>> dicZoneEntities, int index, Entity entity)
    {
        if (!dicZoneEntities.TryGetValue(index, out List<Entity> res))
        {
            res = new List<Entity>();
            dicZoneEntities.Add(index, res);
        }
        res.Add(entity);
    }

    private void FindMiddle(List<Entity> targets, out Vector2 center, out float radius, out float maxRadius)
    {
        var xMax = targets[0].pos.x;
        var xMin = targets[0].pos.x;
        var yMax = targets[0].pos.y;
        var yMin = targets[0].pos.y;
        maxRadius = 0;
        foreach (var target in targets)
        {
            var x = target.pos.x;
            var y = target.pos.y;
            if (xMax < x)
                xMax = x;
            else if (xMin > x)
                xMin = x;
            if(yMax < y)
                yMax = y;
            else if (yMin > y)
                yMin = y;
            if (target.radius > maxRadius)
                maxRadius = target.radius;
        }
        var xRadius = (xMax - xMin) / 2;
        var yRadius = (yMax - yMin) / 2;
        center = new Vector2( xRadius + xMin, yRadius + yMin);
        radius = xRadius > yRadius ? xRadius : yRadius;
        foreach (var target in targets)
        {
            var targetPos = target.pos;
            var x = targetPos.x - center.x;
            var y = targetPos.y - center.y;
            var targetToCenterDis = x * x + y * y;
            if ((targetToCenterDis) > radius * radius)
            {
                targetToCenterDis = (float) Math.Sqrt(targetToCenterDis);
                radius = (radius + targetToCenterDis) * 0.5f;
                float scale = radius / targetToCenterDis;
                var centerX = targetPos.x - x * scale;
                var centerY = targetPos.y - y * scale;
                center = new Vector2(centerX, centerY);
            }
        }
    }
}