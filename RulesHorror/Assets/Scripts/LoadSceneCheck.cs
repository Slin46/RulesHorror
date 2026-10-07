using UnityEngine;

public class LoadSceneCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.sceneChangeCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
