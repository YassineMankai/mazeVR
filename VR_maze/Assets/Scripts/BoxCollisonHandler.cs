using UnityEngine;

public class BoxCollisonHandler : MonoBehaviour
{
    private GameObject PlayGround;
    private GameObject otherBox;
    Vector3 emptyBoxPos;
    private bool collided = false;
    private bool inserted = false;
    private int freeBoxIndex;

    public void setFreeBoxIndex(int index)
    {
        freeBoxIndex = index;
    }

    private void Start()
    {
        PlayGround = GameObject.Find("PlayGround");
    }

    private void Update()
    {
        if (transform.position.y < -2)
        {
            transform.localPosition = new Vector3(1 + 2.5f * (freeBoxIndex / 12), 7, 2+ 3.0f * (freeBoxIndex % 12));
        }
        
        if (collided && Vector3.Distance(otherBox.transform.position, transform.position) <1.7f)
        {
            Debug.Log("inserted");
            inserted = true;
            collided = false;
            PlayGround.GetComponent<PipeGeneration>().HandleBoxCollision(new Vector3Int((int)emptyBoxPos.x / 2, (int)emptyBoxPos.y / 2, (int)emptyBoxPos.z / 2), gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        otherBox = collision.gameObject;
        
        if ((tag == "CornerBox" && otherBox.tag == "EmptyCornerBox") || (tag == "DirectBox" && otherBox.tag == "EmptyDirectBox"))
        {
            Debug.Log("collided");
            collided = true;
            emptyBoxPos = otherBox.transform.localPosition;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collided && !inserted)
        {
            Debug.Log("activate");
            collided = false;
        }
    }



}
