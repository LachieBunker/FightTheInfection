using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public Vector3[] screenPositions;
    public Vector3 destination;
    public float moveSpeed;
    public float dir;
    public bool moving;
    public int gameLevel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(moving)
        {
            transform.Translate(moveSpeed * dir, 0, 0);
            Debug.Log(Vector3.Distance(transform.localPosition, destination));
            if(Vector3.Distance(transform.localPosition, destination) < 10)
            {
                transform.localPosition = destination;
                moving = false;
                if(destination == screenPositions[2])
                {
                    LoadLevel(gameLevel);
                }
            }
        }
	}

    public void ChangeScreen(int newScreenNum)
    {
        destination = screenPositions[newScreenNum];
        if(destination.x < transform.position.x)
        {
            dir = -1;
        }
        else
        {
            dir = 1;
        }
        moving = true;
    }

    public void SetGameLevel(int level)
    {
        gameLevel = level;
    }

    public void LoadLevel(int levelNum)
    {
        SceneManager.LoadScene(levelNum);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
