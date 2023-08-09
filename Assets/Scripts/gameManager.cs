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

    public TextMeshProUGUI matchingTry_1;
    public TextMeshProUGUI bestScore_1;
    public TextMeshProUGUI stage_1;

    public GameObject endTxt;
    public GameObject card;
    public TextMeshProUGUI matchingTryNum;
    public Member focusedMember;
    public GameObject choosedCard;
    public AudioClip match;
    public AudioSource audioSource;

    private float time = 20f;
    private bool isWarning = false;
    private int matchingCount = 0;
    int stage = stageManager.stageNum; // 스테이지 변수
    string[] initial = { "KDH", "YJS", "SBE", "JUS" }; //사진 이름 변수
    int[] matchingTry = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        focusedMember.MemberClicked();
        timeScoreReset();
        stage_1.text = stage.ToString();
        bestScore_1.text = stageManager.bestScore[stage-1].ToString("D2") ;
        cardArr(stage);
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
    public void ChangeFocus(Member FocusMember)
    {
        focusedMember.anim.SetBool("isFocused", false);
        FocusMember.anim.SetBool("isFocused", true);
        focusedMember = FocusMember;

    }
    public void Match()
    {

        string choosedCardInitial = choosedCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name.Substring(0, 3);
        string focuesdMemberInitial = focusedMember.name;

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
    }
    void GameEnd()
    {
        Time.timeScale = 0;
        endTxt.SetActive(true);
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
}
