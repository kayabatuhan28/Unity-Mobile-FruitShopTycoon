using MobileTycoon;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void HandleButtonAction(int actionIndex)
    {
        UIAction action = (UIAction)actionIndex;
        switch (action)
        {
            case UIAction.StartGame:
                SceneManager.LoadScene(1);
                break;           
            case UIAction.Quit:
                Application.Quit();
                break;

        }
    }

}
