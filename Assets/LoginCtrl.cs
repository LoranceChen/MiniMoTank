using UnityEngine;
using System.Collections;
using UniRx;
using RSG;

public class LoginCtrl : MonoBehaviour {
	NetStart netStart;
	// Use this for initialization
	void Start () {
		netStart = NetStart.s_inst;
//		NetStart.s_inst.Login (new LoginReq ("admin", "admin", NetStart.s_inst.GetTaskId(), 1L)).Done (x => 
//			Debug.Log(x.json)
//		);

		Login (new LoginReq ("admin", "admin", 1L)).Done (x => 
			Debug.Log("login result - " + x.ToString())
		);

	
//		new System.Threading.Thread (() => {
//			Debug.Log("LoginCtrl: thread id - " + System.Threading.Thread.CurrentThread.ManagedThreadId);
//			var i = 0;
//			while(i < 1) {
//				i += 1;
//
//				Login (new LoginReq ("admin", "admin", 1L)).Done (x => {
//					Debug.Log("login result - " + x.ToString());
//				});
//				System.Threading.Thread.Sleep(4*1000 );
//			}
//		}).Start();

	}

	//TryLogin(name: String, pwd: String, taskId: String, aim: Long)
	//case class AccountAuthenRsp(id: String, errorMsg: Option[String])
	public IPromise<LoginRsp> Login(LoginReq loginReq) {
		return netStart.SendWithJsonResult<LoginReq, LoginRsp>(loginReq);
	}
}

//Net message
public class LoginReq{//: JsonEncode {
	public string name;
	public string pwd;
	public long aim;
	public LoginReq (string name, string pwd, long aim) {
		this.name = name;
		this.pwd = pwd;
		this.aim = aim;
	}

	public override string ToString ()
	{
		return string.Format ("[LoginReq] {0} {1} {2}", name, pwd, aim);
	}
}

public class LoginRsp{//: TaskIdentity, JsonDecode {
	public string id;
	public string errorMsg;

	public override string ToString ()
	{
		return string.Format ("[LoginRsp] {0} {1}", id, errorMsg);
	}
}
