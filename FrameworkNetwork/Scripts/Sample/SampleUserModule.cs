﻿using System;
using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;
using UniRx;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public class SampleUserModule : AModule
	{
		public override string ModuleName { get { return "user"; } }

		public IObservable<IResponseResult<SampleResponseRegister>> Register(string userName, string password)
		{
			JObject data = new JObject();
			data.Add("name", new JValue(userName));
			data.Add("password", new JValue(password));
			
			JObject obj = new JObject();
			obj.Add("data", data);

			return Command<SampleResponseRegister>(obj, "register");
		}

		public IObservable<IResponseResult<SampleResponseLogin>> Login(string userName)
		{
			JObject data = new JObject();
			data.Add("name", new JValue(userName));

			JObject obj = new JObject();
			obj.Add("data", data);

			return Command<SampleResponseLogin>(obj, "login");
		}
	}
}