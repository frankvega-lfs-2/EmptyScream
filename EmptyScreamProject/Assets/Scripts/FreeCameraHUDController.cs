using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraHUDController : MonoBehaviour
{
    public GameObject[] HUD;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            foreach (GameObject HUDItem in HUD)
            {
                HUDItem.SetActive(!HUDItem.activeInHierarchy);
            }
        }
    }
}
