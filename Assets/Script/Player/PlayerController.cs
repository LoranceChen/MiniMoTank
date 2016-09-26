using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary 
{
	public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour 
{
	public Boundary boundary;
	public float speed;
	public float fireRate;
	public Transform shotSpawn;
	public GameObject Bullet;
	public AudioSource audioShot;
	//public GameObject gameController;
	private float nextFire;
	private int state=0;
	private GameController gameController;
	private bool notMove=false;
	//private char[] direction=new char[2]{'0','0'};
	void Start()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
		StartCoroutine(CouldMove());
	}
	IEnumerator CouldMove()
	{
		yield return new WaitForSeconds (1);
		while (true) {
						if (gameController.IsOver ()) {
								notMove = true;
						}
			yield return new WaitForSeconds (2.5f);
				}
	}
	void Update()
	{ 
		if (notMove)
		{
			GetComponent<Rigidbody2D>().velocity=Vector2.zero;
			return;
		}
		if (Input.GetButton ("Fire1") && Time.time > nextFire)
		{
			nextFire = Time.time + fireRate; 
			Instantiate (Bullet, shotSpawn.position, shotSpawn.rotation);
			audioShot.Play ();
		}
		transform.position = new Vector3
		(
			Mathf.Clamp (transform.position.x, boundary.xMin, boundary.xMax),  
			Mathf.Clamp (transform.position.y, boundary.yMin, boundary.yMax),
			0.0f
		);
	}
	void FixedUpdate ()
	{
		if (notMove)
			return;
		float moveHorizontal = 0;
		float moveVertical = 0;
		if(state==0)
			GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
		if (MoveToQuit() == 1)//从移动到静止 
		{
			GetComponent<Rigidbody2D>().velocity=new Vector2(0,0);
			return;//跳出FixedUpdate循环
		}
		if (QuitToMove () != 0)//从静止到移动 
		{
			MoveToMove ();//决定方向
		}
		//移动到移动
		MoveToMove ();

		if (state==1) 
		{
			moveHorizontal = Input.GetAxis ("Horizontal");
			//旋转
			if(moveHorizontal>0)
			{
				transform.eulerAngles=new Vector3(0,0,270);
			}
			if(moveHorizontal<0)
			{
				transform.eulerAngles=new Vector3(0,0,90);
			}
			GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(moveHorizontal)*speed,0);
			//moveVertical=0;
		}
		else if (state==2) 
		{
			moveVertical = Input.GetAxis ("Vertical");
			if(moveVertical >0)
			{
				transform.eulerAngles=new Vector3(0,0,0);
			}
			if(moveVertical <0)
			{
				transform.eulerAngles=new Vector3(0,0,180);
			}
			GetComponent<Rigidbody2D>().velocity = new Vector2(0,Mathf.Sign(moveVertical)*speed);
			//moveHorizontal=0;
		}
		//transform.position = new Vector3
			//(
			//	Mathf.Clamp (transform.position.x, boundary.xMin, boundary.xMax),  
			//	Mathf.Clamp (transform.position.z, boundary.yMin, boundary.yMax),
			//	0.0f
			//);
	}
	int QuitToMove()//上次的state，判定这次
	{
		//如果是静止状态，判断是否有输入
		if (state == 0) 
		{
			if(Input.GetAxis("Horizontal")!=0)
			{	
				state=1;
				return 1;
			}
			if(Input.GetAxis("Vertical")!=0)
			{
				state=2;
				return 2;
			}
		}
		return 0;
	}
	int MoveToQuit()
	{
		//上次为移动状态
		if (state != 0) 
		{
			//这次没有按键触发
			if(Input.GetAxis("Horizontal")==0&&Input.GetAxis("Vertical")==0)
			{
				state=0;//修改状态为静止状态
				return 1;
			}	
		}
		return 0;
	}
	void MoveToMove()
	{

		if (state == 1) 
		{
			//先判断是否有水平触发
			if(Input.GetAxis ("Horizontal")!=0)
			{
				//state=1;
				return;//状态不变
			}
			//没有水平触发判断是否有垂直触发
			else if(Input.GetAxis ("Vertical")!=0)
				{
					state=2;//修改状态
					return;
				}	
				else
				{
					state=0;
					return;
				}
		}
		if (state == 2) 
		{
			if(Input.GetAxis ("Vertical")!=0)
			{
				//state=2;
				return;
			}
			else if(Input.GetAxis ("Horizontal")!=0)
			{
				state=1;
				return;
			}		
		}
		if (Input.GetAxis ("Horizontal") ==0&& Input.GetAxis ("Vertical") ==0)
		{
			state=0;
			return;
		}
		return;
	}
}
