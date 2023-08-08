using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Member : MonoBehaviour
{
    public string initial;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        initial = gameObject.name.Substring(gameObject.name.Length - 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MemberClicked()
    {
        gameManager.I.ChangeFocus(this);
    }
}
