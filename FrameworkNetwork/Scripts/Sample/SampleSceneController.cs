using com.Artefact.FrameworkNetwork.Samples.Views;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public class SampleSceneController : MonoBehaviour
	{
		[SerializeField]
		private List<AView> _Views = new List<AView>();

		private void Start()
		{
			Observable.FromCoroutine(() => Run()).Subscribe().AddTo(this);
		}

		private IEnumerator Run()
		{
			foreach(var view in _Views)
			{
				bool isEnd = false;

				view.ProcessEndAsObservable.Subscribe(ex =>
				{
					if(ex != null)
					{
						Debug.Log("Error : " + ex.Message);
						return;
					}

					view.Finalizer();

					isEnd = true;
				}).AddTo(this);

				yield return view.Initialize();

				while(!isEnd) yield return null;
			}
			yield break;
		}

		#region UserId
		public static readonly string KeyUserId = "UserId";
		public static readonly string KeyUserName = "UserName";

		public int UserId
		{
			get
			{
				if(PlayerPrefs.HasKey(KeyUserId))
				{
					return PlayerPrefs.GetInt(KeyUserId, 0);
				}
				else
				{
					return 0;
				}
			}
		}
		public string UserName
		{
			get
			{
				if(PlayerPrefs.HasKey(KeyUserName))
				{
					return PlayerPrefs.GetString(KeyUserName, string.Empty);
				}
				else
				{
					return string.Empty;
				}
			}
		}
		#endregion
	}
}