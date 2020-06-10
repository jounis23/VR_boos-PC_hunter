using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenualButton : MonoBehaviour
{
    public GameObject[] menualImage;
    public void OnBossButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                menualImage[i].SetActive(true);
                continue;
            }
            menualImage[i].SetActive(false);
        }

    }
    public void OnArcherButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 1)
            {
                menualImage[i].SetActive(true);
                continue;
            }
            menualImage[i].SetActive(false);
        }
    }
    public void OnBerserkerButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
            {
                menualImage[i].SetActive(true);
                continue;
            }
            menualImage[i].SetActive(false);
        }
    }
    public void OnKnightButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 3)
            {
                menualImage[i].SetActive(true);
                continue;
            }
            menualImage[i].SetActive(false);
        }
    }
    public void OnSorceressButton()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 4)
            {
                menualImage[i].SetActive(true);
                continue;
            }
            menualImage[i].SetActive(false);
        }
    }
}
