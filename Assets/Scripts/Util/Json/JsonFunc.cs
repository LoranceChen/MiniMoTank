using UnityEngine;
using System.Collections;

public class JsonFunc
{
	public string ToJStr (object obj)
	{
		return JsonUtility.ToJson (obj);
	}

	public T Parse<T>(string jStr) {
		return JsonUtility.FromJson<T> (jStr);
	}
}
