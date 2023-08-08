using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    private static audioManager i;
    public static audioManager I
    {
        get
        {
            if (i == null)
            {
                i = new audioManager();
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

    public AudioClip bgMusic;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = bgMusic;
        audioSource.Play();
    }

    public void SetPitch(float ratio)
    {
        audioSource.pitch = ratio;
    }
}
