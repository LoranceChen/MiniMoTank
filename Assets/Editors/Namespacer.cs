using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

public class Namespacer : EditorWindow
{
	string csName = "NewBehaviour";
	string tips = "";
	MessageType tipsType = MessageType.Info;

	bool debug = false;

	// Add menu item named "My Window" to the Window menu
	//% - cmd(ctrl on windows); # - shift
	[MenuItem("Window/GenSpacer %#e")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(Namespacer));
	}
		
	void OnGUI() {
		GUILayout.Label ("namespacer", EditorStyles.boldLabel);
		csName = EditorGUILayout.TextField (".cs name", csName);

		//tips
		if(tips != "") EditorGUILayout.HelpBox (tips, tipsType);

		if (GUILayout.Button ("mkfile", GUILayout.Width (100f))) {
			if (Selection.assetGUIDs.Length == 0) {
				setGUITips ("not selection", MessageType.Warning);
			} else {

				//check csName is reasonable
				var regex = new Regex ("^[a-z|A-Z][\\w]{0,100}$");
				if (!regex.Match (csName).Success) {
					setGUITips ("csName unformat", MessageType.Warning);
				} else {
					foreach (string guid in Selection.assetGUIDs) {
						var assetPath = AssetDatabase.GUIDToAssetPath (guid);
						EditorDebug ("assetPath - " + assetPath);

						var nameSpace = parseNameSpace (assetPath);
						if (nameSpace != null) {
							if (nameSpace == "") {//ignore namespace when result empty string
								setGUITips("NOT support non-namespace yet", MessageType.Warning);
							} else {//1. read template form "Editors/mkScript.tpl"
								string templete = System.IO.File.ReadAllText (Application.dataPath + "/Editors/mkScript.tpl");
								//2. replace #namespace#
								string newTemplete = templete.Replace ("#namespace#", nameSpace).Replace ("#csName#", csName); //todo refine
								EditorDebug ("newTemplete content - " + newTemplete);
								//3. write data into new file at assetPath + "/csName.cs"
								//3.1 ensure file not exist
								var aimPath = Application.dataPath + "/../" + assetPath + "/" + csName + ".cs";
								if (File.Exists (aimPath)) {
									setGUITips ("file has exist", MessageType.Warning);
								} else {
									using (StreamWriter outputFile = new StreamWriter (aimPath)) {
										outputFile.WriteLine (newTemplete);
										setGUITips ("success", MessageType.Info);
									}
									AssetDatabase.Refresh ();
		
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Parses the name space.
	/// </summary>
	/// <returns>The name space.
	/// 	1. null - parse error
	/// 	2. "" - Match .*/Scripts
	/// 	3. others - name.space with dot splited normally
	/// </returns>
	/// <param name="assetPath">Asset path.</param>
	string parseNameSpace(string assetPath) {
		var regex = new Regex (@".*Scripts/(?:([a-z|A-Z|_]*)/)*([a-z|A-Z|_]*)$|.*Scripts");
		var match = regex.Match (assetPath);
		string spaceName = null;

		//all group and capture. Notice(Fucking) the data stucture
//		foreach(Group group in match.Groups) {
//			EditorDebug ("All group item - " + group.Value);
//			foreach (Capture capture in group.Captures) {
//				EditorDebug ("All capture item - " + capture.Value);
//			}
//		}

		if (match.Success == false) {
			setGUITips ("hold in Scripts directory or .cs name format error");
		} else {
			for (int i = 1; i < match.Length; i++) {
				foreach (Capture capture in match.Groups[i].Captures) {
					EditorDebug ("Capture item - " + capture.Value);
					if (spaceName == null) {
						spaceName = capture.Value;
					} else {
						spaceName += ("." + capture.Value);
					}
				}
			}
			if (spaceName == null)
				spaceName = "";
		}

		EditorDebug ("spaceName - " + spaceName);
	
		return spaceName;
	}

	void OnLostFocus() {
		setGUITips("");
	}

	void setGUITips (string tips, MessageType msgTip = MessageType.None) {
		this.tips = tips;
		this.tipsType = msgTip;
	}

	void EditorDebug(string info) {
		if (debug) {
			Debug.Log (info);
		}
	}
}
