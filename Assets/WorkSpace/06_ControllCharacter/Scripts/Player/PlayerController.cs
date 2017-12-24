using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public abstract class APlayerController : MonoBehaviour
    {
        public abstract ATargetable LockObject { get; }

        public Vector3 Position { get { return transform.position; } }
    }

    [RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(SearchForTargetable))]
	public class PlayerController : APlayerController
    {
        public override ATargetable LockObject { get { return m_LockObj; } }

        [SerializeField]
		private float m_Speed = 10f;

        /// <summary> 入力管理クラス </summary>
        //[SerializeField]
        //private PlayerInputController m_InputController = null;

        /// <summary> アニメーション管理クラス </summary>
        [SerializeField]
        private APlayerAnimationController m_AnimationController = null;

        /// <summary> 入力管理クラス </summary>
        private PlayerInputStickManager m_InputStick = null;

        /// <summary> 入力管理クラス </summary>
        private PlayerInputButtonManager m_InputButton = null;

        private Vector3 m_InputVec = Vector3.zero;

		private Vector3 m_CalcVec = Vector3.zero;

		private Rigidbody m_Rigidbody { get; set; }

        private SearchForTargetable m_SearchForTargetable { get; set; }

        private bool m_IsLock { get; set; }

        private ATargetable m_LockObj { get; set; }

        private void Awake()
        {
            m_SearchForTargetable = GetComponent<SearchForTargetable>();
            Assert.IsNotNull(m_SearchForTargetable);
        }

        void Start()
        {
            Assert.IsNotNull(PlayerInputController.Instance);
            PlayerInputController.Instance.Initialize();

            // 操作管理クラスのインスタンスの取得
            m_InputStick = PlayerInputController.Instance.InputStickManager;
            m_InputButton = PlayerInputController.Instance.InputButtonManager;
            Assert.IsNotNull(m_InputStick);
            Assert.IsNotNull(m_InputButton);

            // 回転しないようにする
            m_Rigidbody = GetComponent<Rigidbody>();
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // 左スティック入力
            m_InputStick.OnInputLeftStickAsObservable.Subscribe(inputVec =>
			{
				m_InputVec.x = inputVec.x;
				m_InputVec.z = inputVec.y;
			}).AddTo(this);

            // FixedUpdate()
            (this).FixedUpdateAsObservable().Subscribe(_ =>
            {
                Move();
            }).AddTo(this);

            InitializeAbility();           
        }

        #region Initialize

        private void InitializeAbility()
        {
            InitializeSearchForTargetable();

            InitializeTargetLock();
        }

        private void InitializeTargetLock()
        {
            m_InputButton.IsTargetLockAsObservable.Subscribe(isLock =>
            {
                if (isLock)
                {
                    Debug.Log("Lock");

                    if(m_SearchForTargetable.Target != null)
                    {
                        m_LockObj = m_SearchForTargetable.Target;
                    }
                }
                else
                {
                    Debug.Log("not Lock");

                    m_LockObj = null;
                }

                m_IsLock = isLock;
            }).AddTo(this);
        }

        private void InitializeSearchForTargetable()
        {
            // 最近接オブジェクト探索
            m_SearchForTargetable.Initialize(GameConfig.SearchForTargetablePerFrame);
            m_SearchForTargetable.Run();

            this.ObserveEveryValueChanged(x => x.m_SearchForTargetable.Target)
                .Subscribe(target =>
                {
                    if (target == null)
                    {
                        Debug.Log("- targetable is not found");

                        // ターゲットロックが行えなくなる
                        m_InputButton.SetEnableTargetLock(false);
                    }
                    else
                    {
                        Debug.Log("- targetable : " + target.name);

                        // ターゲットロックが行える
                        m_InputButton.SetEnableTargetLock(true);
                    }
                }).AddTo(this);
        }

        #endregion

        private void Move()
        {
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

        private void SetAnimationVelocity(Vector2 inputVector)
        {
            var velocity = new Vector3(Mathf.Abs(m_InputVec.x), 0f, Mathf.Abs(m_InputVec.z));
            var inputVelocity = (velocity.x > velocity.z ? velocity.x : velocity.z);

            m_AnimationController.SetMoveVelocity(inputVelocity);
        }
    }
}