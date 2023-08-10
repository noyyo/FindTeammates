using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class openStageBtn : MonoBehaviour
{
    public void StageSelected()
    {
        stageManager.I.stageNum = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1));
        stageManager.I.LoadMainScene();
    }
    
}
