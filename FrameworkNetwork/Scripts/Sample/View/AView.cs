using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace com.Artefact.FrameworkNetwork.Samples.Views
{
	public abstract class AView : MonoBehaviour
	{
		public abstract IObservable<Exception> ProcessEndAsObservable { get; }

		public IEnumerator Initialize()
		{
			yield return _Initialize();

			RegisterEvent();
		}

		public abstract void Finalizer();

		protected virtual IEnumerator _Initialize()
		{
			yield break;
		}

		protected abstract void RegisterEvent();
	}
}