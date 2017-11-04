using UnityEngine;
using UnityEngine.UI;

namespace com.Artefact.FrameworkNetwork.Samples
{
	public class SampleErrorManager : MonoBehaviour
	{
		#region singleton
		private static SampleErrorManager _Instance = null;

		public static SampleErrorManager Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = FindObjectOfType<SampleErrorManager>();
				}
				return _Instance;
			}
		}
		#endregion

		[SerializeField]
		private GameObject _ErrorWindow = null;

		[SerializeField]
		private Text _TextMessage = null;

		public void SetMessage(string message)
		{
			_ErrorWindow.SetActive(true);

			_TextMessage.text = message;
		}
	}
}