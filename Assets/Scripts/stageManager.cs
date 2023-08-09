using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class stageManager : MonoBehaviour
{
    public static int stageNum=1;
    public static int[] bestScore = new int[3];
    public TextMeshProUGUI bestScore_1;
    public TextMeshProUGUI bestScore_2;
    public TextMeshProUGUI bestScore_3;
    public GameObject lockStage_2;
    public GameObject openStage_2;
    public GameObject lockStage_3;
    public GameObject openStage_3;

    public static void call()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Start is called before the first frame update
    void Start()
    {
        bestScore_1.text = bestScore[0].ToString("D2");
        bestScore_2.text = bestScore[1].ToString("D2");
        bestScore_3.text = bestScore[2].ToString("D2");
        checkStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void checkStage()
    {
        if (bestScore[0] > 0)
        {
            openStage_2.SetActive(true);
            lockStage_2.SetActive(false);
        }
        if (bestScore[1] > 0)
        {
            openStage_3.SetActive(true);
            lockStage_3.SetActive(false);
        }
    }
}
