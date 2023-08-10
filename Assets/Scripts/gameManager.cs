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
    public GameObject retryText; //endText�� retryText�� ����
    public GameObject exitText; //�߰�(SelectScene���� ���� ��ư)
    public GameObject card;
    public GameObject cards;
    public TextMeshProUGUI stageNum;
    public GameObject minusTxt; //���̳ʽ� �ؽ�Ʈ 
    public TextMeshProUGUI matchingTryNum;
    public Member focusedMember; //member Ŭ����
    public GameObject choosedCard;
    public AudioClip match;
    public AudioSource audioSource;
    public Collocator collocator;

    private float time;
    private bool isWarning;
    private bool firstCardClicked;
    private bool isCleared;
    private bool isGameEnded;
    public TextMeshProUGUI bestScoreNum;
    public Member nextMember;

    private float origintime = 60f; // �ʱ� �ð����� ���� ���ھ� ��ȭ�� �ֱ� ���� ����(���� �ð� ���� �� ���� ������ �ּ���~~)
    private int matchingCount;
    private int stage; // �������� ����
    private int totalscore;
    private int score;
    private string[] initial = { "KDH", "YJS", "SBE", "JUS" }; //���� �̸� ����
    private const float TIME_LIMIT = 5f; // ù ��° ī�� ���� �ð�
    private float clickedTime;

    // Start is called before the first frame update
    void Start()
    {
        StartStage();
        //bestScoreNum.text = stageManager.bestScore[stage-1].ToString("D2") ; // �ְ�����
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime; //�ʱ� ������ �ð��� ����
        timeTxt.text = time.ToString("N2"); // �ð��� 2�ڸ� ǥ��
        // �ð��� 10�� �̸��϶� ȿ�� �� ����
        CheckTimeLimit();
        if (time < 10f && !isGameEnded)
        {
            if (isWarning == false)
            {
                WarningRemainingTime();
            }
        }
        if (time < 0f)
        {
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
        time = stage * 20f;
        origintime = time;
        clickedTime = 0f;
        cardArr(stage);
        isWarning = false;
        firstCardClicked = false;
        isCleared = false;
        isGameEnded = false;
        Time.timeScale = 1f;
    }
    public void addScore(int score)// ���ھ� �߰�
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
        firstCardClicked = false;

        string choosedCardInitial = choosedCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name.Substring(0, 3);
        string focuesdMemberInitial = focusedMember.initial;
        // ���� ���õ� ī���� �̴ϼ� == ���� ������ ī���� �̴ϼȰ� ���� ��

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
            // �ð����� ���ھ �ٸ��� �߰�����

            if (cardsLeft == 1)
            {
                isCleared = true;
                Invoke("GameEnd", 0.5f);
            }

        }
        else
        {
            minusTxt.SetActive(true);
            minusTime();
            time -= 2; // ��Ī ���н� �ð� ����
            focusedMember.anim.SetTrigger("isFailed");
            choosedCard.GetComponent<card>().closeCard();
        }
        matchingTryNum.text = (++matchingCount).ToString("D2");
        ChangeFocus(focusedMember);
    }
    private void GameEnd()
    {
        isGameEnded = true;
        Time.timeScale = 0;
        timeTxt.GetComponent<Animator>().SetBool("isImminent", false);
        isWarning = false;
        audioManager.I.SetPitch(1f);
        retryText.SetActive(true);
        exitText.SetActive(true);
        stageManager.I.SetStageClearFlag(stage, isCleared);
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
        //�̹��� ���� (ex : "picture")
        string[] type = new string[stage];
        //�̹��� �̸� (ex : "KDH_name")
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
    public void CardClicked()
    {
        if (focusedMember != null) return;
        if (choosedCard != null) return;

        firstCardClicked = true;
        clickedTime = time;
    }
    public void CheckTimeLimit()
    {
        if (!firstCardClicked) return;
        if (time < clickedTime - TIME_LIMIT)
        {
            firstCardClicked = false;
            if (focusedMember != null)
            {
                ChangeFocus(focusedMember);
            }
            else if (choosedCard != null)
            {
                choosedCard.GetComponent<card>().closeCard(0f);
            }
        }
    }
}
