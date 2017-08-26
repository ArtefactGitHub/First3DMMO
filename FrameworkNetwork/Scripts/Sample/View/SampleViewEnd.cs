using System;
using System.Collections;
using UniRx;

namespace com.Artefact.FrameworkNetwork.Samples.Views
{
	public class SampleViewEnd : AView
	{
		public override IObservable<Exception> ProcessEndAsObservable { get { return _ProcessEndAsObservable.AsObservable(); } }
		
		private Subject<Exception> _ProcessEndAsObservable = new Subject<Exception>();

		public override void Finalizer()
		{
			gameObject.SetActive(false);
		}

		protected override IEnumerator _Initialize()
		{
			gameObject.SetActive(true);
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