using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class BoxGenerator : MonoBehaviour
{
    public GameObject           fallCheck;
    public int                  max;
    public float                waitTime;
    public float                regenTime;
    private int                 total;
    private bool                isReady;
    public GameObject           box;
    private List<GameObject>    boxes;
    private Transform           origin;

    private void Start()
    {
        origin      = transform;
        total       = 0;
        isReady     = false;
        boxes       = new List<GameObject>();
        Invoke("GetReady", waitTime);
    }

    private void Update()
    {
        if (total < max)
        {
            GenerateBox();
        }
        ManageBox();
    }

    private bool FallCheck(Vector2 vector)
    {
        if (vector.y < fallCheck.transform.position.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GenerateBox()
    {
        if (isReady)
        {
            GameObject item = Instantiate(box, origin.position, Quaternion.identity);
            boxes.Add(item);
            total++;
            isReady = false;
            Invoke("GetReady", regenTime);
        }
    }

    private void ManageBox()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            GameObject item                 = boxes[i];
            PhysicalObject physicalObject   = item.GetComponent<PhysicalObject>();
            if (physicalObject.GetDestroyed())
            {
                physicalObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            Vector2 position = physicalObject.transform.position;
            if (FallCheck(position))
            {
                RemoveBox(i);
            }
        }
    }

    private void RemoveBox(int idx)
    {
        Destroy(boxes[idx]);
        boxes.RemoveAt(idx);
        total--;
    }

    private void GetReady()
    {
        isReady = true;
    }

    private void OnDrawGizmos()
    {
        Vector2 origin  = fallCheck.transform.position;
        Vector2 start   = new Vector2(origin.x - 1000 ,origin.y);
        Vector2 end     = new Vector2(origin.x + 1000, origin.y);
        Gizmos.DrawLine(start, end);
    }
}