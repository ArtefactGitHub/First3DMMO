using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.InputStick
{
	/// <summary>
	/// スティック入力オブジェクトのインターフェース
	/// </summary>
	public interface IInputStickObject
	{
		Vector2 Vector { get; }
	}

	/// <summary>
	/// スティック入力オブジェクト
	/// 
	/// スティックをドラッグして動かした入力値を正規化したベクトルを取得出来ます。
	/// </summary>
	public class InputStickObject : MonoBehaviour, IInputStickObject, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerUpHandler
	{
		/// <summary> スティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)） </summary>
		public Vector2 Vector { get; private set; }

		[SerializeField]
		private Image m_ImageStick = null;

		/// <summary> スティックの移動可能距離（半径） </summary>
		[SerializeField, Tooltip("スティックの移動可能距離（半径）")]
		private float m_Radius = 200.0f;

		private float m_RadiusHalf = 0f;

		private void Start()
		{
			m_RadiusHalf = m_Radius / 2.0f;
			Vector = Vector2.zero;
		}

		#region EventSystem

		public void OnPointerEnter(PointerEventData eventData)
		{
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Vector3 pos = transform.InverseTransformPoint(eventData.position);

			m_ImageStick.rectTransform.anchoredPosition = pos;
			// 入力ベクトルを正規化
			Vector = pos.normalized;
		}

		/// <summary>
		/// ドラッグ中イベント
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDrag(PointerEventData eventData)
		{
			Vector3 pos = transform.InverseTransformPoint(eventData.position);

			// スティックが半径の範囲を超えないように調整
			pos = new Vector3(Mathf.Clamp(pos.x, -m_RadiusHalf, m_RadiusHalf), Mathf.Clamp(pos.y, -m_RadiusHalf, m_RadiusHalf));
			m_ImageStick.rectTransform.anchoredPosition = pos;

			// 入力ベクトルを正規化
			Vector = pos.normalized;
		}

		/// <summary>
		/// タップ終了イベント
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerUp(PointerEventData eventData)
		{
			// スティックを初期座標へ戻し、入力ベクトルも初期化する
			m_ImageStick.rectTransform.anchoredPosition3D = Vector2.zero;
			Vector = Vector2.zero;
		}

		#endregion
	}
}