using System.Collections;
using UniRx;
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
    [RequireComponent(typeof(Movable))]
    [RequireComponent(typeof(Actionable))]
    public class PlayerController : APlayerController
    {
        #region property

        public override ATargetable LockObject { get { return m_LockObj; } }

        [SerializeField]
        private float m_Speed = 20f;

        [SerializeField]
        private float m_DashDistance = 50f;

        [SerializeField]
        private float m_DashTimeSecond = 0.3f;

        /// <summary> アニメーション管理クラス </summary>
        [SerializeField]
        private APlayerAnimationController m_AnimationController = null;

        [SerializeField]
        private Rigidbody m_Rigidbody = null;

        /// <summary> 入力管理クラス </summary>
        private IPlayerInputStickManager m_InputStick = null;

        /// <summary> 入力管理クラス </summary>
        private IPlayerInputButtonManager m_InputButton = null;

        private WorldUIManager m_WorldUI = null;

        private Vector3 m_InputVec = Vector3.zero;

        private SearchForTargetable m_SearchForTargetable = null;

        private Dashable m_Dashable = null;

        private Movable m_Movable = null;

        private Actionable m_Actionable = null;

        private bool m_IsLock = false;

        private ATargetable m_LockObj = null;

        #endregion

        #region Awake

        private void Awake()
        {
            m_SearchForTargetable = GetComponent<SearchForTargetable>();
            Assert.IsNotNull(m_SearchForTargetable);

            m_Dashable = GetComponent<Dashable>();
            Assert.IsNotNull(m_Dashable);

            m_Movable = GetComponent<Movable>();
            Assert.IsNotNull(m_Movable);

            m_Actionable = GetComponent<Actionable>();
            Assert.IsNotNull(m_Actionable);

            // 回転しないようにする
            Assert.IsNotNull(m_Rigidbody);
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            Assert.IsNotNull(m_AnimationController);
        }

        #endregion

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

            m_AnimationController.ActionStateAsObservable.Subscribe(actionState =>
            {
                m_ActionState = actionState;
            }).AddTo(this);

            InitializeAbility();

            yield break;
        }

        #region Initialize - inner

        private void InitializeAbility()
        {
            InitializeDasable();

            InitializeMovable();

            InitializeActionable();

            InitializeSearchForTargetable();

            InitializeTargetLock();
        }

        #region Action

        private ActionState m_ActionState { get; set; }

        private void InitializeActionable()
        {
            // 初期化
            m_Actionable.Initialize(m_InputButton.OnClickActionButton);

            m_Actionable.ActionParamAsObservable.Subscribe(param =>
            {
                OnClickActionButton(param);
            }).AddTo(this);

            m_Actionable.Run();
        }

        private bool IsPlayingAttack()
        {
            return (m_ActionState == ActionState.Attack);
        }

        private void OnClickActionButton(ActionParameter actionParameter)
        {
            if (actionParameter.ActionType == ActionType.Attack)
            {
                m_AnimationController.PlayAction(ActionState.Attack);
            }
            else if (actionParameter.ActionType == ActionType.Dash)
            {
                m_Dashable.Play(m_InputVec);
            }
        }

        #endregion

        #region Dasable

        private bool IsPlayingDash()
        {
            return m_Dashable.IsEnable;
        }

        private void InitializeDasable()
        {
            // 初期化
            m_Dashable.Initialize(
                gameObject,
                m_DashDistance,
                m_DashTimeSecond,
                m_InputStick.OnInputLeftStickAsObservable);

            // 移動パラメータのストリーム
            m_Dashable.MoveParamAsObservable.Subscribe(param =>
            {
                if (param.MoveVector == Vector3.zero)
                {
                    m_Dashable.Stop();

                    m_AnimationController.StopAction();
                }
                else
                {
                    m_Rigidbody.velocity = param.MoveVector;

                    if (param.MoveDirection != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(param.MoveDirection);
                    }
                }

                if (m_ActionState != ActionState.Dash)
                {
                    m_AnimationController.PlayAction(ActionState.Dash);
                }
            }).AddTo(this);

            // コンポーネント有効化制御
            this.ObserveEveryValueChanged(x => x.IsPlayingAttack()).Subscribe(isPlayingAttack =>
            {
                if (isPlayingAttack) m_Dashable.Stop();
                //else m_Dashable.Run();
            }).AddTo(this);

            //m_Dashable.Run();
        }

        #endregion

        #region Movable

        private void InitializeMovable()
        {
            // 初期化
            m_Movable.Initialize(
                gameObject,
                m_Speed,
                m_InputStick.OnInputLeftStickAsObservable);

            // 移動パラメータのストリーム
            m_Movable.MoveParamAsObservable.Subscribe(param =>
            {
                m_Rigidbody.velocity = param.MoveVector;

                if (param.MoveDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(param.MoveDirection);
                }

                SetMoveVelocityForAnimation(param.MoveDirection);
            }).AddTo(this);

            // コンポーネント有効化制御
            this.ObserveEveryValueChanged(x => x.CanMove()).Subscribe(canMove =>
            {
                if (canMove) m_Movable.Run();
                else m_Movable.Stop();
            }).AddTo(this);

            m_Movable.Run();
        }

        private bool CanMove()
        {
            return (!IsPlayingDash() && !IsPlayingAttack());
        }

        private void SetMoveVelocityForAnimation(Vector2 inputVector)
        {
            var velocity = new Vector3(Mathf.Abs(inputVector.x), 0f, Mathf.Abs(inputVector.y));
            var inputVelocity = (velocity.x > velocity.z ? velocity.x : velocity.z);

            m_AnimationController.SetMoveVelocity(inputVelocity);
        }

        #endregion

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
    }
}