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

		[SerializeField]
		private bool IsPlayerPrefsClear = true;

		private void Start()
		{
			if(IsPlayerPrefsClear)
			{
				SamplePlayerPrefs.DeleteAll();
			}

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
	}
}