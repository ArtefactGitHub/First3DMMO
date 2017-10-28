using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.ConnectNetwork.Views
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

			if(SamplePlayerPrefs.HasKey(SampleDefine.KeyUserName))
			{
				Login();
			}

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
				Register();
			}).AddTo(this);
		}

		private void Register()
		{
			if(!string.IsNullOrEmpty(_Input.text))
			{
				//TODO 暗号化
				string password = _Input.text.GetHashCode().ToString();

				SampleModuleManager.Instance.Module.Register(_Input.text, password).Subscribe(result =>
				{
					// エラーの場合、エラーメッセージを表示する
					if(result.Exception != null)
					{
						SampleErrorManager.Instance.SetMessage(result.Exception.Message);
						return;
					}

					if(result.Result != null)
					{
						Debug.Log(result.Result.ToString());

						SamplePlayerPrefs.SetString(SampleDefine.KeyUserName, result.Result.UserName);
						SamplePlayerPrefs.SetString(SampleDefine.KeyPassword, password);
					}

					Login();
				}).AddTo(this);
			}
		}

		private void Login()
		{
			string userName = SamplePlayerPrefs.GetString(SampleDefine.KeyUserName);
			string password = SamplePlayerPrefs.GetString(SampleDefine.KeyPassword);

			SampleModuleManager.Instance.Module.Login(userName, password).Subscribe(result =>
			{
				// エラーの場合、エラーメッセージを表示する
				if(result.Exception != null)
				{
					SampleErrorManager.Instance.SetMessage(result.Exception.Message);
					return;
				}

				if(result.Result != null)
				{
					Debug.Log(result.Result.ToString());
				}

				_ProcessEndAsObservable.OnNext(result.Exception);
			}).AddTo(this);
		}
	}
}