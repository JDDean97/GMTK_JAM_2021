using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    public float ropeLength = 20;
    float ropeSlide = 2;
    int flags;
    GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 0;
        flags = GameObject.FindGameObjectsWithTag("Flag").Length;
        pauseMenu = FindObjectOfType<Canvas>().transform.Find("Pause").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("length: " + ropeLength);
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseMenu.activeInHierarchy)
            {
                unPause();
            }
            else
            {
                pause();
            }
            
        }
    }

    public void pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void unPause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void shortenRope()
    {
        ropeLength += ropeSlide * Time.deltaTime;
        ropeLength = Mathf.Clamp(ropeLength, 0, 25);
    }

    public void flagCollect()
    {
        flags -= 1;
        if(flags<=0)
        {
            victory();
        }
    }

    void victory()
    {
        Time.timeScale = 0;
    }

    public void lengthenRope()
    {
        ropeLength -= ropeSlide * Time.deltaTime;
        ropeLength = Mathf.Clamp(ropeLength, 0.1f, 25);
    }
}
