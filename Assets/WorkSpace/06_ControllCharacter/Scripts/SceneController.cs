using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.Artefact.First3DMMO.WorkSpace.ControllCharacter
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_RootSpawn = null;

        [SerializeField]
        private GameObject m_PrefabSpawn = null;

        [SerializeField, Range(1, 100)]
        private int m_SpawnCount = 100;

        [SerializeField]
        private Vector3 m_SpawnLeftFar = new Vector3(-90.0f, 0f, 90.0f);

        [SerializeField]
        private Vector3 m_SpawnRightNear = new Vector3(90.0f, 0f, -90.0f);

        [SerializeField]
        private APlayerController m_Player = null;

        private void Start()
        {
            for (int i = 0; i < m_SpawnCount; i++)
            {
                ObjectSpawnController.Instance.SpawnRandomArea(m_RootSpawn, m_PrefabSpawn, m_SpawnLeftFar, m_SpawnRightNear);
            }

            //m_SpawnController.Spawn(m_RootSpawn, m_PrefabSpawn, new Vector3(10.0f, 0f, 10.0f));
        }

        private bool m_IsInitialized { get; set; }

        private void Update()
        {
            if (!m_IsInitialized)
            {
                Assert.IsNotNull(PlayerInputController.Instance);
                PlayerInputController.Instance.Initialize();

                Assert.IsNotNull(WorldUIManager.Instance);
                WorldUIManager.Instance.Initialize();

                Assert.IsNotNull(m_Player);
                Observable.FromCoroutine(() => m_Player.Initialize(
                    PlayerInputController.Instance.InputStickManager,
                    PlayerInputController.Instance.InputButtonManager,
                    WorldUIManager.Instance))
                    .Subscribe().AddTo(this);

                m_IsInitialized = true;
            }
        }
    }
}