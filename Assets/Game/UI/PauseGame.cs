using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [HideInInspector] public bool game_paused;

    void Awake()
    {
        GI.pause_game = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggle_pause_game()
    {
        game_paused = !game_paused;
        if (game_paused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor from the window
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
