﻿using UnityEngine;

namespace com.Artefact.First3DMMO.WorkSpace.InputStick
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

		private IObjectSpawnController m_SpawnController = null;

		private void Start()
		{
			m_SpawnController = ObjectSpawnController.Instance;

			for(int i = 0; i < m_SpawnCount; i++)
			{
				m_SpawnController.SpawnRandomArea(m_RootSpawn, m_PrefabSpawn, m_SpawnLeftFar, m_SpawnRightNear);
			}
		}
	}
}