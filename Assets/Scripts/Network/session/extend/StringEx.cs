using System;

public static class ByteArrayEx
{
	public static string GetString(this byte[] bytes)
	{
		return System.Text.Encoding.UTF8.GetString (bytes);
	}
}

//public static class IObservableEx
//{
//	public static string GetString<T>(this IObservable<T> obv)
//	{
//		return System.Text.Encoding.UTF8.GetString (bytes);
//	}
//}

