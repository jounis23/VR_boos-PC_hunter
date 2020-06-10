using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    public AudioClip[] audioClip; // 오디오 소스들 지정.

    Dictionary<string, AudioClip> audioClipsDic;
    public AudioSource sfxPlayer;
    public AudioSource bgmPlayer;

    public int phase = -1;

    void AwakeAfter()
    {
        sfxPlayer = GetComponent<AudioSource>();

        audioClipsDic = new Dictionary<string, AudioClip>();
        foreach (AudioClip a in audioClip)
        {
            audioClipsDic.Add(a.name, a);
        }
    }


    public void ChangePhase(int phase)
    {
        this.phase = phase;
        if (this.phase >= 0)
        {
            bgmPlayer.clip = audioClip[this.phase];
            bgmPlayer.Play();
        }

    }


    private void Start()
    {
        if(bgmPlayer != null)
        	bgmPlayer.Play();
    }

    // 한 번 재생 : 볼륨 매개변수로 지정
    public void PlaySound(string a_name, float a_volume = 1f)
    {
        if (audioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipsDic[a_name], a_volume * masterVolumeSFX);
    }

    // 삭제할때는 리턴값은 GameObject를 참조해서 삭제한다. 나중에 옵션에서 사운드 크기 조정하면 이건 같이 참조해서 바뀌어야함..
    public GameObject PlayLoopSound(string a_name)
    {
        GameObject otherSound = GameObject.FindWithTag("sound");
        if (otherSound)
        {
            Destroy(otherSound.gameObject);
        }
        if (audioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return null;
        }
        

        GameObject l_obj = new GameObject("LoopSound");
        l_obj.tag = "sound";
        AudioSource source = l_obj.AddComponent<AudioSource>();
        source.clip = audioClipsDic[a_name];
        source.volume = masterVolumeSFX;
        source.loop = true;
        source.Play();
        return l_obj;
    }

    // 주로 전투 종료시 음악을 끈다.
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    #region 옵션에서 볼륨조절
    public void SetVolumeSFX(float a_volume)
    {
        masterVolumeSFX = a_volume;
    }

    public void SetVolumeBGM(float a_volume)
    {
        masterVolumeBGM = a_volume;
        bgmPlayer.volume = masterVolumeBGM;
    }
    #endregion
    
}
