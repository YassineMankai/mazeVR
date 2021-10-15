using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Differences : MonoBehaviour
{
    private bool finished = false;
    private GameObject Portal;
    private MyObject[] objects;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("Started");
        Portal = GameObject.Find("Portal");
        Debug.Log("close portal");
        Portal.GetComponent<SceneSwitch>().setClosed();

        objects = (MyObject[]) GameObject.FindObjectsOfType<MyObject>();
    }

    // Update is called once per frame
    void Update()
    {

        finished = true;
        objects = (MyObject[])GameObject.FindObjectsOfType<MyObject>();
        foreach (MyObject o in objects)
        {
            if(o.getIsOutlier())
            {
                finished = false;
            }
        }

        if (finished)
        {
            Portal.GetComponent<SceneSwitch>().setOpen();
        }
        
    }
}
