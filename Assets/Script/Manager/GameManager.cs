using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using UnityEngine.UI;
using Photon.Pun.Demo.Asteroids;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public SoundManager soundManager;

    public int phase = -1;

    // 자기가 Hunter일때
    public Hunter player;

    // 팀원 Hunter
    public List<Hunter> hunters;

    // 틴원 스테이터스
    private string[] teamClass;
    private float[] teamHp;
    private float[] teamMp;

    // 보스
    public Boss boss;
    private float bossHp;

    private bool isInit = false;


    // 총 플레이 시간
    public float playTime = 0;


    // 팀원 스테이터스 Update시간
    private float statusTime =0;

    // 자신이 죽었을때 다른 플레이어를 보도록 하는 플레이어 번호
    private int cameraNumber;


    private void Awake()
    {
        hunters = new List<Hunter>();

    }

    private void Start()
    {

    }




    public void Init()
    {
        StartCoroutine(InitDelay());
    }

    IEnumerator InitDelay()
    {
        while (true)
        {
            hunters.Clear();
            yield return new WaitForSeconds(0.1f);

            if (boss == null)
                boss = FindObjectOfType<Boss>();

            Debug.Log("boss : "+boss);
            if (boss == null)
                continue;


            hunters = FindObjectsOfType<Hunter>().ToList();
            Debug.Log("hunters : " + hunters.Count);
            if (hunters.Count <= 0)
                continue;

            break;


        }

        foreach (Hunter p in hunters)
        {
            if (p.photonView.IsMine)
            {
                player = p;
                hunters.Remove(p);
                break;
            }
        }

        InitUIData();

        phase = 0;
        soundManager.ChangePhase(phase);
        isInit = true;

        player.Initialized();

        /*

        yield return new WaitForSeconds(2.0f);

        boss = FindObjectOfType<Boss>();
        foreach (Hunter p in FindObjectsOfType<Hunter>())
        {
            if (p.photonView.IsMine)
            {
                player = p;
            }
            else
                hunters.Add(p);
        }

        InitUIData();

        phase = 0;
        soundManager.ChangePhase(phase);
        isInit = true;
        */
    }

    private void InitUIData()
    {
        teamHp = new float[hunters.Count];
        teamMp = new float[hunters.Count];
        teamClass = new string[hunters.Count];
        for (int i = 0; i < hunters.Count; i++)
        {
            teamClass[i] = hunters[i].classEntity.className.ToString();
        }

        if(player != null)
            player.UpdateTeamStatusUIClass(teamClass);
    }


    // Hunter와 Boss 체력 업데이트는 0.1초마다 실행
    //  피격 될때 실행되고 여러명이 한번에 다량으로 피격 되면 오버헤드가 심할 것으로 생각되어
    //  일정 시간마다 실행 하도록 수정
    void Update()
    {
        if (isInit)
        {
            playTime += Time.deltaTime;

            // 플레이어가 살아 있을 때
            if(player != null)
            {

                statusTime += Time.deltaTime;
                if (statusTime > 0.1f)
                {
                    for (int i = 0; i < hunters.Count; i++)
                    {
                        teamHp[i] = hunters[i].status.hp / hunters[i].status.hpMax;
                        teamMp[i] = hunters[i].status.mp / hunters[i].status.mpMax;
                        teamClass[i] = hunters[i].classEntity.className.ToString();
                    }

                    if (boss != null)
                        bossHp = boss.status.hp / boss.status.hpMax;
                    else
                        bossHp = 0.5f;

                    player.UpdateTeamStatusUI(teamHp, teamMp, bossHp);

                    statusTime = 0;
                }



                if (boss != null && boss.state == STATE.DIE && boss.phase < 3)
                {
                    boss.status.hpMax *= 1.5f;
                    boss.status.hp = boss.status.hpMax;
                    boss.phase++;
                }


            }

            // 플레이어가 죽어 있을 때
            else
            {
                if (Input.GetKeyDown(KeyCode.Space)){
                    cameraNumber++;

                    if (cameraNumber >= hunters.Count)
                        cameraNumber = 0;
                    else if (cameraNumber < 0)
                        cameraNumber = hunters.Count - 1;

                    for(int i = 0; i < hunters.Count; i++)
                    {
                        if( i == cameraNumber)
                            hunters[i].mainCam.SetActive(true);
                        else
                            hunters[i].mainCam.SetActive(false);
                    }
                }
            }

        }
    }

    public GameObject gameEndPanel;
    public Image[] stackDamageBar;
    float[] stackDamageBarFillAmount;

    public void GameEnd(bool isWin)
    {
        gameEndPanel.SetActive(true);
        for(int i=0; i<4; i++)
        {
            stackDamageBarFillAmount[i] = boss.stackDamage[i] / boss.status.hpMax;
        }
        StartCoroutine(SetStackDamageBar());
    }
    
    IEnumerator SetStackDamageBar()
    {
        bool[] isfillAmoundEnd = { false, false, false, false };
        while (true)
        {
            for(int i=0; i<4; i++)
            {
                if (stackDamageBar[i].fillAmount < stackDamageBarFillAmount[i])
                    stackDamageBar[i].fillAmount += 0.02f;
                else
                    isfillAmoundEnd[i] = true;
            }

            yield return null;
            if(isfillAmoundEnd[0] && isfillAmoundEnd[1] && isfillAmoundEnd[2] && isfillAmoundEnd[3])
                break;
        }
        yield return null;
    }
}
