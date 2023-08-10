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
    }
    

    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI stage_1;
    public GameObject retryText; //endText를 retryText로 변경
    public GameObject exitText; //추가(SelectScene으로 가는 버튼)
    public GameObject card;
    public GameObject cards;
    public TextMeshProUGUI stageNum;
    public GameObject minusTxt; //마이너스 텍스트 
    public TextMeshProUGUI matchingTryNum;
    public Member focusedMember; //member 클래스
    public GameObject choosedCard;
    public AudioClip match;
    public AudioSource audioSource;
    public Collocator collocator;

    private float time;
    private bool isWarning;
    public TextMeshProUGUI bestScoreNum;
    //중복되어 있는 텍스트 삭제

    private float origintime = 60f; // 초기 시간값을 통해 스코어 변화를 주기 위한 변수(위에 시간 변경 시 같이 변경해 주세요~~)
    private int matchingCount;
    private int stage; // 스테이지 변수
    private int totalscore;
    private int score;
    private string[] initial = { "KDH", "YJS", "SBE", "JUS" }; //사진 이름 변수

    // Start is called before the first frame update
    void Start()
    {
        StartStage();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime; //초기 값에서 시간을 감소
        timeTxt.text = time.ToString("N2"); // 시간을 2자리 표시

        // 시간이 10초 미만일때 효과 및 종료
        if (time < 10f)
        {
            if (isWarning == false)
            {
                WarningRemainingTime();
            }
        }
        if (time < 0f)
        {
            Debug.Log("시간");
            timeTxt.text = "0.00";
            Invoke("GameEnd",0f);
        }
    }

    public void StartStage()
    {

        stage = stageManager.I.stageNum;
        stageNum.text = stage.ToString();
        score = 0;
        totalscore = 0;
        matchingCount = 0;
        time = 60f;
        origintime = time;
        cardArr(stage);
        isWarning = false;
        Time.timeScale = 1f;
    }
    public void addScore(int score)// 스코어 추가
    {
        totalscore += score;
        if (isBestScore(totalscore))
        {
            bestScoreNum.text = totalscore.ToString("D2");
            stageManager.I.SetBestScore(stage, totalscore);
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
        if (focusedMember == null)
        {
            focusedMember = FocusMember;
            focusedMember.anim.SetBool("isFocused", true);
            return;
        }
        if (focusedMember == FocusMember)
        {
            focusedMember.anim.SetBool("isFocused", false);
            focusedMember = null;
            return;
        }
        focusedMember.anim.SetBool("isFocused", false);
        FocusMember.anim.SetBool("isFocused", true);
        focusedMember = FocusMember;
    }
    public void Match()
    {
        string choosedCardInitial = choosedCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name.Substring(0, 3);
        string focuesdMemberInitial = focusedMember.initial;
        // 위에 선택된 카드의 이니셜 == 내가 선택한 카드의 이니셜과 같을 때

        if (choosedCardInitial == focuesdMemberInitial)
        {
            int cardsLeft = cards.transform.childCount;

            audioSource.PlayOneShot(match);
            choosedCard.GetComponent<card>().destroyCard();
            focusedMember.anim.SetTrigger("isMatched");
            
            if (time >= origintime * 5 / 6)
                score = 6;
            else if (time >= origintime * 4 / 6)
                score = 5;
            else if (time >= origintime * 3 / 6)
                score = 4;
            else if (time >= origintime * 2 / 6)
                score = 3;
            else if (time >= origintime * 1 / 6)
                score = 2;
            else
                score = 1;
            addScore(score);
            // 시간별로 스코어를 다르게 추가해줌

            if (cardsLeft == 1)
            {
                Invoke("GameEnd", 0.5f);
            }

        }
        else
        {
            minusTxt.SetActive(true);
            minusTime();
            time -= 2; // 매칭 실패시 시간 감소
            focusedMember.anim.SetTrigger("isFailed");
            choosedCard.GetComponent<card>().closeCard();
        }
        matchingTryNum.text = (++matchingCount).ToString("D2");
        choosedCard = null;
        focusedMember.anim.SetBool("isFocused", false);
        focusedMember = null;
    }
    private void GameEnd()
    {
        Time.timeScale = 0;
        timeTxt.GetComponent<Animator>().SetBool("isImminent", false);
        audioManager.I.SetPitch(1f);
        isWarning = false;
        retryText.SetActive(true);
        exitText.SetActive(true);
    }
    public void retryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
        StartStage();
    }

    private void WarningRemainingTime()
    {
        timeTxt.GetComponent<Animator>().SetBool("isImminent", true);
        audioManager.I.SetPitch(1.2f);
        isWarning = true;
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
        collocator.amountToMake = str.Length;
        int count = 0;
        foreach (GameObject obj in collocator.InstantiatePrefab())
        {
            string imageName = str[count];
            obj.transform.Find("front").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MemberImages/" + imageName);
            count++;
        }
    }

    private bool isBestScore(int score)
    {
        return score > stageManager.I.GetBestScore(stage);
    }
}
