
using UnityEngine;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public static class SamplePlayerPrefs
	{
		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static int GetInt(string key)
		{
			return PlayerPrefs.GetInt(key);
		}

		public static string GetString(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public static void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}
	}
}