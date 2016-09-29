using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	public Transform[] occurEnemyTransform = new Transform[3];
	public GameObject Enemy;
	public Vector2 startWait;
	public Vector2 onceWait;
	public Vector2 waveWait;
	public int waveAccount=2;
	public int onceAccount=2;

	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;
	public GUIText quitText;

	private bool gameOver;
	private bool restart;
	private int score;
	private int count;
	private bool exit;
	void Start()
	{
		count = onceAccount;
		gameOver = false;
		restart = false;
		exit = false;
		restartText.text = "";
		quitText.text = "";
		gameOverText.text = "";
		score = 0;
		UpdateScore ();
		StartCoroutine(OccurEnemy());
	}
	void Update()
	{
		if (restart)
		{
			if (Input.GetKeyDown (KeyCode.R))
			{
				//print("R");
				Application.LoadLevel (Application.loadedLevel);
			}
		}
		if( exit ) 
		{
			if(Input.GetKeyDown (KeyCode.E))
			{
				//print("exit");
				Application.Quit();
			}
		}
		if (count == 0) 
		{
			if (gameOver)
			{
				//print("over");
				restartText.text = "Press 'R' for Restart";
				quitText.text=     "Press 'E' for Exit   ";
				restart = true;
				exit = true;
			}
		}
	}
	IEnumerator OccurEnemy()
	{	
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));
		while (waveAccount!=0) 
		{
			while(count!=0)
			{
				for(int i=0;i<3;i++)
				{

					Instantiate(Enemy,occurEnemyTransform[i].position,occurEnemyTransform[i].rotation);
				}

				yield return new WaitForSeconds (Random.Range (onceWait.x, onceWait.y));
				count--;
				print ("count:"+count);
				if (gameOver)
				{
					//print("over");
					restartText.text = "Press 'R' for Restart";
					quitText.text="Press 'E' for Exit";
					restart = true;
					exit=true;
					break;
				}
			}
			waveAccount--;
			if(waveAccount!=0){
				count=onceAccount;//重新计数为下次循环做准备
				yield return new WaitForSeconds (Random.Range (waveWait.x, waveWait.y));
			}
		}
		while (waveAccount==0) 
		{
			if (GameObject.FindWithTag ("enemy") == null&&!gameOver)
			{
				gameOverText.text = "You Win!";
				gameOver=true;
				break;
			}
			yield return new WaitForSeconds (1.8f);
		}
		//count = -1;//若协同函数正常退出，将cout的值赋为-1
	}

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	}

	public void GameOver ()
	{
		gameOverText.text = "Game Over!";
		gameOver = true;
		//return true;
	}
	public bool IsOver()
	{
		if (gameOver == true)
			return true;
		else 
			return false;
	}

}
