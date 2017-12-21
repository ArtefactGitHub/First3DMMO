using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	/// <summary>
	/// スティック入力オブジェクトのインターフェース
	/// </summary>
	public interface IInputStickObject
	{
        IObservable<Vector2> VectorAsObservable { get; }

		Vector2 Vector { get; }
	}

	/// <summary>
	/// スティック入力オブジェクト
	/// 
	/// スティックをドラッグして動かした入力値を正規化したベクトルを取得出来ます。
	/// </summary>
	public abstract class AInputStickObject : MonoBehaviour, IInputStickObject, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerUpHandler
	{
		/// <summary> スティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)）のストリーム </summary>
        public IObservable<Vector2> VectorAsObservable { get { return _VectorAsObservable.AsObservable(); } }

        private Subject<Vector2> _VectorAsObservable = new Subject<Vector2>();

        /// <summary> スティックの出力ベクトル（(-1.0f, -1.0f)～(1.0f, 1.0f)） </summary>
        public Vector2 Vector { get; protected set; }

		[SerializeField]
        protected Image m_ImageStick = null;

		/// <summary> スティックの移動可能距離（半径） </summary>
		[SerializeField, Tooltip("スティックの移動可能距離（半径）")]
        protected float m_Radius = 100.0f;

        protected Vector2 m_OnPointerDownPosition = Vector2.zero;

        protected virtual void Start()
		{
			Vector = Vector2.zero;

            this.ObserveEveryValueChanged(x => x.Vector).Subscribe(vector =>
            {
                _VectorAsObservable.OnNext(vector);
            }).AddTo(this);

        }

        #region EventSystem

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 pos = transform.InverseTransformPoint(eventData.position);

            m_ImageStick.rectTransform.anchoredPosition = pos;
            // 入力ベクトルを正規化
            UpdateVector(GetNormalizedVectorFromEventData(eventData));
        }

        protected Vector2 GetNormalizedVectorFromEventData(PointerEventData eventData)
        {
            Vector2 pos = transform.InverseTransformPoint(eventData.position);

            // タップ開始座標からの距離を求める
            Vector2 subtract = pos - Vector2.zero;
            // スティック入力範囲上の割合を求める
            Vector2 distance = subtract / m_Radius;

            //// -1.0～1.0f に値を丸め、入力ストリームに流す
            return new Vector3(Mathf.Clamp(distance.x, -1.0f, 1.0f), Mathf.Clamp(distance.y, -1.0f, 1.0f));
        }

        /// <summary>
        /// ドラッグ中イベント
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos = transform.InverseTransformPoint(eventData.position);

            //// タップ開始座標からの距離を求める
            //Vector2 subtract = pos - Vector2.zero;
            //// スティック入力範囲上の割合を求める
            //Vector2 distance = subtract / m_Radius;

            ////// -1.0～1.0f に値を丸め、入力ストリームに流す
            //var vector = new Vector3(Mathf.Clamp(distance.x, -1.0f, 1.0f), Mathf.Clamp(distance.y, -1.0f, 1.0f));
            //UpdateVector(vector);
            UpdateVector(GetNormalizedVectorFromEventData(eventData));


            // スティックが半径の範囲を超えないように調整
            var imagePos = new Vector3(Mathf.Clamp(pos.x, -m_Radius, m_Radius), Mathf.Clamp(pos.y, -m_Radius, m_Radius));
            m_ImageStick.rectTransform.anchoredPosition = imagePos;
        }

        /// <summary>
        /// タップ終了イベント
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            // スティックを初期座標へ戻し、入力ベクトルも初期化する
            m_ImageStick.rectTransform.anchoredPosition3D = Vector2.zero;
            m_OnPointerDownPosition = Vector2.zero;

            UpdateVector(Vector2.zero);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        #endregion

        protected void UpdateVector(Vector2 vector)
        {
            this.Vector = vector;
        }
    }
}