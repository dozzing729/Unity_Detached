using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class BoxGenerator : MonoBehaviour
{
    public int                  max;
    public float                waitTime;
    public float                regenTime;
    private int                 total;
    private Boolean             isReady;
    public GameObject           box;
    private List<GameObject>    boxes;
    private List<Boolean>       trash;
    private Transform           origin;

    void Start()
    {
        origin      = transform;
        total       = 0;
        isReady     = false;
        boxes       = new List<GameObject>();
        trash       = new List<Boolean>();
        Invoke("GetReady", waitTime);
    }

    void Update()
    {
        if (total < max)
        {
            GenerateBox();
        }
        ManageBox();
    }

    void GenerateBox()
    {
        if (isReady)
        {
            GameObject item = Instantiate(box, origin.position, Quaternion.identity);
            boxes.Add(item);
            trash.Add(false);
            total++;
            isReady = false;
            Invoke("GetReady", regenTime);
        }
    }

    void ManageBox()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            GameObject item                 = boxes[i];
            PhysicalObject physicalObject   = item.GetComponent<PhysicalObject>();
            if (physicalObject.GetDestroyed())
            {
                if (!trash[i])
                {
                    StartCoroutine(RemoveBox(i, 2.0f));
                    trash[i] = true;
                }
            }
        }
    }

    IEnumerator RemoveBox(int idx, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(boxes[idx]);
        boxes.RemoveAt(idx);
        trash.RemoveAt(idx);
        total--;
    }

    void GetReady()
    {
        isReady = true;
    }
}