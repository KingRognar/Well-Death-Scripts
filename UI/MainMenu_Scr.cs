using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Scr : MonoBehaviour
{
    public static bool exitMenu = false;

    public void StartGameBtn()
    {
        exitMenu = true;
        SceneManager.LoadScene("SampleScene");
    }
    public void ArchivesBtn()
    {

    }
    public void SettingsBtn()
    {

    }
    public void ExitBtn()
    {
        exitMenu = true;
        Application.Quit();
    }
}
