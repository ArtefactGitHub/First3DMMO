using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	/// <summary>
    /// comment
	/// </summary>
	public abstract class APlayerAnimationController : MonoBehaviour
	{
        [SerializeField]
        protected Animator m_Animator = null;

        public abstract void SetMoveVelocity(float velocity);

        public abstract void PlayAttack();
    }
}