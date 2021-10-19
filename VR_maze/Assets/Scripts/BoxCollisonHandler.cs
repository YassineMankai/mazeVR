using UnityEngine;

public class BoxCollisonHandler : MonoBehaviour
{
    private GameObject PlayGround;
    private GameObject otherBox;
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
        if (transform.position.y < -40) // prevents the user from throwing boxes out of range
        {
            transform.localPosition = new Vector3(1 + 2.5f * (freeBoxIndex / 12), 7, 2+ 3.0f * (freeBoxIndex % 12));
        }
        
        if (collided && Vector3.Distance(otherBox.transform.position, transform.position) <1.95f) // check if the collision is intentional
        {
            Debug.Log("inserted");
            inserted = true;
            collided = false;
            PlayGround.GetComponent<PipeGeneration>().HandleBoxInsertion(otherBox, gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) // check if the collided empty cube is waiting for the same pipe type as this cube
    {
        GameObject collidedBox = collision.gameObject;
        
        if ((tag == "CornerBox" && collidedBox.tag == "EmptyCornerBox") || (tag == "DirectBox" && collidedBox.tag == "EmptyDirectBox"))
        {
            otherBox = collidedBox;
            Debug.Log("collided");
            collided = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collided && !inserted)
        {
            Debug.Log("exit collision");
            collided = false;
        }
    }



}
