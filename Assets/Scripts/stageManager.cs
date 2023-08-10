using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class stageManager : MonoBehaviour
{
    private const int STAGE_COUNT = 3;
    public int StageCount { get { return STAGE_COUNT; } }
    private static stageManager i;
    public static stageManager I
    {
        get
        {
            if (i == null)
                i = new stageManager();
            return i;
        }
    }

    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        if (this != i)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Init();

    }
    public Canvas stageUI;
    public int stageNum;
    private int[] bestScores;
    private bool[] stageClearFlags;
    private bool sceneLoaded;

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
    void Start()
    {
    }
    private void Init()
    {
        bestScores = new int[STAGE_COUNT];
        stageClearFlags = new bool[STAGE_COUNT];
    }

public void SetBestScoreTxt()
    {
        if (SceneManager.GetActiveScene().name != "SelectScene")
            return;
        for (int i = 1; i <= STAGE_COUNT; ++i)
        {
            stageUI.transform.GetChild(i - 1).transform.Find($"bestScore{i}").gameObject.GetComponent<TextMeshProUGUI>().text = bestScores[i - 1].ToString("D2");
        }
    }
    public void SetStageClearFlag(int stageNum, bool isCleared)
    {
        stageClearFlags[stageNum - 1] = isCleared;
    }
    public int GetBestScore(int stageNum)
    {
        if (stageNum <= STAGE_COUNT && stageNum >= 1) return bestScores[stageNum - 1];
        else return 0;
    }
    public void SetBestScore(int stageNum, int bestScore)
    {
        bestScores[stageNum - 1] = bestScore;
    }
    public void UpdateStageInfos()
    {
        stageUI = FindObjectOfType<Canvas>();
        for (int i = 1; i <= STAGE_COUNT; ++i)
        {
            if (i > 1)
            {
                stageUI.gameObject.transform.GetChild(i - 1).transform.Find($"openStage{i}").gameObject.SetActive(stageClearFlags[i - 2]);
            }
            if (i > 1 && i <= STAGE_COUNT)
            {
                stageUI.gameObject.transform.GetChild(i - 1).transform.Find($"lockStage{i}").gameObject.SetActive(!stageClearFlags[i - 2]);
            }
        }
        SetBestScoreTxt();
    }
}
