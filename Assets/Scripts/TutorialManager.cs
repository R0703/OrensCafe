using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPopup; 

    public void ShowTutorial()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(true); 
        }
    }

    public void CloseTutorial()
    {
        if (tutorialPopup != null)
        {
            tutorialPopup.SetActive(false);
        }
    }
}
