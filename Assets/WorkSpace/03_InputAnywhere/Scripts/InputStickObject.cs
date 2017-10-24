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
		/// <summary> 移動スティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)）のストリーム </summary>
		IObservable<Vector2> OnInputLeftStickAsObservable { get; }

		/// <summary> カメラスティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)）のストリーム </summary>
		IObservable<Vector2> OnInputRightStickAsObservable { get; }

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
		/// <summary> 移動スティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)）のストリーム </summary>
		public IObservable<Vector2> OnInputLeftStickAsObservable { get { return _OnInputLeftStickAsObservable.AsObservable(); } }

		private Subject<Vector2> _OnInputLeftStickAsObservable = new Subject<Vector2>();

		/// <summary> カメラスティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)）のストリーム </summary>
		public IObservable<Vector2> OnInputRightStickAsObservable { get { return _OnInputRightStickAsObservable.AsObservable(); } }

		private Subject<Vector2> _OnInputRightStickAsObservable = new Subject<Vector2>();

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
			// マウスの左ボタン、右ボタン用
			m_InputDatas.Add(-1, new InputData(Vector2.zero));
			m_InputDatas.Add(-2, new InputData(Vector2.zero));
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

			// タップ座標を取得
			Vector3 pos = transform.InverseTransformPoint(eventData.position);
			if(m_InputDatas.ContainsKey(pointerId))
			{
				m_InputDatas[pointerId].PressPosition = pos;
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

			// タップ開始座標からの距離を求め、スティックが半径の範囲を超えないように調整
			Vector2 subtract = pos - m_InputDatas[pointerId].PressPosition;
			pos = new Vector3(Mathf.Clamp(subtract.x, -m_RadiusHalf, m_RadiusHalf), Mathf.Clamp(subtract.y, -m_RadiusHalf, m_RadiusHalf));

			// 入力ベクトルを正規化し、入力ストリームに流す
			UpdateStream(pointerId, pos.normalized);
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

				// 入力ストリームに流す
				UpdateStream(pointerId, Vector2.zero);
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

		private void UpdateStream(int pointerId, Vector2 vec)
		{
			if(pointerId == (int)InputType.Move)
			{
				_OnInputLeftStickAsObservable.OnNext(vec);
			}
			else if(pointerId == (int)InputType.Camera)
			{
				_OnInputRightStickAsObservable.OnNext(vec);
			}

#if UNITY_EDITOR
			if(pointerId == -1)
			{
				_OnInputLeftStickAsObservable.OnNext(vec);
			}
			else if(pointerId == -2)
			{
				_OnInputRightStickAsObservable.OnNext(vec);
			}
#endif
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

			public InputData(Vector2 pressPosition)
			{
				this.PressPosition = pressPosition;
			}

			public void Reset()
			{
				this.PressPosition = Vector2.zero;
			}

			public override string ToString()
			{
				return string.Format("PressPosition=[{0}]", PressPosition);
			}
		}

		#endregion
	}
}