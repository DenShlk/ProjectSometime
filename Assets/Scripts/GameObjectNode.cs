using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectNode
{ 
    //List<GameObjectNode> children = new List<GameObjectNode>();
    Dictionary<int, GameObjectNode> instanseIdToNode = new Dictionary<int, GameObjectNode>();

    public int TimeStamp { get; private set; }
    public GameObject MyGameObject { get; private set; }

    private StateHolder nodeState;

    public GameObjectNode(GameObjectNode parentNode, GameObject go)
    {
        //creating everything required
        MyGameObject = new GameObject(go.name);
        if (go.GetComponent<SpriteRenderer>() != null)
        {
            MyGameObject.AddComponent<SpriteRenderer>();
        }
        nodeState = go.GetComponent<StateHolder>();
        nodeState.BaseObjectId = go.GetInstanceID();
        //Reconstruct will set up node
        Reconstruct(parentNode, go);
    }

    public void Reconstruct(GameObjectNode parentNode, GameObject go)
    {
        this.TimeStamp = GlobalClock.CurrentTime;
        //do not create any components here!

        var originalRenderer = go.GetComponent<SpriteRenderer>();
        var renderer = MyGameObject.GetComponent<SpriteRenderer>();
        Transform parentTransform = parentNode == null ? null : parentNode.MyGameObject.transform;

        StateHolder goStateHolder = go.GetComponent<StateHolder>();
        if (GlobalClock.TimeDirection >= 0)
        {
            if (goStateHolder != null)
            {
                goStateHolder.CopyTo(MyGameObject, parentTransform);
                nodeState = MyGameObject.GetComponent<StateHolder>();
            }
            else
            {
                if (nodeState != null)
                {
                    Object.Destroy(nodeState);
                    nodeState = null;
                }
            }
        }
        else
        {
            if (nodeState != null)
            {
                if (renderer != null)
                {
                    //prevent copying animated value (from update).
                    renderer.color = initialColor;
                }
                
                nodeState.CopyTo(go, go.transform.parent);
            }
            else
            {
                if (goStateHolder != null)
                {
                    Object.Destroy(goStateHolder);
                }
            }
        }

        SortedSet<int> unusedIds = new SortedSet<int>(instanseIdToNode.Keys);
        for (int i = 0; i < go.transform.childCount; i++)
        {
            var childState = go.transform.GetChild(i).GetComponent<StateHolder>();
            if(childState != null)
            {
                var child = childState.gameObject;
                if (childState.IsAllInstancesDestroyed())
                {
                    if (instanseIdToNode.ContainsKey(child.GetInstanceID()))
                    {
                        unusedIds.Remove(child.GetInstanceID());
                        instanseIdToNode[child.GetInstanceID()].Destroy();
                        instanseIdToNode.Remove(child.GetInstanceID());
                    }
                    Object.Destroy(child.gameObject);
                }
                else
                {
                    if (instanseIdToNode.ContainsKey(child.GetInstanceID()))
                    {
                        var childNode = instanseIdToNode[child.GetInstanceID()];
                        childNode.Reconstruct(this, child);
                        unusedIds.Remove(child.GetInstanceID());
                    }
                    else
                    {
                        instanseIdToNode.Add(child.GetInstanceID(), new GameObjectNode(this, child));
                    }
                }
            }
        }
        foreach (int id in unusedIds)
        {
            //GameObjectNode node = instanseIdToNode[id];
            //nodeToDestroy.nodeState.IsActive = false;
            //Object.Destroy(nodeToDestroy.MyGameObject);
            //instanseIdToNode.Remove(id);
        }

        if (renderer != null)
        {
            initialColor = renderer.color;  
        }
    }

    private Color initialColor;
    public void Update(bool inFuture, float deltaFraction)
    {
        var renderer = MyGameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            
            Color color = initialColor;
            if(inFuture)
            {
                color.a = Mathf.Clamp(deltaFraction * deltaFraction, 0.1f, 0.8f) * initialColor.a;
            }
            else
            {
                color.a = Mathf.Clamp(deltaFraction * deltaFraction * 2, 0.1f, 0.9f) * initialColor.a;
            }
            renderer.color = color;
        }

        foreach(var node in instanseIdToNode.Values)
        {
            node.Update(inFuture, deltaFraction);
        }
    }

    private bool wasActiveBeforeDeactivation = true;
    public void SetActive(bool value)
    {
        if (!value)
        {
            wasActiveBeforeDeactivation = nodeState.IsActive;
            nodeState.IsActive = false;
        }
        else
        {
            nodeState.IsActive = wasActiveBeforeDeactivation;
        }
        var keysToDestroy = new List<int>();
        foreach(var pair in instanseIdToNode)
        {
            var node = pair.Value;
            if (node.nodeState.IsAllInstancesDestroyed())
                keysToDestroy.Add(pair.Key);
            else
                node.SetActive(value);
        }
        foreach(var key in keysToDestroy)
        {
            instanseIdToNode[key].Destroy();
            instanseIdToNode.Remove(key);
        }
    }

    private void Destroy()
    {
        nodeState.IsDestroyed = true;
        foreach(var node in instanseIdToNode.Values)
        {
            node.Destroy();
        }
        Object.Destroy(MyGameObject);   
    }
}
