using UniRx;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    /// <summary>
    /// comment
    /// </summary>
    public class Actionable : AbilityComponent
    {
        public IObservable<ActionParameter> ActionParamAsObservable { get { return m_ActionParamAsObservable.AsObservable(); } }

        private Subject<ActionParameter> m_ActionParamAsObservable = new Subject<ActionParameter>();

        private ActionParameter m_ActionParam = new ActionParameter();

        public void Initialize(IObservable<ActionButtonType> actionButtonTypeAsObservable)
        {
            if (actionButtonTypeAsObservable != null)
            {
                actionButtonTypeAsObservable.Subscribe(actionButtonType =>
                {
                    Exec(actionButtonType);
                }).AddTo(this);
            }
        }

        public override void Run()
        {
            m_IsEnable = true;
        }

        public override void Stop()
        {
            m_IsEnable = false;
        }

        private void Exec(ActionButtonType actionButtonType)
        {
            if (!m_IsEnable)
            {
                m_ActionParam.SetActionType(ActionType.Normal);
                m_ActionParamAsObservable.OnNext(m_ActionParam);
                return;
            }

            ActionType actionType = GetActionType(actionButtonType);
            m_ActionParam.SetActionType(actionType);

            m_ActionParamAsObservable.OnNext(m_ActionParam);
        }

        private ActionType GetActionType(ActionButtonType actionButtonType)
        {
            // TODO 仮
            if (actionButtonType == ActionButtonType.Button_1) return ActionType.Attack;
            else if (actionButtonType == ActionButtonType.Button_2) return ActionType.Dash;
            else return ActionType.Normal;
        }

        // 将来的な実装イメージ
        //private ActionType GetActionType(int actionSlotId)
        //{
        //    int actionId = UserDataManager.Instance.GetActionId(actionSlotId);
        //    ActionData actionData = ActionSkillManager.Instance.GetData(actionId);
        //    return actionData.Type;
        //}
    }

    #region ActionParameter

    public enum ActionType
    {
        Normal, Attack, Dash
    }

    public class ActionParameter
    {
        public ActionType ActionType { get; private set; }

        public ActionParameter()
        {
            ActionType = ActionType.Normal;
        }

        public void SetActionType(ActionType actionType)
        {
            ActionType = actionType;
        }
    }

    #endregion
}