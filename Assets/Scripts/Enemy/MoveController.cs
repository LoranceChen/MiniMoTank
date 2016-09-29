using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {
	//public float ;
	public Boundary boundary; 
	public Vector2 startWait;//开始时，决定要改变方向的预备时间 [0,1]
	public Vector2 maneuverTime;//间隔时间[2,5]
	//public Vector2 maneuverWait;
	//public Vector2 maneuverSpeed;//速度的变化范围0,4
	public Transform[] rayLine = new Transform[8];
	public float moveSpeed=4;//坦克的速度

	private int midDirection=0;
	private int targetManeuver=0;//控制方向.0,1,2,3 下，左，右，上（绝对坐标）
	private bool[] rayIsCollide =new bool[4]{false,false,false,false};//前，左，右，后（相对坐标）
	//private globeRotation;
	// Use this for initialization

	void Start () {

		StartCoroutine(Maneuver());
	}
	//每隔maneuverTime长的时间运行一次————
	//Physics2D.Linecast()判定有无障碍物
	//		1.在前方有障碍物时改变一次方向。
	//		2.左右方向没障碍物时改变一次方向。
	//3.方向由targetManeuver的值决定。-1左 1右 -2下 2上
	//3.
	//方向为准（70%），左右为此()，“后”在前方不能移动时才会触发(小概率15%)
	//	世界坐标是：向下为主
	IEnumerator Maneuver ()
	{

		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));
		while (true) 
		{
			//targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			DecideDirection();

			for(int i=0;i<4;i++)//决定下次的方向
			{
				if(!rayIsCollide[i])//该方向无障碍物
				{
					//“前方”没有阻碍，先按“前方”（原方向）行驶的判定
					if(!rayIsCollide[targetManeuver])//最近一次选择的方向没有障碍物
					{
						float justDirection=Random.Range(0.0f,1.0f);
						if(justDirection>0.5)
							break;
					}
					//随机决定下次方向
					float t=Random.Range(0.0f,3.5f);
					if(t>1.0)//70%概率朝向这个方向
					{
						//print ("rang:  "+t);
						midDirection=i;//决定相对方向
						//targetManeuver2=targetManeuver;
						break;
					}
				}
			}
			//print ("midDirection："+midDirection);
			//relative transfor to globe
			//print ("transform.eulerAnger.z："+transform.eulerAngles.z);
			     //下-y
			/*float z=transform.eulerAngles.z;
			print ("z:"+z);
			if(Mathf.Abs(z-180)<0.01)
			{
				print ("z=180!!!");

			}
			else 
				print ("z!=180");*/
			if(Mathf.Abs(transform.eulerAngles.z-180)<0.01)
			{

				switch(midDirection)
				{
				case 0://relative forword
					targetManeuver=0;
					break;
				case 1://relative left
					targetManeuver=2;
					break;
				case 2://relative right
					targetManeuver=1;
					break;
				case 3://relative back
					targetManeuver=3;
					break;
				default:break;
				}
				//print ("switched targetManeuver:"+targetManeuver);
			}
			//else if(transform.rotation.z==90)//左-x
			else if(Mathf.Abs(transform.rotation.eulerAngles.z-90)<0.01)
			{
				switch(midDirection)
				{
				case 0://relative forword
					targetManeuver=1;
					break;
				case 1://relative left
					targetManeuver=0;
					break;
				case 2://relative right
					targetManeuver=3;
					break;
				case 3://relative back
					targetManeuver=2;
					break;
				default:break;
				}
			}
			//else if(transform.rotation.z==270)//右+x
			else if(Mathf.Abs(transform.rotation.eulerAngles.z-270)<0.01)
			{
				switch(midDirection)
				{
				case 0://relative forword
					targetManeuver=2;
					break;
				case 1://relative left
					targetManeuver=3;
					break;
				case 2://relative right
					targetManeuver=0;
					break;
				case 3://relative back
					targetManeuver=1;
					break;
				default:break;
				}
			}
			else if(Mathf.Abs(transform.rotation.eulerAngles.z-0)<0.01)//后+y
			{
				switch(midDirection)
				{
				case 0://relative forword
					targetManeuver=3;
					break;
				case 1://relative left
					targetManeuver=1;
					break;
				case 2://relative right
					targetManeuver=2;
					break;
				case 3://relative back
					targetManeuver=0;
					break;
				default:break;
				}
			}
			//print ("Coroutine targetManeuver:   "+targetManeuver);
			//moveSpeed=Random.Range(maneuverSpeed.x,maneuverSpeed.y);//改变坦克的速度
			yield return new WaitForSeconds (Random.Range (maneuverTime.x, maneuverTime.y));
			//targetManeuver = 0;
			//yield return new WaitForSeconds (Random.Range (maneuverWait.x, maneuverWait.y));
		}

	}
	void Update()
	{
		transform.position = new Vector3
			(
				Mathf.Clamp (transform.position.x, boundary.xMin, boundary.xMax),  
				Mathf.Clamp (transform.position.y, boundary.yMin, boundary.yMax),
				0.0f
				);
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		//移动，根据Maneuver中的targetManeuver的值选择移动的方向
		//对于敌军来说，下面是前方
		switch (targetManeuver)
		{
		case 1:
			//左边
			transform.eulerAngles=new Vector3(0,0,90);
			//transform.Rotate(Vector3.forward * 90, Space.Self);
			GetComponent<Rigidbody2D>().velocity=new Vector2(-moveSpeed,0);

			//targetManeuver=-1;
			break;
		case 2:
			//右边
			transform.eulerAngles=new Vector3(0,0,270);
			//transform.Rotate(Vector3.forward * 270, Space.Self);
			GetComponent<Rigidbody2D>().velocity=new Vector2(moveSpeed,0);
			//targetManeuver=-1;
			break;
		case 3:
			//上方
			transform.eulerAngles=new Vector3(0,0,0);
			//transform.Rotate(Vector3.forward * 180, Space.Self);
			GetComponent<Rigidbody2D>().velocity=new Vector2(0,moveSpeed);
			//targetManeuver=-1;
			break;
		case 0:
			//下方
			transform.eulerAngles=new Vector3(0,0,180);
			GetComponent<Rigidbody2D>().velocity=new Vector2(0,-moveSpeed);
			//targetManeuver=-1;
			break;
		default:break;
		}
		//print ("targetManeuver"+targetManeuver);
	}
	void DecideDirection()
	{
		for(int i=0;i<4;i++)//向四个方向发射射线，将检测结果放入rayIsCollide中。
		{
			int j=i*2;//0,2,4,6
			//每个方向检测两条射线
			for(;j<i*2+2;j++)//[0,2),[2,4),[4,6),[6,8)
			{
				if(Physics2D.Linecast(transform.position,rayLine[j].position,1<<LayerMask.NameToLayer("ObstacleCan'tMove")))
				{
					rayIsCollide[i]=true;
					break;//有一条碰撞返回true
				}
				else 
					rayIsCollide[i]=false;
			}
			//print("rayIsCollide["+i+"]: "+rayIsCollide[i]);	

		}
	}
}
