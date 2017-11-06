using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    //General properties
    public bool playing;
    public int levelNum;
    public string difficultySetting;
    public int difficultyScaling;
    public float infectionLevel;
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
    public Vector3 waveSpawnPos;
    public Vector3 waveSpawnSpacing;
    [Range(5,40)]
    public int waveSize;
    [Range(10,20)]
    public int numWaves;

    //Player
    public GameObject playerPrefab;
    public int currentLives;
    public int maxLives;
    public Vector3 playerSpawnPos;

    //UI
    public Text levelText;
    public Slider infectionSlider;
    public Text infectionText;
    public Text livesText;
    public GameObject PauseCanvas;
    public GameObject gOverCanvas;


	// Use this for initialization
	void Start ()
    {
        PreLevelSetup();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Set up the level before the game starts
    public void PreLevelSetup()
    {
        
        int addEnemyFrequency = 2;
        switch (difficultySetting)
        {
            case "Easy":
                addEnemyFrequency = 3;
                break;
            case "Medium":
                addEnemyFrequency = 2;
                break;
            case "Hard":
                addEnemyFrequency = 1;
                break;
        }
        if (levelNum == 1)
        {
            GenerateEnemy();
        }
        else if(levelNum%addEnemyFrequency == 0)
        {
            GenerateEnemy();
        }
        SetLevelProperties();
        StartLevel();
    }

    //Set number and size of waves, and numRecentEnemiesCounted
    public void SetLevelProperties()
    {
        
        waveSize = (int)Mathf.Clamp((5 * (levelNum * 0.75f) * (difficultyScaling/2)),10, (20*(difficultyScaling/2)));
        numWaves = (int)(5 + (levelNum / 2));
        numRecentEnemiesCounted = (numWaves * waveSize) * 2;
    }

    //Generate a new enemy to add to spawn list
    public void GenerateEnemy()
    {
        GameObject enemy = (GameObject)Instantiate(enemyPrefab);
        int numAbilities = (int)((levelNum / 5) * (difficultyScaling/2));
        List<BaseEnemyAbilityClass> _abilities = new List<BaseEnemyAbilityClass>();
        if(numAbilities > 1)
        {
            for(int i = 1; i < numAbilities; i++)
            {
                BaseEnemyAbilityClass ability = enemyAbilities[Random.Range(1, enemyAbilities.Count)];
                enemy.AddComponent(ability.GetType());
            }
        }
        //_abilities.Add(enemyAbilities[0]);//Add default attack to list
        enemy.AddComponent(enemyAbilities[0].GetType());
        //enemy.GetComponent<EnemyClass>().SetEnemyAbilities(_abilities);
        GameObject fractal = GenerateEnemyFractal();
        fractal.transform.SetParent(enemy.transform);
        enemy.transform.SetParent(gameObject.transform);
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
            _numWaves--;
            SpawnWave();
            yield return new WaitForSeconds(waveWaitDuration);//make faster as level increases?
            if(!playing)
            {
                break;
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
                GameObject temp = (GameObject)Instantiate(enemy, new Vector3(waveSpawnPos.x + (i * waveSpawnSpacing.x), waveSpawnPos.y, waveSpawnPos.z + (j * waveSpawnSpacing.z)), Quaternion.identity);
                temp.SetActive(true);
            }
        }
    }

    public void EnemyRemoved(string status)
    {
        switch(status)
        {
            case "Dead":
                
                if(numRecentEnemyKills + numRecentEnemyEscapes == numRecentEnemiesCounted)
                {
                    numRecentEnemyKills++;
                    numRecentEnemyEscapes--;
                    if(numRecentEnemyEscapes < 0)
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
                if (numRecentEnemyKills + numRecentEnemyEscapes == numRecentEnemiesCounted)
                {
                    numRecentEnemyEscapes++;
                    numRecentEnemyKills--;
                    if (numRecentEnemyKills < 0)
                    {
                        numRecentEnemyKills = 0;
                    }
                }
                else if (numRecentEnemyKills + numRecentEnemyEscapes < numRecentEnemiesCounted)
                {
                    numRecentEnemyEscapes++;
                }
                break;
        }
        CalculateInfection();
    }

    public void CalculateInfection()
    {
        float escapes = numRecentEnemyEscapes;
        float cap = numRecentEnemiesCounted;
        //Debug.Log(escapes/cap);
        infectionLevel = (float)(escapes / cap) * 100;
    }

    public void PlayerDead(GameObject _player)
    {
        Destroy(_player);
        currentLives--;
        UpdateUI();
        if(currentLives > 0)
        {
            RespawnPlayer();
        }
        else if(currentLives <= 0)
        {
            GameOver("Lost");
        }
    }

    public void RespawnPlayer()
    {
        GameObject _player = (GameObject)Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        _player.GetComponent<PlayerController>().StartInvul();
    }

    public void UpdateUI()
    {

    }

    public void GameOver(string gOCon)
    {

    }

    //Increase levelNum, and start prep for next level
    public void LevelOver()
    {
        levelNum++;
        UpdateUI();
        PreLevelSetup();
    }

}
