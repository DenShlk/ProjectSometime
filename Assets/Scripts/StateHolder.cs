using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHolder : MonoBehaviour
{
    public SpriteRenderer myRenderer;

    private bool active = true;
    public bool destroyed = false;
    private int baseObjectId;

    public int BaseObjectId
    {
        get => baseObjectId;
        set
        {
            baseObjectId = value;
            if (!idToDestroyedInstances.ContainsKey(BaseObjectId))
            {
                idToDestroyedInstances.Add(BaseObjectId, new HashSet<StateHolder>());
            }
        }
    }

    public bool IsActive
    {
        get
        {
            if (GlobalClock.TimeDirection == 0)
                return false;//frozen
            return active;
        }
        set
        {
            //todo - singleton
            active = value;
            onActiveChange(value);
        }
    }

    public bool IsDestroyed
    {
        get => destroyed;
        set
        {
            destroyed = value;
            if (value && IsActive)
                IsActive = false;
            onDestroyedChange(value);
        }
    }

    //garbage collecting
    private static Dictionary<int, HashSet<StateHolder>> idToDestroyedInstances = new Dictionary<int, HashSet<StateHolder>>();

    public bool IsAllInstancesDestroyed()
    {
        if(BaseObjectId == 0)
        {
            return false;//it has not been even initialized
        }
        return idToDestroyedInstances[BaseObjectId].Count >= TimeViewerParams.InstancesCount;
    }

    private static Dictionary<int, int> idToDestroyedCount = new Dictionary<int, int>();

    private void OnDestroy()
    {
        if (idToDestroyedCount.ContainsKey(BaseObjectId))
        {
            idToDestroyedCount[BaseObjectId]++;
            if(idToDestroyedCount[BaseObjectId] == TimeViewerParams.InstancesCount + 100)
            {
                idToDestroyedCount.Remove(BaseObjectId);
                idToDestroyedInstances.Remove(BaseObjectId);
            }
        }
        else
            idToDestroyedCount[BaseObjectId] = 1;
    }

    private void onDestroyedChange(bool value)
    {
        //if not destroyed by node yet
        if (idToDestroyedInstances.ContainsKey(BaseObjectId))
        {
            var baseObjectSet = idToDestroyedInstances[BaseObjectId];
            if (destroyed)
            {
                baseObjectSet.Add(this);
            }
            else
            {
                if (baseObjectSet.Contains(this))
                    baseObjectSet.Remove(this);
            }
        }
    }

    protected virtual void onActiveChange(bool value)
    {
        if(myRenderer == null)
        {
            myRenderer = GetComponent<SpriteRenderer>();
        }
        if (myRenderer != null)
        {
            myRenderer.enabled = value;
        }
    }

    //Adds same component to other gameobject if it does not exist,
    //otherwise copies values.
    public void CopyTo(GameObject other, Transform parent)
    {
        if (myRenderer == null)
        {
            myRenderer = GetComponent<SpriteRenderer>();
        }

        other.transform.parent = parent;
        other.transform.position = transform.position;
        other.transform.rotation = transform.rotation;
        other.transform.localScale = transform.localScale;
        //possible bag if create SpriteRenderer's in runtime - you are crazy if you do
        if (myRenderer != null)
        {
            var otherRenderer = CreateIfNotExist<SpriteRenderer>(other);
            otherRenderer.sprite = myRenderer.sprite;
            otherRenderer.color = myRenderer.color;
            otherRenderer.material = myRenderer.material;

            otherRenderer.size = myRenderer.size;
            otherRenderer.flipX = myRenderer.flipX;
            otherRenderer.flipY = myRenderer.flipY;

            otherRenderer.drawMode = myRenderer.drawMode;
            otherRenderer.adaptiveModeThreshold = myRenderer.adaptiveModeThreshold;
            otherRenderer.allowOcclusionWhenDynamic = myRenderer.allowOcclusionWhenDynamic;
            otherRenderer.forceRenderingOff = myRenderer.forceRenderingOff;
        }

        CopyTo(other);
    }

    public virtual void CopyTo(GameObject other)
    {
        StateHolder otherState = CreateIfNotExist<StateHolder>(other);

        otherState.BaseObjectId = BaseObjectId;
        otherState.IsActive = IsActive;
        otherState.IsDestroyed = IsDestroyed;
    }

    protected T CreateIfNotExist<T>(GameObject other)
        where T : Component
    {
        T holder = other.GetComponent<T>();
        if (holder == null)
        {
            holder = other.AddComponent<T>();
        }

        return holder;
    }
}