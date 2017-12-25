using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public abstract class APlayerController : MonoBehaviour
    {
        public abstract ATargetable LockObject { get; }

        public Vector3 Position { get { return transform.position; } }

        public abstract IEnumerator Initialize(
            IPlayerInputStickManager inputStickManager,
            IPlayerInputButtonManager inputButtonManager,
            WorldUIManager worldUI);
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SearchForTargetable))]
    public class PlayerController : APlayerController
    {
        #region property

        public override ATargetable LockObject { get { return m_LockObj; } }

        [SerializeField]
        private float m_Speed = 10f;

        /// <summary> アニメーション管理クラス </summary>
        [SerializeField]
        private APlayerAnimationController m_AnimationController = null;

        /// <summary> 入力管理クラス </summary>
        private IPlayerInputStickManager m_InputStick = null;

        /// <summary> 入力管理クラス </summary>
        private IPlayerInputButtonManager m_InputButton = null;

        private WorldUIManager m_WorldUI = null;

        private Vector3 m_InputVec = Vector3.zero;

        private Vector3 m_CalcVec = Vector3.zero;

        private Rigidbody m_Rigidbody { get; set; }

        private SearchForTargetable m_SearchForTargetable { get; set; }

        private bool m_IsLock { get; set; }

        private ATargetable m_LockObj { get; set; }

        #endregion

        private void Awake()
        {
            m_SearchForTargetable = GetComponent<SearchForTargetable>();
            Assert.IsNotNull(m_SearchForTargetable);

            m_Rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_Rigidbody);
            // 回転しないようにする
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        #region Initialize

        public override IEnumerator Initialize(
            IPlayerInputStickManager inputStickManager,
            IPlayerInputButtonManager inputButtonManager,
            WorldUIManager worldUI)
        {
            // 操作管理クラスのインスタンスの取得
            m_InputStick = inputStickManager;
            Assert.IsNotNull(m_InputStick);

            m_InputButton = inputButtonManager;
            Assert.IsNotNull(m_InputButton);

            m_WorldUI = worldUI;
            Assert.IsNotNull(m_WorldUI);

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

            // アクションボタン入力
            m_InputButton.OnClickActionButton.Subscribe((ActionButtonType buttonType) =>
            {
                OnClickActionButton(buttonType);
            }).AddTo(this);

            InitializeAbility();

            yield break;
        }

        #region Initialize - inner

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

                    if (m_SearchForTargetable.Target != null)
                    {
                        m_LockObj = m_SearchForTargetable.Target;

                        // ターゲットマーカーを有効にする
                        m_WorldUI.SetLockMarker(m_LockObj);
                    }
                }
                else
                {
                    Debug.Log("not Lock");

                    m_LockObj = null;

                    // ターゲットマーカーを外す
                    m_WorldUI.SetLockMarker(null);
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

                        // ターゲットロックを行えないようにする
                        m_InputButton.SetEnableTargetLock(false);

                        // ターゲットマーカーを外す
                        m_WorldUI.SetTargetMarker(null);
                    }
                    else
                    {
                        Debug.Log("- targetable : " + target.name);

                        // ターゲットロックが行える
                        m_InputButton.SetEnableTargetLock(true);

                        // ターゲットマーカーを有効にする
                        m_WorldUI.SetTargetMarker(target);
                    }
                }).AddTo(this);
        }

        #endregion

        #endregion

        private void OnClickActionButton(ActionButtonType buttonType)
        {
            Debug.LogFormat("Action : {0}", buttonType.ToString());
        }

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