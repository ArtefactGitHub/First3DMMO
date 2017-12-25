using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
	/// <summary>
    /// comment
	/// </summary>
	public class SearchForTargetable : MonoBehaviour
	{
        [SerializeField]
        private GameObject m_BaseObject = null;

        private int m_UpdateIntervalFrame { get; set; }

        private bool m_IsEnable { get; set; }

        private int m_IntervalFrameCounter { get; set; }

        private IDisposable m_Disposable = null;

        public void Initialize(int updateIntervalFrame)
        {
            m_UpdateIntervalFrame = updateIntervalFrame;

            if (m_Disposable != null) m_Disposable.Dispose();
            m_Disposable = this.UpdateAsObservable().Subscribe(_ =>
            {
                if (m_IntervalFrameCounter > m_UpdateIntervalFrame)
                {
                    Exec();

                    m_IntervalFrameCounter = 0;
                }
                else
                {
                    m_IntervalFrameCounter++;
                }
            }).AddTo(this);
        }

        public void Run()
        {
            m_IsEnable = true;
        }

        public void Stop()
        {
            m_IsEnable = false;
        }

        public ATargetable Target { get; private set; }

        private void Exec()
        {
            if (!m_IsEnable)
            {
                return;
            }

            SortList(m_BaseObject);

            var targetObj = ObjectSpawnController.Instance.Targetables[0];
            Target = ((targetObj != null &&
                        CanTargetable(targetObj.SqrMagnitude, GameConfig.SearchForTargetableDistanceSqrMagnitude)) ?
                        targetObj : null);
        }

        private bool CanTargetable(float targetDistance, float distance)
        {
            return (targetDistance < distance);
        }

        private void SortList(GameObject target)
        {
            List<ATargetable> list = ObjectSpawnController.Instance.Targetables;
            foreach (var x in list)
            {
                x.CalcSqrMagnitude(target.transform.position);
            }

            list.Sort((obj1, obj2) => (obj1.SqrMagnitude - obj2.SqrMagnitude < 0f ? -1 : 1));
        }
    }
}