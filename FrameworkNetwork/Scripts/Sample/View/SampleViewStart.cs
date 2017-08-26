using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.FrameworkNetwork.Samples.Views
{
	public class SampleViewStart : AView
	{
		public override IObservable<Exception> ProcessEndAsObservable { get { return _ProcessEndAsObservable.AsObservable(); } }

		private Subject<Exception> _ProcessEndAsObservable = new Subject<Exception>();

		[SerializeField]
		private Button _ButtonRun = null;

		[SerializeField]
		private Text _TextStatusValue = null;

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
			_ButtonRun.OnClickAsObservable().Subscribe(_ =>
			{
				Setup();
			}).AddTo(this);
		}

		private void Setup()
		{
			_TextStatusValue.text = "接続中";
			_ButtonRun.interactable = false;

			Observable.FromCoroutine<Exception>(observer => SampleNetworkManager.Instance.Initialize(observer, SampleDefine.EndPoint))
				.Subscribe(ex =>
				{
					// エラーの場合、エラーメッセージを表示する
					if(ex != null)
					{
						_TextStatusValue.text = string.Format("<color=red>{0}</color>", ex.Message);
					}

					_ProcessEndAsObservable.OnNext(ex);
				}).AddTo(this);
		}
	}
}