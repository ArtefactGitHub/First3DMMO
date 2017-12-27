using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class Dashable : AbilityComponent
    {
        public IObservable<MoveParameter> MoveParamAsObservable { get { return m_MoveParamAsObservable.AsObservable(); } }

        private Subject<MoveParameter> m_MoveParamAsObservable = new Subject<MoveParameter>();

        private MoveParameter m_MoveParam = new MoveParameter();

        private Vector3 m_InputVec = Vector3.zero;

        private float m_Distance = 50.0f;

        private float m_DashTimeSecond = 0.3f;

        private Vector3 m_Direction = Vector3.zero;

        private Vector3 m_CalcVec = Vector3.zero;

        private IDisposable m_Disposable = null;

        public void Initialize(
            GameObject baseObject,
            float distance,
            float dashTimeSecond,
            IObservable<Vector2> moveVectorAsObservable)
        {
            this.m_BaseObject = baseObject;
            this.m_Distance = distance;
            this.m_DashTimeSecond = dashTimeSecond;

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

        public void Play(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                m_Direction = direction;

                Run();
            }
        }

        private float m_Progress = 0f;
        private float m_StartTime = 0f;

        private void Move()
        {
            if (!m_IsEnable || m_Direction == Vector3.zero)
            {
                //MoveStop();
                return;
            }

            if (m_InputVec == Vector3.zero)
            {
                // カメラの進行方向ベクトル
                m_CalcVec.Set(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
                Vector3 cameraForward = m_CalcVec.normalized;

                // カメラと前方ベクトルとの角度を求める
                // 前方ベクトルを向いている場合 0 となる（-180.0f <= 0 <= 180.0f の範囲）
                float angle = Vector3.Angle(Vector3.forward, cameraForward) * (cameraForward.x < 0f ? -1.0f : 1f);

                // 角度分、入力ベクトルを回転させる
                var direction = Quaternion.AngleAxis(angle, Vector3.up) * m_Direction;
                m_InputVec = direction * m_Distance;

                m_Progress = 0f;
                m_StartTime = Time.time;
            }

            var vec = Vector3.zero;
            if (Time.time < m_StartTime + m_DashTimeSecond)
            {
                m_Progress = (m_DashTimeSecond > 0.0f ? ((Time.time - m_StartTime) / m_DashTimeSecond) : 1.0f);
                vec = Vector3.Slerp(m_InputVec, Vector3.zero, m_Progress);
            }
            else
            {
                m_InputVec = Vector3.zero;
            }

            m_MoveParam.SetMoveVector(vec);
            m_MoveParam.SetMoveDirection(m_InputVec);

            m_MoveParamAsObservable.OnNext(m_MoveParam);
        }

        //private void MoveStop()
        //{
        //    m_MoveParam.SetMoveVector(Vector3.zero);
        //    m_MoveParam.SetMoveDirection(Vector3.zero);

        //    m_MoveParamAsObservable.OnNext(m_MoveParam);
        //}
    }
}