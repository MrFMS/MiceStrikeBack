using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ScoreController : MonoSingleton<ScoreController>
{
    int maxMouseCount;
    public int scorePerMouse = 10;
    public int scorePerTime = 1;
    public int scorePenaltyPerError = 5;

    private int mouseCount = 0;
    int newMouseCount = 0;
    private int errorCount = 0;
    int totalErrorCount = 0;
    private float timeElapsed = 0f;



    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        maxMouseCount = GameController.Instance.nombreSouris;
    }
    private void Update()
    {
        if (GameController.Instance.gameOn)
        {
            timeElapsed += Time.deltaTime;
        }

    }

    public void UpdateMaxMouseCount(int newMaxMouseCount)
    {
        maxMouseCount = newMaxMouseCount;
        errorCount = 0;
    }

    public void MouseCaught()
    {
        newMouseCount++;

        GameController.Instance.ChangeTextCounterMice(maxMouseCount-newMouseCount);

        if (newMouseCount >= maxMouseCount)
        {
            mouseCount += newMouseCount;
            newMouseCount = 0;
            Debug.Log("Manche terminée !");
            //Debug.Log("Gagné !");
            int score = CalculateScore();
            Debug.Log("Score : " + score);
            SoundManager.Instance.MuteMice();
            GameController.Instance.EndGame(true);
            
        }
    }

    public void ReinitiateScore()
    {
        mouseCount = 0;
        newMouseCount = 0;
        errorCount = 0;
        totalErrorCount = 0;
        timeElapsed = 0f;
    }

    public void CatchFail()
    {
        errorCount++;
        totalErrorCount++;

		GameObject[] temp = GameObject.FindGameObjectsWithTag("Miss" + errorCount);
		if (temp.Length > 0) temp[0].GetComponent<TMPro.TMP_Text>().enabled = true;


        SoundManager.Instance.PlaySound("failSound");
        if(errorCount >= 5)
        {
            int score = CalculateScore();
            Debug.Log("Score : " + score);
            if (GameController.Instance.isChallenge)
            {
                Leaderboard.Instance.SubmitScoreChallenge(score);
            }
            else
            {
                Leaderboard.Instance.SubmitScore(score);
            }
   
            score = 0;
            GameController.Instance.EndGame(false);
            Debug.Log("Game Over");
        }
    }


    public int CalculateScore()
    {
        int baseScore = mouseCount * 10;
        int timePenalty = (int)(timeElapsed * 0.1f);
        int failurePenalty = totalErrorCount;
        int totalScore = Mathf.Max(0, baseScore - timePenalty - failurePenalty);
        return totalScore;
    }
}