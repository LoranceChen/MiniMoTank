using UnityEngine;
using System.Collections;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class JsonFunc: MonoBehaviour
{
	void Start(){
//		JObject o = JObject.Parse(@"{
//		   'Stores': [
//		     'Lambton Quay',
//		     'Willis Street'
//		   ],
//		   'Manufacturers': [
//		     {
//		       'Name': 'Acme Co',
//		       'Products': [
//		        {
//		          'Name': 'Anvil',
//		          'Price': 50
//		        }
//		      ]
//		    },
//		    {
//		      'Name': 'Contoso',
//		      'Products': [
//		        {
//		          'Name': 'Elbow Grease',
//		          'Price': 99.95
//		        },
//		        {
//		          'Name': 'Headlight Fluid',
//		          'Price': 4
//		        }
//		      ]
//		    }
//		  ]
//		}");
//
//		IList<string> storeNames = o.SelectToken("Stores").Select(s => (string)s).ToList();
//		// Lambton Quay
//		// Willis Street
//
//		IList<string> firstProductNames = o["Manufacturers"].Select(m => (string)m.SelectToken("Products[1].Name")).ToList();
//		// null
//		// Headlight Fluid
//
//		decimal totalPrice = o["Manufacturers"].Sum(m => (decimal)m.SelectToken("Products[0].Price"));
//		// 149.95
//
	}
	public string ToJStr (object obj)
	{
		return JsonUtility.ToJson (obj);
	}

	public T Parse<T>(string jStr) {
		return JsonUtility.FromJson<T> (jStr);
	}

}
//public abstract class JValue{
////	public JValue \ (){
////		return JValue
////	}
//
//}
//	