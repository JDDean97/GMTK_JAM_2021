using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    float coins = 0;
    public float ropeLength = 20;
    float ropeSlide = 2;
    int flags;
    GameObject pauseMenu;
    float ropeMin = 0.1f;
    float ropeMax = 25;
    float transitionSpeed = 35;
    public List<GameObject> masterTetherPoints;
    // Start is called before the first frame update
    void Start()
    {
        masterTetherPoints = new List<GameObject>();
        Application.targetFrameRate = 60;
        Time.timeScale = 0;
        flags = GameObject.FindGameObjectsWithTag("Flag").Length;
        pauseMenu = FindObjectOfType<Canvas>().transform.Find("Pause").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        cleanTethers();
        //Debug.Log("length: " + ropeLength);
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

        //if(ropeLength>ropeMax)
        //{
        //    Mathf.Lerp(ropeLength, ropeMax, transitionSpeed * Time.deltaTime);
        //}
        //else if(ropeLength < ropeMin)
        //{
        //    Mathf.Lerp(ropeLength, ropeMin, transitionSpeed * Time.deltaTime);
        //}
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        if (Vector2.Distance(players[0].transform.position, players[1].transform.position)>ropeLength)
        {
            if (players[0].GetComponent<PlayerControl>().isGrounded() && players[1].GetComponent<PlayerControl>().isGrounded())
            {
                ropeLength = Vector2.Distance(players[0].transform.position, players[1].transform.position) + 1;
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
        //ropeLength = Mathf.Clamp(ropeLength, 0.1f, 25);
    }

    public void coinCollect(Vector3 spot)
    {
        coins += 1;
        Instantiate(Resources.Load<GameObject>("Prefabs/Sprinkle"),spot,Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/coinSound"), spot, Quaternion.identity);
    }

    public void flagCollect(Vector3 spot)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/flagSprinkle"), spot, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/flagSound"), spot, Quaternion.identity);
        flags -= 1;
        if(flags<=0)
        {
            victory();
        }
    }

    void cleanTethers()
    {
        PlayerControl[] pcs = FindObjectsOfType<PlayerControl>();
        foreach(PlayerControl p in pcs)
        {
            if (masterTetherPoints.Contains(p.gameObject))
            {
                masterTetherPoints.Remove(p.gameObject);
            }
        }
    }

    void victory()
    {
        FindObjectOfType<Canvas>().transform.Find("Victory").gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void lengthenRope()
    {
        ropeLength -= ropeSlide * Time.deltaTime;
        //ropeLength = Mathf.Clamp(ropeLength, 0.1f, 25);
    }
}
