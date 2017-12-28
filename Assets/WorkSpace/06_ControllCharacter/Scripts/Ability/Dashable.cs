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
        private CalcDashParameter m_DashParam = new CalcDashParameter();

        public void Initialize(
            GameObject baseObject,
            float dashDistance,
            float dashTimeSecond,
            IObservable<Vector2> moveVectorAsObservable)
        {
            this.m_BaseObject = baseObject;

            m_DashParam.Initialize(dashDistance, dashTimeSecond);

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
                // カメラ向きを考慮した移動ベクトルの計算
                m_InputVec = CalcMoveDirectionVector(direction, Camera.main).normalized;

                m_DashParam.Calc(m_BaseObject.transform.position, m_InputVec);

                Run();
            }
        }

        private void Move()
        {
            if (!m_IsEnable || m_InputVec == Vector3.zero)
            {
                //MoveStop();
                return;
            }

            var vec = Vector3.zero;
            var p = m_DashParam;
            if (p.IsProgress())
            {
                p.UpdateProgress(p.DashTimeSecond > 0.0f ? ((Time.time - p.StartTime) / p.DashTimeSecond) : 1.0f);
                vec = Vector3.Lerp(p.MoveVec, Vector3.zero, p.Progress);
            }
            else
            {
                m_InputVec = Vector3.zero;

                m_DashParam.Reset();
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

    #region CalcDashParameter

    public class CalcDashParameter
    {
        public float DashTimeSecond { get; private set; }
        public float DashDistance { get; private set; }
        public Vector3 StartVec { get; private set; }
        public Vector3 MoveVec { get; private set; }
        public float StartTime { get; private set; }
        public float Progress { get; private set; }

        public void Initialize(float dashDistance, float dashTimeSecond)
        {
            this.DashDistance = dashDistance;
            this.DashTimeSecond = dashTimeSecond;
        }

        public void Reset()
        {
            Progress = 0f;
            StartTime = 0f;
            StartVec = Vector3.zero;
            MoveVec = Vector3.zero;
        }

        public bool IsProgress()
        {
            return (Time.time < StartTime + DashTimeSecond);
        }

        public void UpdateProgress(float progress)
        {
            this.Progress = progress;
        }

        public void Calc(Vector3 startVec, Vector3 direction)
        {
            this.StartVec = startVec;

            StartTime = Time.time;
            Progress = 0f;

            MoveVec = direction * DashDistance * (1.0f / DashTimeSecond * 2.0f);
        }

    }

    #endregion
}