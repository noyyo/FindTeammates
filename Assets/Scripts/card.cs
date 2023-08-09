using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class card : MonoBehaviour
{
    public Animator anim;
    public AudioClip flip;
    public AudioSource audioSource;
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
        n = Random.Range(0, 3);
        audioSource.PlayOneShot(flip);
        if (n == 0)
            anim.SetBool("isOpen", true);
        else if (n == 1)
            anim.SetBool("isOpen2", true);
        else
            anim.SetBool("isOpen3", true);
        transform.Find("front").gameObject.SetActive(true);
        transform.Find("back").gameObject.SetActive(false);
        gameManager.I.choosedCard = gameObject;
        gameManager.I.Match();
    }

    public void destroyCard()
    {
        Invoke("destroyCardInvoke", 1.0f);
    }

    void destroyCardInvoke()
    {
        Destroy(gameObject);
    }

    public void closeCard(float delay = 1f)
    {
        Invoke("closeCardInvoke", delay);
    }

    void closeCardInvoke()
    {
        if (n == 0)
            anim.SetBool("isOpen", false);
        else if (n == 1)
            anim.SetBool("isOpen2", false);
        else
            anim.SetBool("isOpen3", false);
        transform.Find("back").gameObject.SetActive(true);
        transform.Find("front").gameObject.SetActive(false);
    }
}
