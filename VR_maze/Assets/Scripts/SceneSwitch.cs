using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneSwitch : XRGrabInteractable
{
    public string destinationScene;
    private static string sourceScene = "";
    public string currentScene;
    private bool isOpen;
    public Material openMaterial;
    public Material closedMaterial;

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        if (!isOpen)
        {
            return;
        }
        base.OnSelectEnter(interactor);
        sourceScene = currentScene;
        SceneManager.LoadScene(destinationScene);
    }

    public static string getSourceScene()
    {
        return sourceScene;
    }

    public void setOpen()
    {
        isOpen = true;
        gameObject.transform.Find("Sphere").GetComponent<Renderer>().material = openMaterial;
    }

    public void setClosed()
    {
        Debug.Log("closed");
        isOpen = false;
        gameObject.transform.Find("Sphere").GetComponent<Renderer>().material = closedMaterial;
    }

}
