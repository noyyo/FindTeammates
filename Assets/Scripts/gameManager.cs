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
    public GameObject firstCard;
    public GameObject secondCard;
    public AudioClip match;
    public AudioSource audioSource;


    float time;
    int stage = stageManager.stageNum; // 스테이지 변수
    string[] initial = { "KDH", "YJS", "SBE", "JUS" }; //사진 이름 변수
    int[] matchingTry = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        variableReset();
        stage_1.text = stage.ToString();
        bestScore_1.text = stageManager.bestScore[stage-1].ToString("D2") ;
        cardArr(stage);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        timeTxt.text = time.ToString("N2");

        if (time > 30f)
        {
            Invoke("GameEnd",0f);
        }
    }

    public void isMatched()
    {
        string firstCardImage = firstCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name;
        string secondCardImage = secondCard.transform.Find("front").GetComponent<SpriteRenderer>().sprite.name;

        // '_'이후 문자 저장
        string firstCardType = firstCardImage.Substring(firstCardImage.IndexOf('_') + 1);
        string secondCardType = secondCardImage.Substring(secondCardImage.IndexOf('_') + 1);

        // '_'이후 문자 제거로 이니셜 비교
        firstCardImage = firstCardImage.Substring(0, firstCardImage.LastIndexOf("_"));
        secondCardImage = secondCardImage.Substring(0, secondCardImage.LastIndexOf("_"));

        //매칭 시도와 최고 점수
        matchingTry[stage-1]++;
        matchingTry_1.text = matchingTry[stage-1].ToString("D2");

        if (firstCardImage == secondCardImage)
        {
            audioSource.PlayOneShot(match);

            // 이미지 타입이 이름인지 구별하여 이름이 아닐시 삭제
            isName(firstCardType, secondCardType);

            int cardsLeft = GameObject.Find("cards").transform.childCount;

            //카드수 확인 후 종료
            isLastCards(cardsLeft, firstCardType, secondCardType);

        }
        else
        {
            firstCard.GetComponent<card>().closeCard();
            secondCard.GetComponent<card>().closeCard();
        }

        firstCard = null;
        secondCard = null;
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
        //이미지 종류 (ex : "name")
        string[] type = new string[stage + 1];
        //이미지 이름 (ex : "KDH_name")
        string[] str = new string[type.Length * initial.Length];

        string[] a = new string[] { "name", "picture", "animal", "game" };

        for (int i = 0; i < stage + 1; i++)
        {
            type[i] = a[i];
        }

        for (int i = 0; i < initial.Length; i++)
        {
            for (int j = 0; j < stage + 1; j++)
            {
                str[i * (stage + 1) + j] = initial[i] + '_' + type[j];
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


    private void isName(string firstCardType, string secondCardType)
    {
        if (firstCardType == "name")
        {
            firstCard.GetComponent<card>().closeCard();
            secondCard.GetComponent<card>().destroyCard();
        }
        else if (secondCardType == "name")
        {
            firstCard.GetComponent<card>().destroyCard();
            secondCard.GetComponent<card>().closeCard();
        }
        else
        {
            firstCard.GetComponent<card>().destroyCard();
            secondCard.GetComponent<card>().destroyCard();
        }
    }

    private void isLastCards(int cardsLeft, string firstCardType, string secondCardType)
    {
        if (cardsLeft == 6)
        {
            if (firstCardType != "name" && secondCardType != "name")
            {
                stageManager.bestScore[stage -1] = isBestScore(matchingTry[stage -1], stageManager.bestScore[stage -1]);
                Debug.Log("bestScore[" + (stage - 1) + "] == " + stageManager.bestScore[stage - 1]);
                bestScore_1.text = stageManager.bestScore[stage-1].ToString("D2");
                Invoke("GameEnd", 1f);
            }
        }
        else if (cardsLeft <= 5)
        {
            stageManager.bestScore[stage - 1] = isBestScore(matchingTry[stage - 1], stageManager.bestScore[stage - 1]);
            Debug.Log("bestScore["+ (stage - 1) + "] == " + stageManager.bestScore[stage - 1]);
            bestScore_1.text = stageManager.bestScore[stage - 1].ToString("D2");
            Invoke("GameEnd", 1f);
        }
    }

    private int isBestScore(int matchingTry, int bestSore)
    {
        if (bestSore == 0)
            bestSore = matchingTry;
        else if(bestSore > matchingTry) 
            bestSore = matchingTry;
        return bestSore;
    }

    private void variableReset()
    {
        Time.timeScale = 1;
    }

}
