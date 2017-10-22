using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.Artefact.First3DMMO.WorkSpace.InputStickAnywhere
{
	/// <summary>
	/// スティック入力オブジェクトのインターフェース
	/// </summary>
	public interface IInputStickObject
	{
		Vector2 Vector { get; }

		Vector2 CameraVector { get; }

		//Vector2 ClickPosition { get; }
		//IObservable<Vector2> OnClickPositionAsObservable { get; }
	}

	/// <summary>
	/// スティック入力オブジェクト
	/// 
	/// スティックをドラッグして動かした入力値を正規化したベクトルを取得出来ます。
	/// </summary>
	public class InputStickObject : MonoBehaviour, IInputStickObject, IPointerDownHandler, IDragHandler, IPointerClickHandler, IPointerUpHandler
	{
		/// <summary> 移動スティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)） </summary>
#if UNITY_EDITOR
		// Unityエディタ上の PointerId は -1 
		public Vector2 Vector { get { return m_InputDatas[-1].Vector; } }
#else
		public Vector2 Vector { get { return m_InputDatas[(int)InputType.Move].Vector; } }
#endif

		/// <summary> カメラスティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)） </summary>
		public Vector2 CameraVector { get { return m_InputDatas[(int)InputType.Camera].Vector; } }

		//public Vector2 ClickPosition { get; private set; }

		/// <summary> スティックの移動可能距離（半径） </summary>
		[SerializeField, Tooltip("スティックの移動可能距離（半径）")]
		private float m_Radius = 200.0f;

		private float m_RadiusHalf = 0f;

		private Dictionary<int, InputData> m_InputDatas = new Dictionary<int, InputData>();

		private void Start()
		{
			m_RadiusHalf = m_Radius / 2.0f;

			// 入力情報の生成
			int max = Enum.GetValues(typeof(InputType)).Length;
			for(int i = 0; i < max; i++)
			{
				m_InputDatas.Add(i, new InputData(Vector2.zero));
			}

#if UNITY_EDITOR
			m_InputDatas.Add(-1, new InputData(Vector2.zero));
#endif
		}

		#region EventSystem

		public void OnPointerClick(PointerEventData eventData)
		{
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			int pointerId = eventData.pointerId;
			if(IsInputOver(eventData))
			{
				return;
			}

			Vector3 pos = transform.InverseTransformPoint(eventData.position);
			if(m_InputDatas.ContainsKey(pointerId))
			{
				m_InputDatas[pointerId].PressPosition = (pos);
			}

			//Debug.Log(string.Format("down - [{0}]-[{1}]", pointerId, pos));
		}

		/// <summary>
		/// ドラッグ中イベント
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDrag(PointerEventData eventData)
		{
			int pointerId = eventData.pointerId;
			if(!m_InputDatas.ContainsKey(pointerId))
			{
				return;
			}

			Vector2 pos = transform.InverseTransformPoint(eventData.position);

			// 押下開始座標からの距離を求め、スティックが半径の範囲を超えないように調整
			Vector2 subtract = pos - m_InputDatas[pointerId].PressPosition;
			pos = new Vector3(Mathf.Clamp(subtract.x, -m_RadiusHalf, m_RadiusHalf), Mathf.Clamp(subtract.y, -m_RadiusHalf, m_RadiusHalf));

			// 入力ベクトルを正規化
			m_InputDatas[pointerId].Vector = pos.normalized;
			//Debug.Log(string.Format("drag [{0}]-[{1}]{2}", pointerId, pos, m_InputDatas[pointerId]));
		}

		/// <summary>
		/// タップ終了イベント
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerUp(PointerEventData eventData)
		{
			// スティックを初期座標へ戻し、入力ベクトルも初期化する
			int pointerId = eventData.pointerId;
			if(m_InputDatas.ContainsKey(pointerId))
			{
				m_InputDatas[pointerId].Reset();
				//Debug.Log(string.Format("up - [{0}]", pointerId));
			}
		}

		#endregion

		/// <summary>
		/// 入力情報過多か
		/// </summary>
		/// <param name="eventData"></param>
		/// <returns></returns>
		private bool IsInputOver(PointerEventData eventData)
		{
			return (eventData.pointerId >= 2);
		}

		#region InputType

		/// <summary>
		/// 入力種別
		/// </summary>
		private enum InputType
		{
			/// <summary> 移動用 </summary>
			Move,
			/// <summary> カメラ用 </summary>
			Camera,
		}

		#endregion

		#region InputData

		/// <summary>
		/// 入力情報
		/// </summary>
		public class InputData
		{
			public Vector2 PressPosition = Vector2.zero;

			public Vector2 Vector = Vector2.zero;

			public InputData(Vector2 pressPosition)
			{
				this.PressPosition = pressPosition;
			}

			public void Reset()
			{
				this.PressPosition = Vector2.zero;
				this.Vector = Vector2.zero;
			}

			public override string ToString()
			{
				return string.Format("[{0}]\nVector=[{1}]", PressPosition, Vector);
			}
		}

		#endregion
	}
}