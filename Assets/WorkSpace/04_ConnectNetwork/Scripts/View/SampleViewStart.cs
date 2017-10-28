using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.ConnectNetwork.Views
{
	public class SampleViewStart : AView
	{
		public override IObservable<Exception> ProcessEndAsObservable { get { return _ProcessEndAsObservable.AsObservable(); } }

		private Subject<Exception> _ProcessEndAsObservable = new Subject<Exception>();

		[SerializeField]
		private Button _ButtonRun = null;

		[SerializeField]
		private Button _ButtonClearPlayerPrefs = null;

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

			_ButtonClearPlayerPrefs.OnClickAsObservable().Subscribe(_ =>
			{
				SamplePlayerPrefs.DeleteAll();
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
						SampleErrorManager.Instance.SetMessage(ex.Message);
					}

					_ProcessEndAsObservable.OnNext(ex);
				}).AddTo(this);
		}
	}
}