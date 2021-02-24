using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static bool isOnePlayerGame = true;

    public Text playerText1;
    public Text playerSelector;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Level1");
        }
    }
}
