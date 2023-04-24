using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameController : MonoSingleton<GameController>
{
    public int nombreSouris = 10;

    int numberTrap = 0;

    public GameObject sourisPrefab;
    public bool hunt = false;

    public float timeBeforeStart = 60f;
    float timer = 0f;

    public List<Souris> sourisList = new List<Souris>();

    string sceneName;

    public bool gameOn = false;

    public bool isChallenge = false;

    private TMPro.TMP_Text miceCounterText;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        Debug.Log("Awake");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (hunt == false  && sourisList.Count > 0 && gameOn == true)
        {
            gameOn = true;
            timer += Time.deltaTime;
            if (timer >= timeBeforeStart)
            {
                hunt = true;
                MusicManager.Instance.PlayHuntMusic();
                if (sceneName != "DayOne")
                {
                    LightController.Instance.LightOff();
                }
                TrapSelectionMenu.Instance.DesactiveManager();
                GameObject.FindGameObjectWithTag("MenuTrap").SetActive(false);
				GameObject.FindGameObjectWithTag("MenuCatch")?.GetComponent<ShowTriesTimer>().OpenMenu();
				SoundManager.Instance.ResumeMice();
				SpawnSouris();
			}
			TrapSelectionMenu.Instance.DisplayTime(timeBeforeStart - timer);
		}
        else {
            gameOn = false;
        }

    }

    public void GameOn()
    {
        gameOn = true;

    }

    public void LaunchRandom()
    {
        SpawnSourisRandom();
        GameObject.FindGameObjectWithTag("MenuTrap").GetComponentInChildren<TrapMenu>().GenerateRandomTraps(numberTrap);
    }

    public void ChangeNbSouris(int nbSouris)
    {
        nombreSouris = nbSouris;
        ChangeTextCounterMice(nbSouris);
    }
    public void ChangeTextCounterMice(int textCounter)
    {
        if (miceCounterText != null) miceCounterText.text = textCounter.ToString();
    }

    public void ChangeNbTrap(int nbTrap)
    {
        numberTrap = nbTrap;
    }
    

    public void SkipTime()
    {
        timer = timeBeforeStart;
		GameObject.FindGameObjectWithTag("MenuCatch")?.GetComponent<ShowTriesTimer>().OpenMenu();
	}

    public void SpawnSourisRandom()
    {
        for (int i = 0; i < nombreSouris; i++)
        {
            Vector3 randomPosition = RandomNavMeshLocation();
            GameObject souris = Instantiate(sourisPrefab, randomPosition, Quaternion.Euler(90, 0, 0));
            sourisList.Add(souris.GetComponent<Souris>());
            souris.SetActive(false);
        }
        ScoreController.Instance.UpdateMaxMouseCount(nombreSouris);
    }
    public void SpawnSouris()
    {
        foreach (Souris souris in sourisList)
        {
            souris.gameObject.SetActive(true);
        }
       /* for (int i = 0; i < nombreSouris; i++)
        {
            Vector3 randomPosition = RandomNavMeshLocation();
            GameObject souris = Instantiate(sourisPrefab, randomPosition, Quaternion.Euler(90, 0, 0));
        }*/
    }

    private Vector3 RandomNavMeshLocation()
    {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        Vector3 randomPoint = Vector3.zero;

        int maxIterations = 100;

    for (int i = 0; i < maxIterations; i++)
    {
        int randomIndex = UnityEngine.Random.Range(0, triangulation.indices.Length - 2); 
        int index1 = triangulation.indices[randomIndex];
        int index2 = triangulation.indices[randomIndex + 1];
        int index3 = triangulation.indices[randomIndex + 2];

        Vector3 v1 = triangulation.vertices[index1];
        Vector3 v2 = triangulation.vertices[index2];
        Vector3 v3 = triangulation.vertices[index3];

        randomPoint = (v1 + v2 + v3) / 3f; 

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas)) 
        {
            randomPoint = hit.position;
            break;
        }
    }
            return randomPoint;
        }


        public void EndGame(bool isWin) 
        {
            sourisList.Clear();
            hunt = false;
            timer = 0;
            if (isWin)
            {
                Debug.Log("Gagné !");
                switch (sceneName)
                {
                    case "DayOne":
                        SceneController.Instance.LoadScene("DayTwo");
                        break;
                    case "DayTwo":
                        SceneController.Instance.LoadScene("DayThree");
                        break;
                    case "DayThree":
                        SceneController.Instance.LoadScene("DayFour");
                        break;
                    case "DayFour":
                        SceneController.Instance.LoadScene("MenuPrincipal");
<<<<<<< HEAD
                        Debug.Log("win");
=======
>>>>>>> parent of ba66407 (update)
                        break;
                    default:
                        break;
                }
            }
            else
            {
                SceneController.Instance.LoadScene("MenuPrincipal");
                Debug.Log("Perdu !");
            }
        }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
		GameObject[] temp = GameObject.FindGameObjectsWithTag("MiceCounter");
		if(temp.Length>0) miceCounterText = temp[0].GetComponent<TMPro.TMP_Text>();
        TrapSelectionMenu.Instance.Init();

		if (scene.name == "MenuPrincipal")
        {
            ScoreController.Instance.ReinitiateScore();
        }
        else if (scene.name == "Challenge")
        {
            isChallenge = true;
            MusicManager.Instance.PlayTrapMusic();
            SoundManager.Instance.MuteMice();
        }
        else
        {
            sceneName = scene.name;
            switch (scene.name)
            {
                case "DayOne":
                    timeBeforeStart = 0f;
                    break;
                case "DayTwo":
                    timeBeforeStart = 60f;
                    break;
                case "DayThree":
                    timeBeforeStart = 60f;
                    break;
                case "DayFour":
                    timeBeforeStart = 120f;
                    break;
                default:
                    return;
            }
            MusicManager.Instance.PlayTrapMusic();
            SoundManager.Instance.MuteMice();
            foreach (Souris go in Resources.FindObjectsOfTypeAll<Souris>())
            {
                if (go.gameObject.scene.name != null)
                    sourisList.Add(go);
            }
            ScoreController.Instance.UpdateMaxMouseCount(sourisList.Count);
			ChangeNbSouris(sourisList.Count);
			isChallenge = false;
        }
   
    }
}
