using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Differences : MonoBehaviour
{
    private bool finished = false;
    private GameObject portal;
    private MyObject[] objects;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Started");
        portal = GameObject.Find("Portal");
        portal.SetActive(false);

        objects = (MyObject[]) GameObject.FindObjectsOfType<MyObject>();
    }

    // Update is called once per frame
    void Update()
    {

        finished = true;
        objects = (MyObject[])GameObject.FindObjectsOfType<MyObject>();
        foreach (MyObject o in objects)
        {
            Debug.Log(o.getIsOutlier());
            if(o.getIsOutlier())
            {
                finished = false;
            }
        }

        if (finished)
        {
            Debug.Log("appear");
            portal.SetActive(true);
        }
        
    }
}
