using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class card : MonoBehaviour
{
    public Animator anim;
    public AudioClip flip;
    public AudioSource audioSource;
    public int flipCount;
    public bool isFliped;

    int n;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openCard()
    {
        gameManager.I.CardClicked();
        if (gameManager.I.choosedCard != null)
        {
            anim.SetTrigger("alreadyChoosed");
            gameManager.I.choosedCard.GetComponent<card>().closeCard(0.5f);
            
            gameManager.I.choosedCard = null;
            return;
        }
        n = Random.Range(0, 4);
        audioSource.PlayOneShot(flip);
        if (n == 0)
            anim.SetBool("isOpen", true);
        else if (n == 1)
            anim.SetBool("isOpen2", true);
        else if(n == 2)
            anim.SetBool("isOpen3", true);
        else
            anim.SetBool("isOpen4", true);
        transform.Find("front").gameObject.SetActive(true);
        transform.Find("back").gameObject.SetActive(false);
        gameManager.I.choosedCard = gameObject;
        flipCount++;
        ChangeFlipedCardColor(gameObject.transform.Find("back").GetComponent<SpriteRenderer>());

        if (gameManager.I.focusedMember != null)
        {
            gameManager.I.Match();
        }
    }

    public void destroyCard()
    {
        gameManager.I.choosedCard = null;
        Invoke("destroyCardInvoke", 0.5f);
    }

    void destroyCardInvoke()
    {
        Destroy(gameObject);
    }

    public void closeCard(float delay = 0.5f)
    {
        gameManager.I.choosedCard = null;
        Invoke("closeCardInvoke", delay);
    }

    void closeCardInvoke()
    {
        if (n == 0)
            anim.SetBool("isOpen", false);
        else if (n == 1)
            anim.SetBool("isOpen2", false);
        else if(n == 2)
            anim.SetBool("isOpen3", false);
        else
            anim.SetBool("isOpen4", false);
        transform.Find("back").gameObject.SetActive(true);
        transform.Find("front").gameObject.SetActive(false);
    }
    public void ChangeFlipedCardColor(SpriteRenderer cardRenderer)
    {
        if (cardRenderer.color.r > 100f / 255f)
        {
            cardRenderer.color -= new Color(20f / 255f, 20f / 255f, 20f / 255f, 0f);
        }
    }
}
