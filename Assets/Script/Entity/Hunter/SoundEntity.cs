using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundEntity : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip attackedClip;
    public AudioClip attackClip;
    public AudioClip[] clip;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void AttackedSound(float delay = 0)
    {
        if (attackedClip == null)
            return;

        if (delay == 0)
            audioSource.PlayOneShot(attackedClip);
        else
            StartCoroutine(SoundDelay(attackedClip, delay));
    }


    public void AttackSound(float delay = 0)
    {
        if (attackClip == null)
            return;

        if (delay == 0)
            audioSource.PlayOneShot(attackClip);
        else
            StartCoroutine(SoundDelay(attackClip, delay));
    }



    public void ClipSound(int number, float delay = 0)
    {
        if (number >= clip.Length || clip[number] == null)
            return;

        if (delay == 0)
            audioSource.PlayOneShot(clip[number]);
        else
            StartCoroutine(SoundDelay(clip[number], delay));
    }


    IEnumerator SoundDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);

    }


}
