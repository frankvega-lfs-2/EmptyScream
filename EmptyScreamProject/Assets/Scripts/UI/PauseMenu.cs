
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class PauseMenu : MonoBehaviour
{
    public bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public FirstPersonController player;

    /*
     player.GetComponent<FirstPersonController>().m_MouseLook.SetCursorLock(false);
                player.GetComponent<FirstPersonController>().enabled = false;
     */

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuUI.SetActive(false);
        player = GameManager.Get().playerGO.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F2))
        {
            SwitchPause();
        }
    }

    public void SwitchPause()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            player.GetComponent<FirstPersonController>().m_MouseLook.SetCursorLock(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Time.timeScale = 0;
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseMenuUI.SetActive(false);
            player.GetComponent<FirstPersonController>().m_MouseLook.SetCursorLock(true);
            player.GetComponent<FirstPersonController>().enabled = true;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
