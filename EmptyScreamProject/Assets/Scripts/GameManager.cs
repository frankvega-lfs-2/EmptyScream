using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonobehaviourSingleton<GameManager>
{
    public GameObject playerGO;
    public GameObject freeCamera;
    public bool restart;
    public float restartTime;
    private float restartTimer;
    private UIScreenChanger scene;
    // Start is called before the first frame update
    void Start()
    {
        scene = GetComponent<UIScreenChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        if(restart)
        {
            restartTimer += Time.deltaTime;
            if(restartTimer >= restartTime)
            {
                RestartLevel();
            }
        }

        if (Input.GetKey(KeyCode.F12))
        {
            if(freeCamera)
            {
                freeCamera.SetActive(!freeCamera.activeInHierarchy);
                playerGO.SetActive(!playerGO.activeInHierarchy);
            }
        }
    }

    public void RestartLevel()
    {
        scene.changeScene = true;
        scene.nameOfNewScene = SceneManager.GetActiveScene().name;
        scene.ChangeScreen();
    }
}
