using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class Movable : AbilityComponent
    {
        public IObservable<MoveParameter> MoveParamAsObservable { get { return m_MoveParamAsObservable.AsObservable(); } }

        private Subject<MoveParameter> m_MoveParamAsObservable = new Subject<MoveParameter>();

        private MoveParameter m_MoveParam = new MoveParameter();

        private float m_Speed = 20f;

        private Vector3 m_InputVec = Vector3.zero;

        private Vector3 m_CalcVec = Vector3.zero;

        private Rigidbody m_Rigidbody = null;

        private IDisposable m_Disposable = null;

        private ActionState m_ActionState = ActionState.None;

        public void Initialize(
            GameObject baseObject,
            Rigidbody rigidBody,
            float speed,
            IObservable<Vector2> moveVectorAsObservable)
        {
            this.m_BaseObject = baseObject;
            this.m_Rigidbody = rigidBody;
            this.m_Speed = speed;

            if (moveVectorAsObservable != null)
            {
                moveVectorAsObservable.Subscribe(vec =>
                {
                    m_InputVec.x = vec.x;
                    m_InputVec.z = vec.y;
                }).AddTo(this);
            }

            // FixedUpdate()
            if (m_Disposable != null) m_Disposable.Dispose();
            m_Disposable = (this).FixedUpdateAsObservable().Subscribe(_ =>
            {
                Move();
            }).AddTo(this);
        }

        public override void Run()
        {
            m_IsEnable = true;
        }

        public override void Stop()
        {
            m_IsEnable = false;
        }

        private void Move()
        {
            if (!m_IsEnable)
            {
                MoveStop();
                return;
            }

            // カメラの進行方向ベクトル
            m_CalcVec.Set(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
            Vector3 cameraForward = m_CalcVec.normalized;

            // カメラと前方ベクトルとの角度を求める
            // 前方ベクトルを向いている場合 0 となる（-180.0f <= 0 <= 180.0f の範囲）
            float angle = Vector3.Angle(Vector3.forward, cameraForward) * (cameraForward.x < 0f ? -1.0f : 1f);

            // 角度分、入力ベクトルを回転させる
            var moveVec = Quaternion.AngleAxis(angle, Vector3.up) * m_InputVec;

            m_MoveParam.SetMoveVector(moveVec * m_Speed);
            m_MoveParam.SetMoveDirection(moveVec);

            m_MoveParamAsObservable.OnNext(m_MoveParam);
        }

        private void MoveStop()
        {
            m_MoveParam.SetMoveVector(Vector3.zero);
            m_MoveParam.SetMoveDirection(Vector3.zero);

            m_MoveParamAsObservable.OnNext(m_MoveParam);
        }
    }

    #region MoveParameter

    public class MoveParameter
    {
        public Vector3 MoveVector { get; private set; }

        public Vector3 MoveDirection { get; private set; }

        public MoveParameter()
        {
            MoveVector = Vector3.zero;
            MoveDirection = Vector3.zero;
        }

        public void SetMoveVector(Vector3 vector)
        {
            MoveVector = vector;
        }

        public void SetMoveDirection(Vector3 vector)
        {
            MoveDirection = vector;
        }
    }

    #endregion
}