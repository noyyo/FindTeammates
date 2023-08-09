using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.SceneManagement;

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
        DontDestroyOnLoad(gameObject);
    }
    

    public TextMeshProUGUI timeTxt;
    public GameObject endTxt;
    public GameObject card;
    public TextMeshProUGUI stageNum;
    public TextMeshProUGUI matchingTryNum;
    public Member focusedMember;
    public GameObject choosedCard;
    public AudioClip match;
    public AudioSource audioSource;
    public Collocator collocator;

    private float time;
    private bool isWarning;
    private int matchingCount;
    int stage; // 스테이지 변수
    string[] initial = { "KDH", "YJS", "SBE", "JUS" }; //사진 이름 변수

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        timeTxt.text = time.ToString("N2");

        if (time < 10f)
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

    public void StartStage(int stage)
    {
        stageNum.text = stage.ToString();
        cardArr(stage);
        this.time = stage * 20f;
        isWarning = false;
        matchingCount = 0;
        Time.timeScale = 1f;
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


        if (choosedCardInitial == focuesdMemberInitial)
        {
            audioSource.PlayOneShot(match);
            choosedCard.GetComponent<card>().destroyCard();
            focusedMember.anim.SetTrigger("isMatched");
            int cardsLeft = GameObject.Find("cards").transform.childCount;

            if (cardsLeft == 1)
            {
                Invoke("GameEnd", 1f);
            }
        }
        else
        {
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
        endTxt.SetActive(true);
        timeTxt.GetComponent<Animator>().SetBool("isImminent", false);
        audioManager.I.SetPitch(1f);
        isWarning = false;
    }
    public void retryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
        StartStage(stage);
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

}
