using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
	{
		[SerializeField]
		//private Transform m_Target = null;
		private APlayerController m_Target = null;

        [SerializeField]
        private Vector3 m_PositionOffset = new Vector3(0f, 5.0f, -15.0f);

        [SerializeField]
		private float m_RotateSpeed = 50.0f;

		[SerializeField]
		private float m_RollupSpeed = 20.0f;

        /// <summary> 入力管理クラス </summary>
        private PlayerInputStickManager m_Input = null;

        private Vector3 m_TargetPositionLast = Vector3.zero;

		private Vector3 m_InputVec = Vector3.zero;

        private Camera m_Camera = null;

        private ATargetable m_LockObject { get; set; }

        private void Awake()
        {
            m_Camera = GetComponent<Camera>();
        }

        private void Start()
        {
            // 操作管理クラスのインスタンスの取得
            m_Input = PlayerInputController.Instance.InputStickManager;
            Assert.IsNotNull(m_Input);

            m_TargetPositionLast = m_Target.transform.position;

			// 右（カメラ）スティック入力
			m_Input.OnInputRightStickAsObservable.Subscribe(inputVec =>
			{
				m_InputVec = inputVec;
			}).AddTo(this);

            (this).UpdateAsObservable().Subscribe(_ =>
			{
                if (IsTargetLock())
                {
                    LookAtMoveCamera();
                }
                else
                {
                    MoveCamera();
                }
            }).AddTo(this);

            this.ObserveEveryValueChanged(x => x.m_Target.LockObject).Subscribe(lockObject =>
            {
                m_LockObject = lockObject;
            }).AddTo(this);
		}

        private bool IsTargetLock()
        {
            return (m_LockObject != null);
        }

        private void LookAtMoveCamera()
        {
            var direction = (m_LockObject.transform.position - m_TargetPositionLast).normalized;
            var vec = new Vector3(direction.x * m_PositionOffset.z, m_PositionOffset.y, direction.z * m_PositionOffset.z);
            
            //transform.position = m_TargetPositionLast + vec;
            transform.position = Vector3.Lerp(transform.position, (m_TargetPositionLast + vec), 0.5f);

            m_TargetPositionLast = m_Target.transform.position;

            m_Camera.transform.LookAt((m_LockObject != null ? m_LockObject.transform : null));
        }

        private void MoveCamera()
        {
            transform.position += (m_Target.transform.position - m_TargetPositionLast);

            m_TargetPositionLast = m_Target.transform.position;

            // 左右の回転
            transform.RotateAround(m_TargetPositionLast, Vector3.up, m_InputVec.x * (Time.deltaTime * m_RotateSpeed));

            // 上下の回転
            transform.RotateAround(m_TargetPositionLast, transform.right, m_InputVec.y * (Time.deltaTime * m_RollupSpeed));
        }
    }
}