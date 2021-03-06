﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //General properties
    public bool playing;
    public int levelNum;
    public LevelMode levelMode;
    public DifficultyLevel difficulty;
    public int difficultyScaling;
    public float infectionLevel;
    public InfectionLevel infectionState;
    [Range(0, 800)]
    public int numRecentEnemyKills;
    [Range(0, 800)]
    public int numRecentEnemyEscapes;
    public int numRecentEnemiesCounted;

    //Enemy spawning
    public GameObject enemyPrefab;
    public GameObject enemyFractalGenerator;
    public List<BaseEnemyAbilityClass> enemyAbilities;
    public List<GameObject> possibleEnemySpawns;
    public float waveWaitDuration;
    public float levelWaitDuration;
    public Vector3 waveSpawnPos;
    public Vector3 waveSpawnSpacing;
    [Range(5,40)]
    public int waveSize;
    [Range(3,5)]
    public int numWaves;

    //Player
    public GameObject playerPrefab;
    //public int currentLives;
    //public int maxLives;
    public bool playerDead;
    public Vector3 playerSpawnPos;

    //UI
    public Text levelText;
    public Slider infectionSlider;
    public Text infectionText;
    public Text livesText;
    public GameObject mainCanvas;
    public GameObject PauseCanvas;
    public GameObject gOverCanvas;
    public GameObject blackOutImage;
    public GameObject bonusLevelAlert;

	// Use this for initialization
	void Start ()
    {
        PreLevelSetup();
        UpdateUI();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator DelayLevelSetup(float delay)
    {
        yield return new WaitForSeconds(delay);
        PreLevelSetup();
    }

    //Set up the level before the game starts
    public void PreLevelSetup()
    {
        if(playerDead)
        {
            RespawnPlayer();
        }
        CheckStartLevelConditions();
        int addEnemyFrequency = 2;
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                difficultyScaling = 2;
                addEnemyFrequency = 3;
                break;
            case DifficultyLevel.Medium:
                difficultyScaling = 3;
                addEnemyFrequency = 2;
                break;
            case DifficultyLevel.Hard:
                difficultyScaling = 4;
                addEnemyFrequency = 1;
                break;
        }
        if(possibleEnemySpawns.Count == 0)
        {
            for(int i = 1; i <= levelNum; i++)
            {
                if(i == 1 || i%addEnemyFrequency == 0)
                {
                    GenerateEnemy();
                }
            }
        }
        else if(levelNum%addEnemyFrequency == 0)
        {
            GenerateEnemy();
        }
        SetLevelProperties();
        //currentLives = maxLives;
        if(levelNum > 1)
        {
            StartCoroutine(DelayStartLevel(levelWaitDuration));
        }
        else
        {
            StartLevel();
        }
        
    }

    public void CheckStartLevelConditions()
    {
        switch (infectionState)
        {
            case InfectionLevel.Healthy:
                levelMode = LevelMode.Normal;
                SetPlayerStats(levelMode);
                break;
            case InfectionLevel.Sick:
                levelMode = LevelMode.Normal;
                SetPlayerStats(levelMode);
                break;
            case InfectionLevel.VerySick:
                float bonusLevelChance = Random.Range(0.0f, 100.0f);
                if (bonusLevelChance < 50)
                {
                    levelMode = LevelMode.Bonus;
                    SetBonusLevel();
                }
                else
                {
                    levelMode = LevelMode.Normal;
                    SetPlayerStats(levelMode);
                }
                break;
            case InfectionLevel.DeathlySick:
                levelMode = LevelMode.Bonus;
                SetBonusLevel();
                break;
        }
    }

    private void SetBonusLevel()
    {
        //display bonus level notification/ui
        Debug.Log("Bonus Level");
        GameObject alert = (GameObject)Instantiate(bonusLevelAlert, mainCanvas.transform);
        alert.transform.localPosition = new Vector3(1300, 150);
        SetPlayerStats(levelMode);
    }

    private void SetPlayerStats(LevelMode mode)
    {
        switch (mode)
        {
            case LevelMode.Normal:
                //maxLives = 3;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().attackCoolDownDuration = 0.25f;
                break;
            case LevelMode.Bonus:
                //maxLives = 3 + (5 - difficultyScaling);
                float pAttackSpeed = 0.25f;
                if(difficulty == DifficultyLevel.Easy)
                {
                    pAttackSpeed = 0.1f;
                }
                else
                {
                    pAttackSpeed = 0.15f;
                }
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().attackCoolDownDuration = pAttackSpeed;
                break;
        }
    }

    //Set number and size of waves, and numRecentEnemiesCounted
    public void SetLevelProperties()
    {
        //Calculate base waveSize
        waveSize = (int)Mathf.Clamp((5 * (levelNum * 0.75f) * (difficultyScaling/2)),10, (20*(difficultyScaling/2)));
        Debug.Log("Base wave size is: " + waveSize);
        //Scale waveSize with infection level
        waveSize = (int)(waveSize + (waveSize * (infectionLevel/100)));
        Debug.Log("Scaled wave size is: " + waveSize);
        numWaves = (int)(2 + (levelNum / 2));
        if(numRecentEnemiesCounted > 0)
        {
            float recentKills = numRecentEnemyKills;
            float recentEscapes = numRecentEnemyEscapes;
            float recentCounted = numRecentEnemiesCounted;
            float killsPercent = recentKills / recentCounted;
            Debug.Log("kill percent: " + killsPercent);
            Debug.Log("recent kills: " + recentKills);
            Debug.Log("recent counted: " + recentCounted);
            float escapesPercent = recentEscapes / recentCounted;
            numRecentEnemiesCounted = (waveSize) * 4;
            numRecentEnemyKills = (int)(numRecentEnemiesCounted * killsPercent);
            numRecentEnemyEscapes = (int)(numRecentEnemiesCounted * escapesPercent);
        }
        else
        {
            numRecentEnemiesCounted = (waveSize) * 4;
        }
        
    }

    //Generate a new enemy to add to spawn list
    public void GenerateEnemy()
    {
        GameObject enemy = (GameObject)Instantiate(enemyPrefab);
        int numAbilities = (int)(Mathf.Clamp((levelNum/2), 1, 8) * (difficultyScaling/2));
        Debug.Log("levelnum: " + levelNum / 2 + " Scaling: " + difficultyScaling / 2);
        Debug.Log("enemy abilities: " + numAbilities);
        List<BaseEnemyAbilityClass> _abilities = new List<BaseEnemyAbilityClass>();
        if(numAbilities > 1)
        {
            for(int i = 1; i < numAbilities; i++)
            {
                BaseEnemyAbilityClass ability = getEnemyAbility(enemy);
                if(ability != null)
                {
                    enemy.AddComponent(ability.GetType());
                }
            }
        }
        //_abilities.Add(enemyAbilities[0]);//Add default attack to list
        enemy.AddComponent(enemyAbilities[0].GetType());
        //enemy.GetComponent<EnemyClass>().SetEnemyAbilities(_abilities);
        GameObject fractal = GenerateEnemyFractal();
        fractal.transform.SetParent(enemy.transform);
        enemy.transform.SetParent(gameObject.transform);
        enemy.GetComponent<EnemyClass>().bulletPrefab.GetComponent<MeshRenderer>().material = fractal.GetComponentInChildren<MeshRenderer>().material;
        possibleEnemySpawns.Add(enemy);
        enemy.SetActive(false);
    }

    //Generate fractal appearance of enemy
    public GameObject GenerateEnemyFractal()
    {
        GameObject fractal = (GameObject)Instantiate(enemyFractalGenerator);
        fractal.GetComponent<EnemyFractalGenerator>().SetValues(difficultyScaling, true);
        fractal.GetComponent<EnemyFractalGenerator>().Populate();
        return fractal;
    }

    private BaseEnemyAbilityClass getEnemyAbility(GameObject enemy)
    {
        BaseEnemyAbilityClass[] abilities = enemy.GetComponents<BaseEnemyAbilityClass>();
        BaseEnemyAbilityClass randAbility = enemyAbilities[Random.Range(1, enemyAbilities.Count)];
        int hits = 0;
        foreach(BaseEnemyAbilityClass tempAbility in abilities)
        {
            if(tempAbility.GetType() == randAbility.GetType())
            {
                hits++;
            }
        }
        if(hits == 0)
        {
            return randAbility;
        }
        else
        {
            for(int i = 1; i < enemyAbilities.Count; i++)
            {
                hits = 0;
                foreach (BaseEnemyAbilityClass tempAbility in abilities)
                {
                    if (tempAbility.GetType() == enemyAbilities[i].GetType())
                    {
                        hits++;
                    }
                }
                if (hits == 0)
                {
                    return enemyAbilities[i];
                }
            }
        }
        return null;
    }

    private IEnumerator DelayStartLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartLevel();
    }

    public void StartLevel()
    {
        Time.timeScale = 1.0f;
        playing = true;
        StartCoroutine(SpawnEnemies());
    }

    public IEnumerator SpawnEnemies()
    {
        int _numWaves = numWaves;
        while(_numWaves > 0)
        {
            if(!playerDead)
            {
                _numWaves--;
                SpawnWave();
                yield return new WaitForSeconds(3 + ((waveSize / 5) * 1.0f));//make faster as level increases?
            }
            else
            {
                if (GameObject.FindGameObjectsWithTag("Enemy").Length <= 0)
                {
                    RespawnPlayer();
                }
                else
                {
                    Debug.Log("Still enemies");
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        if(playing)
        {
            LevelOver();
        }
        //while num waves to spawn > 0
            //tick down num waves to spawn
            //spawn wave
            //wait for wave wait duration
            //If !playing, break
        //after while loop [call level over/set (levelOver bool) to true/check if enemies are all dead, then call level over]
    }

    public void SpawnWave()
    {
        GameObject enemy = possibleEnemySpawns[Random.Range(0, possibleEnemySpawns.Count)];
        int numRows = waveSize / 5;
        for(int i = 0; i < numRows; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                GameObject temp = (GameObject)Instantiate(enemy, new Vector3(waveSpawnPos.x + (i * waveSpawnSpacing.x), waveSpawnPos.y, (-waveSpawnSpacing.z * 2.0f) + (j * waveSpawnSpacing.z)), Quaternion.identity);
                temp.SetActive(true);
            }
        }
    }

    public void EnemyRemoved(string status)
    {
        switch(status)
        {
            case "Dead":
                
                if(numRecentEnemyKills + numRecentEnemyEscapes > numRecentEnemiesCounted)
                {
                    numRecentEnemyKills++;
                    numRecentEnemyEscapes = numRecentEnemiesCounted - numRecentEnemyKills;
                    if(numRecentEnemyEscapes < 0)
                    {
                        numRecentEnemyEscapes = 0;
                    }
                }
                else if (numRecentEnemyKills + numRecentEnemyEscapes == numRecentEnemiesCounted)
                {
                    numRecentEnemyKills++;
                    numRecentEnemyEscapes--;
                    if (numRecentEnemyEscapes < 0)
                    {
                        numRecentEnemyEscapes = 0;
                    }
                }
                else if(numRecentEnemyKills + numRecentEnemyEscapes < numRecentEnemiesCounted)
                {
                    numRecentEnemyKills++;
                }
                break;
            case "Escaped":
                if (numRecentEnemyKills + numRecentEnemyEscapes > numRecentEnemiesCounted)
                {
                    numRecentEnemyEscapes+= 2;
                    numRecentEnemyKills = numRecentEnemiesCounted - numRecentEnemyEscapes;
                    if (numRecentEnemyKills < 0)
                    {
                        numRecentEnemyKills = 0;
                    }
                }
                else if (numRecentEnemyKills + numRecentEnemyEscapes == numRecentEnemiesCounted)
                {
                    numRecentEnemyEscapes += 4;
                    numRecentEnemyKills -= 4;
                    if (numRecentEnemyKills < 0)
                    {
                        numRecentEnemyKills = 0;
                    }
                }
                else if (numRecentEnemyKills + numRecentEnemyEscapes < numRecentEnemiesCounted)
                {
                    numRecentEnemyEscapes+= 4;
                }
                break;
        }
        CalculateInfection();
        UpdateUI();
    }

    public void CalculateInfection()
    {
        float escapes = numRecentEnemyEscapes;
        float cap = numRecentEnemiesCounted;
        //Debug.Log(escapes/cap);
        infectionLevel = (float)(escapes / cap) * 100;
        blackOutImage.GetComponent<RawImage>().color = new Color(0.6f, 0, 0, (infectionLevel/200));
        if(infectionLevel <= 20)
        {
            infectionState = InfectionLevel.Healthy;
        }
        else if(infectionLevel <= 60)
        {
            infectionState = InfectionLevel.Sick;
        }
        else if(infectionLevel <= 90)
        {
            infectionState = InfectionLevel.VerySick;
        }
        else if(infectionLevel <= 100)
        {
            infectionState = InfectionLevel.DeathlySick;
        }
    }

    public void PlayerDead(GameObject _player)
    {
        Destroy(_player);
        playerDead = true;
        UpdateUI();
    }

    public void RespawnPlayer()
    {
        GameObject _player = (GameObject)Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        _player.GetComponent<PlayerController>().StartInvul();
        playerDead = false;
        SetPlayerStats(levelMode);
    }

    public void UpdateUI()
    {
        levelText.text = "Night: " + levelNum;
        //livesText.text = "Lives: " + currentLives;
        infectionSlider.value = infectionLevel;
    }

    public void GameOver(string gOCon)
    {
        playing = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().playing = false;
        gOverCanvas.gameObject.SetActive(true);
    }

    //Increase levelNum, and start prep for next level
    public void LevelOver()
    {
        if(CheckEndLevelConditions())
        {
            GameOver("Lost");
        }
        else
        {
            levelNum++;
            UpdateUI();
            //PreLevelSetup();
            StartCoroutine(DelayLevelSetup(2));
        }
        
    }

    public bool CheckEndLevelConditions()
    {
        switch(infectionState)
        {
            case InfectionLevel.Healthy:

                break;
            case InfectionLevel.Sick:

                break;
            case InfectionLevel.VerySick:

                break;
            case InfectionLevel.DeathlySick:
                float deathChance = Random.Range(0.0f, 100.0f);
                if (deathChance < 50)
                {
                    return true;
                    //GameOver("Lost");
                }
                break;
        }
        return false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        PauseCanvas.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        PauseCanvas.gameObject.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

}

public enum DifficultyLevel { Easy, Medium, Hard }
public enum InfectionLevel { Healthy, Sick, VerySick, DeathlySick }
public enum LevelMode { Normal, Bonus }
