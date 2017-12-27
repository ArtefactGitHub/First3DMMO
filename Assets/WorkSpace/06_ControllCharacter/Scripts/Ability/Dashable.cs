using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class Dashable : Movable
    {
        private float m_Distance = 50.0f;

        private float m_DashTimeSecond = 0.3f;

        private Vector3 m_Direction = Vector3.zero;

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

                var moveDirectionVector = CalcMoveDirectionVector(m_Direction, Camera.main);
                m_InputVec = moveDirectionVector * m_Distance;

                m_Progress = 0f;
                m_StartTime = Time.time;

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

            var vec = Vector3.zero;
            if (Time.time < m_StartTime + m_DashTimeSecond)
            {
                m_Progress = (m_DashTimeSecond > 0.0f ? ((Time.time - m_StartTime) / m_DashTimeSecond) : 1.0f);
                vec = Vector3.Slerp(m_InputVec, Vector3.zero, m_Progress);
            }
            else
            {
                m_InputVec = Vector3.zero;
                m_Direction = Vector3.zero;
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