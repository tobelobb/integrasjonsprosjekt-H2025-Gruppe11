using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject MainPanel;
    public GameObject PlayPanel;
    public GameObject optionsPanel;
    public GameObject highscorePanel;
    public GameObject friendsPanel;
    public GameObject userPanel;

    public void OpenPlayMenu()
    {
        Debug.Log("Play Menu Opened");
        MainPanel.SetActive(false);
        PlayPanel.SetActive(true);
    }

    public void OpenOptions()
    {
        Debug.Log("Options Menu Opened");
        optionsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void OpenHighscore()
    {
        Debug.Log("Highscore Menu Opened");
        MainPanel.SetActive(false);
        highscorePanel.SetActive(true);
    }

    public void OpenMainMenu()
    {
        Debug.Log("Main Menu Opened");
        MainPanel.SetActive(true);
        PlayPanel.SetActive(false);
        optionsPanel.SetActive(false);
        friendsPanel.SetActive(false);
        highscorePanel.SetActive(false);
    }

    public void Quit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void OpenFriends()
    {
        Debug.Log("Friends Menu Opened");
        MainPanel.SetActive(false);
        friendsPanel.SetActive(true);
    }

    public void OpenUser()
    {
        Debug.Log("user UI opened");
        if (userPanel.activeSelf)
        {
            userPanel.SetActive(false);
        }
        else
        {
            userPanel.SetActive(true);
        }
    }
}