using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourDController : MonoBehaviour
{

    //in ticks
    public int maxTime = -10000;
    public int minTime = 10000;

    private float tickTimer = 0;
    private float tickDelay;

    private int oldestChildrenIndex = 0;
    private List<GameObjectNode> childrenTrees = new List<GameObjectNode>();

    private void Start()
    {
        childrenTrees.Capacity = TimeViewerParams.InstancesCount;

        tickDelay = 1 / TimeViewerParams.TicksPerSecond;

        for (int i = TimeViewerParams.InstancesCount - 1; i >= 0; i--)
        {
            GlobalClock.CurrentTime = -i;

            oldestChildrenIndex = childrenTrees.Count;
            var newNode = new GameObjectNode(null, gameObject);
            childrenTrees.Add(newNode);

            maxTime = Mathf.Max(GlobalClock.CurrentTime, maxTime);
            minTime = Mathf.Min(GlobalClock.CurrentTime, minTime);
        }

        GlobalClock.CurrentTime = 0;
    }

    void Update()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer < 0)
        {
            tickTimer += tickDelay;

            GlobalClock.CurrentTime += GlobalClock.TimeDirection;
            maxTime = Mathf.Max(GlobalClock.CurrentTime, maxTime);
            minTime = Mathf.Min(GlobalClock.CurrentTime, minTime);

            if (GlobalClock.CurrentTime == minTime)
            {
                GlobalClock.TimeDirection = 1;
            }

            GameObjectNode previousOldestNode = childrenTrees[oldestChildrenIndex];
            previousOldestNode.SetActive(true);

            oldestChildrenIndex = (oldestChildrenIndex + GlobalClock.TimeDirection + TimeViewerParams.InstancesCount) % TimeViewerParams.InstancesCount;
            GameObjectNode oldestNode = childrenTrees[oldestChildrenIndex];

            if (oldestNode.TimeStamp == maxTime)
            {
                maxTime--;
            }
            if (oldestNode.TimeStamp == minTime)
            {
                minTime++;
            }

            oldestNode.Reconstruct(null, gameObject);
            oldestNode.SetActive(false);


            float tickFraction = tickTimer / tickDelay;

            //transform.position = Vector3.Lerp(
            //    Vector3.forward * deltaZ * (maxTime - currentTime),
            //    Vector3.forward * deltaZ * (maxTime - currentTime - timeDirection),
            //    tickFraction);
            transform.position = Vector3.forward * TimeViewerParams.DeltaZ * (maxTime - GlobalClock.CurrentTime);
            //print(tickFraction);

            foreach (GameObjectNode state in childrenTrees)
            {
                //state.MyGameObject.transform.position = Vector3.Lerp(
                //    Vector3.forward * deltaZ * (maxTime - state.TimeStamp),
                //    Vector3.forward * deltaZ * (maxTime - state.TimeStamp - timeDirection),
                //    tickFraction);
                state.MyGameObject.transform.position = Vector3.forward * TimeViewerParams.DeltaZ * (maxTime - state.TimeStamp);

                if ((state.TimeStamp >= GlobalClock.CurrentTime && maxTime != GlobalClock.CurrentTime) 
                    || GlobalClock.CurrentTime == minTime)
                {
                    state.Update(true, 1f * (maxTime - state.TimeStamp) / (maxTime - GlobalClock.CurrentTime));
                }
                else
                {
                    state.Update(false, 1f * (state.TimeStamp - minTime) / (GlobalClock.CurrentTime - minTime));
                }
            }
        }
    }
}
