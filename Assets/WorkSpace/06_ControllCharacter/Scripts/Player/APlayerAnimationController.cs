using UniRx;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public abstract class APlayerAnimationController : MonoBehaviour
	{
        public abstract IObservable<ActionState> ActionStateAsObservable { get; }

        [SerializeField]
        protected Animator m_Animator = null;

        public abstract void SetMoveVelocity(float velocity);

        public abstract void PlayAttack();
    }

    public enum ActionState
    {
        None, Attack
    }

    public enum MoveState
    {
        Wait, Walk, Run
    }
}