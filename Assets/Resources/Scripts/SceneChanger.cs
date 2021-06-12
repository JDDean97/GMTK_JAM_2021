using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger: MonoBehaviour
{
    public void load(int sceneNumber)
    {
        GameObject loader = Resources.Load<GameObject>("Prefabs/LoadScreen");
        GameObject.Instantiate(loader);
        SceneManager.LoadSceneAsync(sceneNumber);
    }

    public void quit()
    {
        Application.Quit();
    }
}
