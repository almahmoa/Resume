using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioSource punch1SFX;
    public AudioSource punch2SFX;

    public AudioSource kickSFX;
    public AudioSource projectileSFX;

    public AudioSource ohYeah1;
    public AudioSource ohYeah2;
    public AudioSource ohYeah3;
    public AudioSource ohYeah4;
    public AudioSource ohYeah5;
    public AudioSource ohYeah6;
    public AudioSource ohYeah7;
    public AudioSource ohYeah8;

    public void PunchedSFX()
    {
        int ranNum = Random.Range(0, 2);
        if (ranNum == 1)
        {
            if (!punch1SFX.isPlaying)
            {
                punch1SFX.pitch = Random.Range(.85f, 1);
                punch1SFX.Play();
            }
        }
        else
        {
            if (!punch2SFX.isPlaying)
            {
                punch2SFX.pitch = Random.Range(.95f, 1);
                punch2SFX.Play();
            }
        }
    }

    public void KickedSFX()
    {
        if(!kickSFX.isPlaying)
        {
            kickSFX.pitch = Random.Range(.65f, 1f);
            kickSFX.Play();
        }
    }

    public void ProjectileSFX()
    {
        if (!projectileSFX.isPlaying)
        {
            projectileSFX.pitch = Random.Range(1.5f, 2);
            projectileSFX.Play();
        }
    }

    public void OhYeahSFX()
    {
        int ranNum = Random.Range(0, 9);
        if(ranNum == 1)
        {
            ohYeah1.pitch = Random.Range(.85f, .9f);
            ohYeah1.Play();
        }
        else if (ranNum == 2)
        {
            ohYeah2.pitch = Random.Range(.85f, .9f);
            ohYeah2.Play();
        }
        else if (ranNum == 3)
        {
            ohYeah3.pitch = Random.Range(.85f, .9f);
            ohYeah3.Play();
        }
        else if (ranNum == 4)
        {
            ohYeah4.pitch = Random.Range(.85f, .9f);
            ohYeah4.Play();
        }
        else if (ranNum == 5)
        {
            ohYeah5.pitch = Random.Range(.85f, .9f);
            ohYeah5.Play();
        }
        else if (ranNum == 6)
        {
            ohYeah6.pitch = Random.Range(.85f, .9f);
            ohYeah6.Play();
        }
        else if (ranNum == 7)
        {
            ohYeah7.pitch = Random.Range(.85f, .9f);
            ohYeah7.Play();
        }
        else
        {
            ohYeah8.pitch = Random.Range(.85f, 1);
            ohYeah8.Play();
        }
    }
}
