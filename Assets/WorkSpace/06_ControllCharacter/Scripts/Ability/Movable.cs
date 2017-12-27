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

        protected Subject<MoveParameter> m_MoveParamAsObservable = new Subject<MoveParameter>();

        protected MoveParameter m_MoveParam = new MoveParameter();

        protected float m_Speed = 20f;

        protected Vector3 m_InputVec = Vector3.zero;

        protected Vector3 m_CalcVec = Vector3.zero;

        protected IDisposable m_Disposable = null;

        public void Initialize(
            GameObject baseObject,
            float speed,
            IObservable<Vector2> moveVectorAsObservable)
        {
            this.m_BaseObject = baseObject;
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

            MoveStop();
        }

        private void Move()
        {
            if (!m_IsEnable)
            {
                //MoveStop();
                return;
            }

            Vector3 moveDirectionVector = CalcMoveDirectionVector(m_InputVec, Camera.main);
            UpdateMoveParameter((moveDirectionVector * m_Speed), moveDirectionVector);
        }

        private void MoveStop()
        {
            UpdateMoveParameter(Vector3.zero, Vector3.zero);
        }

        protected Vector3 CalcMoveDirectionVector(Vector3 inputDirectionVector, Camera camera)
        {
            // カメラの進行方向ベクトル
            m_CalcVec.Set(camera.transform.forward.x, 0f, camera.transform.forward.z);
            Vector3 cameraForward = m_CalcVec.normalized;

            // カメラと前方ベクトルとの角度を求める
            // 前方ベクトルを向いている場合 0 となる（-180.0f <= 0 <= 180.0f の範囲）
            float angle = Vector3.Angle(Vector3.forward, cameraForward) * (cameraForward.x < 0f ? -1.0f : 1f);

            // 角度分、入力ベクトルを回転させる
            var moveVec = Quaternion.AngleAxis(angle, Vector3.up) * inputDirectionVector;
            return moveVec;
        }

        protected void UpdateMoveParameter(Vector3 moveVec, Vector3 moveDirection)
        {
            m_MoveParam.SetMoveVector(moveVec);
            m_MoveParam.SetMoveDirection(moveDirection);

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