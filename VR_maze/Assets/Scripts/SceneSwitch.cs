using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneSwitch : XRGrabInteractable
{
    public string destinationScene;
    private static string sourceScene = "";
    public string currentScene;
    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        base.OnSelectEnter(interactor);
        sourceScene = currentScene;
        SceneManager.LoadScene(destinationScene);
    }

    public static string getSourceScene()
    {
        return sourceScene;
    }

}
