using System.Collections.Generic;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	/// <summary>
    /// comment
	/// </summary>
	public class PlayerAnimationController : APlayerAnimationController
    {
        private Dictionary<State, float> m_AnimStateMap = new Dictionary<State, float>()
        {
            { State.Run, 0.5f }, { State.Walk, 0.1f }, { State.Wait, 0f },
        };

        private State m_State = State.Wait;

        public override void SetMoveVelocity(float velocity)
        {
            if (m_Animator != null)
            {
                Dictionary<State, float>.KeyCollection keys = m_AnimStateMap.Keys;
                foreach(var key in keys)
                {
                    float targetVelocity = m_AnimStateMap[key];
                    if (velocity >= targetVelocity)
                    {
                        if (m_State != key)
                        {
                            m_Animator.CrossFadeInFixedTime(key.ToString(), 0.1f);
                            m_State = key;
                        }

                        break;
                    }
                }
            }
        }

        public enum State
        {
            Wait, Walk, Run
        }
    }
}