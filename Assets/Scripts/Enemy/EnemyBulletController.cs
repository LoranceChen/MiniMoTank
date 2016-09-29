using UnityEngine;
using System.Collections;

public class EnemyBulletController : MonoBehaviour 
{
	public float fireRate=3;//两次时间间隔
	public Vector2 autoWait;//自动随机发射子弹
	public GameObject enemyBullet;
	public Transform bulletAnchor;
	public Transform rayForBullet;

	private float nearFire;
	void Start()
	{
		StartCoroutine (AutoFire ());
	}
	IEnumerator AutoFire()//每隔（2,5）秒发射子弹
	{
		while (true) 
		{

			yield return new WaitForSeconds (Random.Range (autoWait.x, autoWait.y));
			if(Time.time>nearFire)
			Instantiate(enemyBullet,bulletAnchor.position,bulletAnchor.rotation);
			nearFire=fireRate+Time.time;//每次发射子弹更新nearFire的值
		}
	}
	void Update()
	{
		if (Physics2D.Linecast ( bulletAnchor.position,rayForBullet.position, 1 << LayerMask.NameToLayer ("Player")|1<<LayerMask.NameToLayer ("Home"))&&Time.time>nearFire) 
		{
			nearFire=fireRate+Time.time;//每次if成立，说明要发射子弹，这时更新nearFire的值
			Instantiate(enemyBullet,bulletAnchor.position,bulletAnchor.rotation);
			//print("find");
		}
	}
}
