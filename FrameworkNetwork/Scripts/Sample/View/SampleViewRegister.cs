using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.FrameworkNetwork.Samples.Views
{
	public class SampleViewRegister : AView
	{
		public override IObservable<Exception> ProcessEndAsObservable { get { return _ProcessEndAsObservable.AsObservable(); } }

		private Subject<Exception> _ProcessEndAsObservable = new Subject<Exception>();

		[SerializeField]
		private InputField _Input = null;

		[SerializeField]
		private Button _ButtonRun = null;

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
			_Input.OnEndEditAsObservable().Subscribe(text =>
			{
				Debug.Log("UserName : " + text);
			}).AddTo(this);

			_ButtonRun.OnClickAsObservable().Subscribe(_ =>
			{
				Setup();
			}).AddTo(this);
		}

		private void Setup()
		{
			if(!string.IsNullOrEmpty(_Input.text))
			{
				SampleModuleManager.Instance.Module.Register(_Input.text, (ex, res) =>
				{
					// エラーの場合、エラーメッセージを表示する
					if(ex != null)
					{
						SampleErrorManager.Instance.SetMessage(ex.Message);
					}

					_ProcessEndAsObservable.OnNext(ex);
				});
			}
		}
	}
}