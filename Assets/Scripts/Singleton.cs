using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour , IDisposable
{
    public static void ResetShutDown()
	{
		Singleton<T>.m_ShuttingDown = false;
	}

	public static T Instance
	{
		get
		{
			T result;
			if (Singleton<T>.m_ShuttingDown)
			{
				string str = "[Singleton] Instance '";
				Type typeFromHandle = typeof(T);
				Debug.LogWarning(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "' already destroyed. Returning null.");
				result = default(T);
				return result;
			}
			object @lock = Singleton<T>.m_Lock;
			lock (@lock)
			{
				if (Singleton<T>.m_Instance == null)
				{
					Singleton<T>.m_Instance = (T)((object)UnityEngine.Object.FindFirstObjectByType(typeof(T)));
					if (Singleton<T>.m_Instance == null)
					{
						GameObject gameObject = new GameObject();
						Singleton<T>.m_Instance = gameObject.AddComponent<T>();
						gameObject.name = typeof(T).ToString() + " (Singleton)";
					}
				}
				result = Singleton<T>.m_Instance;
			}
			return result;
		}
	}

	private void OnApplicationQuit()
	{
		Singleton<T>.m_ShuttingDown = true;
	}

	private void OnDestroy()
	{
		Singleton<T>.m_ShuttingDown = true;
	}

	private static bool m_ShuttingDown = false;
	private static object m_Lock = new object();
	private static T m_Instance;
}
