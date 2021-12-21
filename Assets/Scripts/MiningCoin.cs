using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MiningCoin : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinNumberText;
    public TextMeshProUGUI playerCoinsText;
    public float playerCoins;
    int coinAmountToAdd = 5;
    float totalCoinsInProcess = 0;
    float finalTime = 28800f;
    float mhour = 0;
    float mmin = 1;
    float msec = 0;
    float startTime;
    float elapsedTime;

    private int hours;
    private int minutes;
    private int seconds;

    private List<int> timeValues = new List<int>();


    private int timetoDelete;


    // Start is called before the first frame update
    void Start()
    {
       //PlayerPrefs.DeleteAll();

        if (PlayerPrefs.HasKey("DateTime") && PlayerPrefs.HasKey("MiningTime"))
        {
            Debug.Log("ilk if");
            string[] getTimes = PlayerPrefs.GetString("MiningTime").Split(',');

            for (int i = 0; i < getTimes.Length; i++)
            {
                timeValues.Add(int.Parse(getTimes[i]));
            }

            var exitDates = DateTime.Parse(PlayerPrefs.GetString("DateTime"));
            var enteredDates = DateTime.Now;
            var resultDates = enteredDates - exitDates;
            Debug.Log(resultDates);

            int seconds = resultDates.Seconds + timeValues[2];
            int minutes = (resultDates.Minutes + timeValues[1]) * 60;
            int hours = (resultDates.Hours + timeValues[0]) * 3600;
            elapsedTime = seconds + minutes + hours;
            timetoDelete = (int)elapsedTime % coinAmountToAdd;

            Debug.Log(" timetodelete : " + timetoDelete);
            Debug.Log("elapsed time: " + elapsedTime);
        }

        if (PlayerPrefs.HasKey("TotalCoinsInProcess"))
        {
            totalCoinsInProcess = PlayerPrefs.GetFloat("TotalCoinsInProcess");
            if(elapsedTime <= finalTime)
            {
                int coinAmount = (int)elapsedTime / coinAmountToAdd;
                totalCoinsInProcess = coinAmount * coinAmountToAdd;
            }
            else
            {
                totalCoinsInProcess = 28800; //max coin that can be mined. for 8 hours.
            }
                

        }
        else
        {
            totalCoinsInProcess = 0;
        }

       

        //PlayerPrefs.DeleteKey("PlayerCoins");
        StartCoroutine(IncreaseTimer());
        StartCoroutine(IncreaseCoinAmount());
        //timerText.text = "";
        //coinNumberText.text = "0";
        coinNumberText.text = totalCoinsInProcess.ToString();
        startTime = 0;
        if (PlayerPrefs.HasKey("PlayerCoins"))
        {
            playerCoins = PlayerPrefs.GetFloat("PlayerCoins");
        }
        else
        {
            playerCoins = 0;
        }
        Debug.Log("player has " + PlayerPrefs.GetFloat("PlayerCoins") + " coins");
        playerCoinsText.text = playerCoins.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetString("DateTime", DateTime.Now.ToString());
        PlayerPrefs.SetString("MiningTime", hours.ToString() + "," + minutes.ToString() + "," + seconds.ToString());


    }


    IEnumerator IncreaseTimer()
    {
        Time.timeScale = 1f;
        elapsedTime += Time.deltaTime + startTime;

        hours = (int)((0 + elapsedTime) / 3600) % 24;
        minutes = (int)((0 + elapsedTime) / 60) % 60;
        seconds = (int)((0 + elapsedTime) % 60);

        //if (seconds >= 20) // added
        //{
        //    // to do: make it for 2 min first, then 1 hour- then 5 hour, and watch.
        //    seconds = 20;
        //}

        if(mhour == hours && mmin == minutes && msec == seconds)
        {
            // to do: fix it. 
            seconds = (int)msec;
            minutes = (int)mmin;
            hours = (int)mhour;
        }


        string gameTimerString = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        Debug.Log(gameTimerString);

        if(elapsedTime <= finalTime)
        {
            timerText.text = gameTimerString;
        }
        else
        {
            timerText.text = "08:00:00";
        }

        if (elapsedTime > finalTime)
        {
            // Times up
            StopCoroutine(IncreaseTimer());
            StopCoroutine(IncreaseCoinAmount());
            Debug.Log("stop");
        }

        yield return new WaitForSeconds(0f);

        if (elapsedTime < finalTime)
        {
            //Debug.Log(elapsedTime);
            StartCoroutine(IncreaseTimer());
        }

    }

    bool firstOpening = true;
    private float waitTimeToAddCoin = 5f;
    IEnumerator IncreaseCoinAmount()
    {
        if(firstOpening == true)
        {
            firstOpening = false;
            waitTimeToAddCoin = waitTimeToAddCoin - timetoDelete;
        }
        else
        {
            waitTimeToAddCoin = 5f;
        }

        yield return new WaitForSeconds(waitTimeToAddCoin);
        //totalCoinsInProcess += coinAmountToAdd;
        //PlayerPrefs.SetFloat("TotalCoinsInProcess", totalCoinsInProcess);
        //coinNumberText.text = totalCoinsInProcess.ToString();

        if (elapsedTime <= finalTime + 1) // +1 for making sure it adds the last mining coin.
        {
            totalCoinsInProcess += coinAmountToAdd;
            PlayerPrefs.SetFloat("TotalCoinsInProcess", totalCoinsInProcess);
            coinNumberText.text = totalCoinsInProcess.ToString();
            StartCoroutine(IncreaseCoinAmount());
        }
        else
        {
            StopCoroutine(IncreaseCoinAmount());
        }


    }

    public void ClaimReward()
    {
        StopAllCoroutines();
        playerCoins += totalCoinsInProcess;
        PlayerPrefs.SetFloat("PlayerCoins", playerCoins);
        playerCoinsText.text = playerCoins.ToString();
        Debug.Log("player has " + PlayerPrefs.GetFloat("PlayerCoins") + " coins");
        //coinNumberText.text = "0";
        totalCoinsInProcess = 0;
        coinNumberText.text = totalCoinsInProcess.ToString();
        PlayerPrefs.SetFloat("TotalCoinsInProcess", totalCoinsInProcess);
        elapsedTime = 0;
        StartCoroutine(IncreaseTimer());
        StartCoroutine(IncreaseCoinAmount());
    }

}
