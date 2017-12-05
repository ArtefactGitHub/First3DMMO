using com.Artefact.FrameworkNetwork.Cores;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.SyncPlayer
{
	public class PushMessageManager
	{
		#region singleton
		private static PushMessageManager _Instance = null;

		public static PushMessageManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = new PushMessageManager();
				}
				return _Instance;
			}
		}
		#endregion

		public IObservable<IMessageData> PushMessageAsObservable { get { return m_PushMessageAsObservable.AsObservable(); } }

		private Subject<IMessageData> m_PushMessageAsObservable = new Subject<IMessageData>();

		private List<string> m_PushCommandNames = new List<string>
		{
			"syncPlayer", "syncOtherPlayers", "updateOtherPlayers", "disconnectPlayer",
		};

		public void OnMessage(IMessageData message)
		{
			if(m_PushCommandNames.Any(x => x.Equals(message.CommandName)))
			{
				m_PushMessageAsObservable.OnNext(message);
			}
		}
		
		private void Log(string str)
		{
			Debug.Log(string.Format("<color=green>{0}</color>", str));
		}
	}
}