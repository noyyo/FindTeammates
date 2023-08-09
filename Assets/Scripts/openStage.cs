using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class openStage : MonoBehaviour
{
    public void SelectStage1()
    {
        stageManager.stageNum = 1;
        stageManager.call();
    }
    public void SelectStage2()
    {
        stageManager.stageNum = 2;
        stageManager.call();
    }
    public void SelectStage3()
    {
        stageManager.stageNum = 3;
        stageManager.call();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
