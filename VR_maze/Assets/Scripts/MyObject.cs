using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObject : MonoBehaviour
{

    private bool isOutlier = false;
    private GameObject outliers;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        outliers = GameObject.Find("Outliers");

        if (GameObject.ReferenceEquals(this.transform.parent.gameObject, outliers)) {
            isOutlier = true;
        }

        startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y <= -50)
        {
            Debug.Log("coucou");
            if (isOutlier)
            {
                Destroy(gameObject);
            } else
            {
                this.transform.position = new Vector3(startPosition[0], startPosition[1] + 5, startPosition[2]);
            }
        }
    }

    public bool getIsOutlier()
    {
        return isOutlier;
    }
}
