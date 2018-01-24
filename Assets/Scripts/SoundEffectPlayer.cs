using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour {


    [SerializeField]
    private AudioSource successSound;

    [SerializeField]
    private AudioSource failSound;

    public void PlaySuccessSound()
    {
        successSound.PlayOneShot(successSound.clip);
    }

    public void PlayFailSound()
    {
        failSound.PlayOneShot(failSound.clip);
    }
}
