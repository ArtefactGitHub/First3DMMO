using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.ConnectNetwork.Views
{
	public class SampleViewEnd : AView
	{
		public override IObservable<Exception> ProcessEndAsObservable { get { return _ProcessEndAsObservable.AsObservable(); } }
		
		private Subject<Exception> _ProcessEndAsObservable = new Subject<Exception>();

		[SerializeField]
		private Text _TextUserName = null;

		public override void Finalizer()
		{
			gameObject.SetActive(false);
		}

		protected override IEnumerator _Initialize()
		{
			gameObject.SetActive(true);

			_TextUserName.text = SamplePlayerPrefs.GetString(SampleDefine.KeyUserName);

			yield break;
		}

		protected override void RegisterEvent()
		{
		}

		private void Setup()
		{
		}
	}
}