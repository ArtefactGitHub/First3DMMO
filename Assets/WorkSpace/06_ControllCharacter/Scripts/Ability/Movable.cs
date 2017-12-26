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
        [SerializeField]
        private float m_Speed = 20f;

        /// <summary> アニメーション管理クラス </summary>
        private APlayerAnimationController m_AnimationController = null;

        private Vector3 m_InputVec = Vector3.zero;

        private Vector3 m_CalcVec = Vector3.zero;

        private Rigidbody m_Rigidbody = null;

        private IDisposable m_Disposable = null;

        private ActionState m_ActionState = ActionState.None;

        public void Initialize(
            GameObject baseObject,
            Rigidbody rigidBody,
            APlayerAnimationController aPlayerAnimationController,
            IObservable<Vector2> moveVectorAsObservable)
        {
            this.m_BaseObject = baseObject;
            this.m_Rigidbody = rigidBody;
            this.m_AnimationController = aPlayerAnimationController;

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
                return;
            }

            if (IsPlayingAttack())
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

            m_Rigidbody.velocity = moveVec * m_Speed;

            if (moveVec != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveVec);
            }

            SetAnimationVelocity(m_InputVec);
        }

        private void MoveStop()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        private void SetAnimationVelocity(Vector2 inputVector)
        {
            var velocity = new Vector3(Mathf.Abs(m_InputVec.x), 0f, Mathf.Abs(m_InputVec.z));
            var inputVelocity = (velocity.x > velocity.z ? velocity.x : velocity.z);

            m_AnimationController.SetMoveVelocity(inputVelocity);
        }

        private bool IsPlayingAttack()
        {
            return (m_ActionState == ActionState.Attack);
        }
    }
}