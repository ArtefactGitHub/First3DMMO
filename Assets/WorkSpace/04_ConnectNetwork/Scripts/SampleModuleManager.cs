using com.Artefact.FrameworkNetwork.Cores;
using Newtonsoft.Json.Linq;
using System;

namespace com.Artefact.First3DMMO.WorkSpace.ConnectNetwork
{
	public class SampleModuleManager
	{
		#region singleton
		private static SampleModuleManager _Instance = null;

		public static SampleModuleManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = new SampleModuleManager();
				}
				return _Instance;
			}
		}

		private SampleModuleManager()
		{
			Module = new SampleUserModule();
		}
		#endregion

		public SampleUserModule Module { get; private set; }
	}
}