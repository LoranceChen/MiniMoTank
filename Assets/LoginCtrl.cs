using UnityEngine;
using System.Collections;
using UniRx;
using RSG;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoginCtrl : MonoBehaviour {
	IPromise<NetGate> netGateProm;

	public GameObject netWorldPrefab;
	public GameObject loginObj;

	public Button loginBtn;

	public GameObject enterObj;
	public Button enterWorldBtn;

	public Text account;
	public Text pwd;
	public Text rspTips;

	// Use this for initialization
	void Start () {
		netGateProm = NetGate.s_netGate;
//		do after login success
//		var worlds = GetWorldInfo (new WorldInfoReq ());
//		worlds.Done(x => {
//			x.states.ForEach(y => {
//				Debug.Log("world - " + y);
//			});
//		});
		//todo wait server complete the api
//		var roles = GetRoleInfo (new RoleInfoReq ());
//		roles.Done (x => {
//			x.roles.ForEach(y => {
//				Debug.Log("role - " + y);
//			});
//		});
//
		loginBtn.onClick.AddListener (() => {
			//todo BUG: not dispatch on main sometimes
			for(var i = 0; i < 50; i++) {
			Login (new LoginReq (account.text, pwd.text, 1L)).SchedulerOn (Scheduler.MainThread).Done (x => {
				Debug.Log ("login req thread - " + System.Threading.Thread.CurrentThread.ManagedThreadId);
				Package.Log ("Login response - " + x.ToString ());
				if (x.errorMsg == null) {
					Package.Log ("login success - " + x.ToString ());
					//choice game world -> choice role
					enterObj.SetActive (true);
					loginObj.SetActive (false);
				} else {
					rspTips.text = x.ToString ();
				}
			});
			}
		});

		enterWorldBtn.onClick.AddListener(() => {
			Debug.Log("enterWorldBtn.onClick");
			EnterWorld(new EnterWorldReq("127.0.0.1", 12553, GateProtoID.ENTER_WORLD)).SchedulerOn(Scheduler.MainThread).Done(x => {
				Debug.Log("enter world ready success - " + x.isSuccess);
				//do connect worlds socket
				var netWorld = Instantiate(netWorldPrefab);
				var jsonFur = NetWorld.s_notWorldFur.Then(xx => {
					return xx.jsonObvFur;
				});
				jsonFur.Done(xxx => {
					SceneManager.LoadSceneAsync(SceneConstant.World);
				});
				DontDestroyOnLoad(netWorld);
				netGateProm.Then(netGate => Destroy(netGate.gameObject));
			});
		});

//		NetStart.s_inst.Login (new LoginReq ("admin", "admin", NetStart.s_inst.GetTaskId(), 1L)).Done (x => 
//			Debug.Log(x.json)
//		);

//		Login (new LoginReq ("admin", "admin", 1L)).Done (x => 
//			Debug.Log("login result - " + x.ToString())
//		);

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
		return netGateProm.Then(x => x.SendWithJsonResult<LoginReq, LoginRsp>(loginReq));
	}

	public IPromise<WorldInfoRsp> GetWorldInfo(WorldInfoReq worldInfoReq) {
		return netGateProm.Then(x => x.SendWithJsonResult<WorldInfoReq, WorldInfoRsp>(worldInfoReq));
	}

	public IPromise<RoleInfoRsp> GetRoleInfo(RoleInfoReq roleInfoReq) {
		return netGateProm.Then(x => x.SendWithJsonResult<RoleInfoReq, RoleInfoRsp>(roleInfoReq));
	}

	public IPromise<EnterWorldRsp> EnterWorld(EnterWorldReq enterWorld) {
		return netGateProm.Then(x => x.SendWithJsonResult<EnterWorldReq, EnterWorldRsp>(enterWorld));
//		return netStart.socket.Done (x => {
//			x.send();
//		});//<EnterWorld, RoleInfoRsp>(roleInfoReq);
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

public class WorldInfoReq{
	public long aim = GateProtoID.GET_WORLD_SERVER_STATUS;

	public override string ToString ()
	{
		return string.Format ("[WorldInfoReq] {0}", aim);
	}
}

//case class Status(host: String, port: Int, loadLevel: String, name: String)
[System.Serializable]
public class WorldInfoRsp{
	public List<Status> states;//todo create Map for List

	public override string ToString ()
	{
		return string.Format ("[WorldInfoRsp] {0}", states);
	}
}
[System.Serializable]
public class Status {
	public string host;
	public int port;
	public string loadLevel;
	public string name;
}

public class RoleInfoReq{
	public long aim = GateProtoID.GET_ROLES;

	public override string ToString ()
	{
		return string.Format ("[LoginRsp] {0}", aim);
	}
}

[System.Serializable]
public class RoleInfoRsp{
	public List<Role> roles;

	public override string ToString ()
	{
		return string.Format ("[LoginRsp] {0}", roles);
	}
}

//Role(name: String, level: Int, id: String)
[System.Serializable]
public struct Role {
	public string name;
	public int level;
	public string id;
}

//EnterWorld(host: String, port: Int, taskId: String, aim: Long) 
[System.Serializable]
public struct EnterWorldReq {
	public string host;
	public int port;
	public long aim;

	public EnterWorldReq(string host, int port, long aim){
		this.host = host;
		this.port = port;
		this.aim = aim;
	}
}

public struct EnterWorldRsp {
	public bool isSuccess;
}