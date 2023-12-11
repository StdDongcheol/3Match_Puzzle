using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void LoadPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
