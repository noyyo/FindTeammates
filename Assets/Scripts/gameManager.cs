using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    private static gameManager i;
    public static gameManager I
        { 
            get
            {
                if (i == null)
                {
                    i = new gameManager();
                }
                return i;
            }
        }

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        if (this != i)
        {
            Destroy(gameObject);
        }
    }

    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI stage_1;
    public GameObject retryText;
    public GameObject exitText;
    public GameObject card;
    public GameObject minusTxt;
    public TextMeshProUGUI matchingTryNum;
    public Member focusedMember;
    public GameObject choosedCard;
    public AudioClip match;
    public AudioSource audioSource;
    public TextMeshProUGUI bestScoreNum;
    public TextMeshProUGUI FinishBestScroeNum;
    public TextMeshProUGUI FinishMatchingTryNum;
    public GameObject endPanel;
    public TextMeshProUGUI scoreTxt;

    private float time = stageManager.stageNum * 20f;
    private float origintime;
    private bool isWarning = false;
    private int[] matchingCount = new int[3];
    private int stage = stageManager.stageNum;
    private int score = 0;
    private string[] initial = { "KDH", "YJS", "SBE", "JUS" };
    private float timeLimit = 5f;

    // Start is called before the first frame update
    void Start()
    {
        origintime = time;
        focusedMember.MemberClicked();
        Time.timeScale = 1;
        stage_1.text = stage.ToString();
        bestScoreNum.text = stageManager.bestScore[stage-1].ToString("D2") ;
        cardArr(stage);
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        timeTxt.text = time.ToString("N2");
        firstCard_TimeLimit();
        if (time < 10f)
        {
            if (isWarning == false)
            {
                timeTxt.GetComponent<Animator>().SetBool("isImminent", true);
                audioManager.I.SetPitch(1.2f);
                isWarning = true;
            }
        }
        if (time < 0f)
        {
            timeTxt.text = "0.00";
            Invoke("GameEnd",0f);
        }
    }
    public void minusTime()
    {
        Invoke("minusTimeInvoke", 1.0f);
    }

    void minusTimeInvoke()
    {
        minusTxt.SetActive(false);
    }
    public void ChangeFocus(Member FocusMember)
    {
        focusedMember.anim.SetBool("isFocused", false);
        FocusMember.anim.SetBool("isFocused", true);
        focusedMember = FocusMember;
        timeLimit = 5f;
    }
    public void Match()
    {
        
        string choosedCardInitial = choosedCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name.Substring(0, 3);
        string focuesdMemberInitial = focusedMember.initial;

        if (choosedCardInitial == focuesdMemberInitial)
        {
            audioSource.PlayOneShot(match);
            choosedCard.GetComponent<card>().destroyCard();
            focusedMember.anim.SetTrigger("isMatched");
            int cardsLeft = GameObject.Find("cards").transform.childCount;

            if (time >= origintime * 5 / 6)
                score += 6;
            else if (time >= origintime * 4 / 6)
                score += 5;
            else if (time >= origintime * 3 / 6)
                score += 4;
            else if (time >= origintime * 2 / 6)
                score += 3;
            else if (time >= origintime * 1 / 6)
                score += 2;
            else
                score += 1;
            bestScoreNum.text = isBestScore(score, stageManager.bestScore[stage - 1]).ToString("D2");

            if (cardsLeft == 1)
            {
                stageManager.bestScore[stage - 1] = isBestScore(score, stageManager.bestScore[stage - 1]);
                Invoke("GameEnd", 1f);
            }
                
        }
        else
        {
            minusTxt.SetActive(true);
            minusTime();
            time -= 2;
            focusedMember.anim.SetTrigger("isFailed");
            choosedCard.GetComponent<card>().closeCard();
        }
        matchingTryNum.text = (++matchingCount[stage-1]).ToString("D2");
        choosedCard = null;
    }
    void GameEnd()
    {
        Time.timeScale = 0;
        endPanelTxt();
    }
    public void retryGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void cardArr(int stage)
    {
        //이미지 종류 (ex : "picture")
        string[] type = new string[stage];
        //이미지 이름 (ex : "KDH_name")
        string[] str = new string[type.Length * initial.Length];

        string[] a = new string[] { "picture", "animal", "game" };

        for (int i = 0; i < stage; i++)
        {
            type[i] = a[i];
        }

        for (int i = 0; i < initial.Length; i++)
        {
            for (int j = 0; j < stage; j++)
            {
                str[i * stage + j] = initial[i] + '_' + type[j];
            }
        }
        str = str.OrderBy(item => Random.Range(-1.0f, 1.0f)).ToArray();
        for (int i = 0; i < str.Length; i++)
        {
            GameObject newCard = Instantiate(card);
            newCard.transform.parent = GameObject.Find("cards").transform;

            float x = (i / 4) * 1.4f - (0.7f * stage);
            float y = (i % 4) * 1.4f - 3.0f;
            newCard.transform.position = new Vector3(x, y, 0);

            string imageName = str[i];
            newCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MemberImages/" + imageName);
        }
    }
    private int isBestScore(int totalscore, int bestScore)
    {
        if (bestScore == 0)
            bestScore = totalscore;
        else if (bestScore < totalscore)
            bestScore = totalscore;
        return bestScore;
    }

    private void firstCard_TimeLimit()
    {
        timeLimit -= Time.deltaTime;
        if (timeLimit <= 0)
        {
            if(focusedMember.initial == "YJS")
                GameObject.FindWithTag("KDH").GetComponent<Member>().MemberClicked();
            else if (focusedMember.initial == "KDH")
                GameObject.FindWithTag("SBE").GetComponent<Member>().MemberClicked();
            else if (focusedMember.initial == "SBE")
                GameObject.FindWithTag("JUS").GetComponent<Member>().MemberClicked();
            else
                GameObject.FindWithTag("YJS").GetComponent<Member>().MemberClicked();
        }
    }
    private void endPanelTxt()
    {
        FinishMatchingTryNum.text = matchingCount[stage - 1].ToString("D2");
        FinishBestScroeNum.text = stageManager.bestScore[stage - 1].ToString("D2");
        scoreTxt.text = score.ToString("D2") + "점";
        endPanel.SetActive(true);
    }
}
