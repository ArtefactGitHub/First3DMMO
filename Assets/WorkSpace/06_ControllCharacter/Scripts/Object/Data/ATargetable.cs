using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	/// <summary>
    /// comment
	/// </summary>
	public abstract class ATargetable : MonoBehaviour
	{
        public Vector3 Position { get { return gameObject.transform.position; } }

        public float SqrMagnitude { get; protected set; }

        public virtual void CalcSqrMagnitude(Vector3 targetPosition)
        {
            SqrMagnitude = (Position - targetPosition).sqrMagnitude;
        }
    }
}