using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class SearchForTargetable : MonoBehaviour
    {
        public ATargetable Target { get; private set; }

        [SerializeField]
        private GameObject m_BaseObject = null;

        private float m_SearchForTargetableDistanceSqrMagnitude = (50.0f * 50.0f);

        private int m_UpdateIntervalFrame = 10;

        private bool m_IsEnable = false;

        private int m_IntervalFrameCounter = 0;

        private IDisposable m_Disposable = null;

        public void Initialize(float searchDistance, int updateIntervalFrame)
        {
            m_SearchForTargetableDistanceSqrMagnitude = (searchDistance * searchDistance);
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

        private void Exec()
        {
            if (!m_IsEnable)
            {
                return;
            }

            SortList(m_BaseObject);

            Matrix4x4 matrix = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix;

            Target = null;
            foreach (var targetObj in ObjectSpawnController.Instance.Targetables)
            {
                // 以降に有効なターゲットが存在しない場合、処理を終了する
                if (targetObj == null || !CanTargetable(targetObj, m_SearchForTargetableDistanceSqrMagnitude))
                {
                    break;
                }

                // 画面に表示されているオブジェクトが見つかった場合、処理を終了する
                if (IsVisible(targetObj, matrix))
                {
                    Target = targetObj;
                    break;
                }
            }
        }

        private bool IsVisible(ATargetable targetObj, Matrix4x4 matrix)
        {
            var result = false;
            Vector4 pos = matrix * new Vector4(
                targetObj.transform.position.x,
                targetObj.transform.position.y,
                targetObj.transform.position.z,
                1.0f);

            if (pos.w == 0)
            {
                result = true;
            }
            else
            {
                float x = pos.x / pos.w;
                float y = pos.y / pos.w;
                float z = pos.z / pos.w;

                if ((x < -1.0f || x > 1.0f) ||
                    (y < -1.0f || y > 1.0f) ||
                    (z < -1.0f || z > 1.0f))
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        private bool CanTargetable(ATargetable targetObj, float distance)
        {
            float targetDistance = targetObj.SqrMagnitude;
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