using System.Collections.Generic;
using UniRx;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class PlayerAnimationController : APlayerAnimationController
    {
        public override IObservable<ActionState> ActionStateAsObservable { get { return m_ActionStateAsObservable.AsObservable(); } }

        private Dictionary<MoveState, float> m_AnimStateMap = new Dictionary<MoveState, float>()
        {
            { MoveState.Run, 0.5f }, { MoveState.Walk, 0.1f }, { MoveState.Wait, 0f },
        };

        private MoveState m_MoveState = MoveState.Wait;

        private ActionState m_ActionState = ActionState.None;

        private Subject<ActionState> m_ActionStateAsObservable = new Subject<ActionState>();

        private void Start()
        {
            this.ObserveEveryValueChanged(x => x.m_ActionState)
                .Subscribe(actionState => m_ActionStateAsObservable.OnNext(actionState)).AddTo(this);
        }

        #region Attack

        public override void PlayAttack()
        {
            if (m_Animator == null)
            {
                return;
            }

            if (m_ActionState != ActionState.Attack)
            {
                m_MoveState = MoveState.Wait;

                m_ActionState = ActionState.Attack;
                m_Animator.CrossFadeInFixedTime(m_ActionState.ToString(), 0f);
            }
        }

        public void EndAttack()
        {
            m_ActionState = ActionState.None;

            m_Animator.CrossFadeInFixedTime(MoveState.Wait.ToString(), 0f);
        }

        #endregion

        #region Move

        public override void SetMoveVelocity(float velocity)
        {
            if (m_Animator == null)
            {
                return;                
            }

            Dictionary<MoveState, float>.KeyCollection keys = m_AnimStateMap.Keys;
            foreach (var key in keys)
            {
                float targetVelocity = m_AnimStateMap[key];
                if (velocity >= targetVelocity)
                {
                    if (m_MoveState != key)
                    {
                        m_Animator.CrossFadeInFixedTime(key.ToString(), 0f);
                        m_MoveState = key;
                    }

                    break;
                }
            }
        }

        #endregion
    }
}