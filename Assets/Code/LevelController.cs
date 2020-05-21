using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;


namespace Code
{
    public class LevelController: MonoBehaviour
    {
        public static bool IsLevelReady;

        public int MapWidth = 50;
        public int MapHeight = 50;
        public int PercentWalls = 25;
        public GameObject WallCubePrefab;
        public GameObject EnemyPrefab;
        public GameObject StartPlate;
        public int EnemyCount = 10;
        private static int[,] Map;

        private static bool m_FoundPathInThisFrame;

        private static List<Vector3> m_FreePoints = new List<Vector3>();
        private List<GameObject> m_Enemies;
        private List<GameObject> m_Walls;

        private GameObject m_Player;

        void Start()
        {
            m_Player = FindObjectOfType<PlayerControllerV2>()?.gameObject;
            ResetLevel();
        }

        public void ResetLevel()
        {
            IsLevelReady = false;
            if (m_Walls == null)
            {
                var maxWallsCount = MapWidth * MapHeight;
                m_Walls = new List<GameObject>();
                for (int i = 0; i < maxWallsCount; i++)
                {
                    m_Walls.Add(Instantiate(WallCubePrefab, new Vector3(0, 0.5f, 0), Quaternion.identity));
                }
            }
            else
            {
                foreach (var wall in m_Walls)
                {
                    wall.SetActive(false);
                }
            }
            
            if (m_Enemies == null)
            {
                m_Enemies = new List<GameObject>();
                for (int i = 0; i < EnemyCount; i++)
                {
                    m_Enemies.Add(Instantiate(EnemyPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity));
                }
            }
            else
            {
                foreach (var e in m_Enemies)
                {
                    e.SetActive(false);
                }
            }


            var walls = m_Walls.GetEnumerator();
            walls.MoveNext();
            Map = new MapGenerator(MapWidth, MapHeight, PercentWalls).Map;
            for (var x = 0; x < 50; x++)
            {
                for (var z = 0; z < 50; z++)
                {
                    if (Map[x, z] == 1)
                    {
                        walls.Current.gameObject.transform.position = new Vector3(x, 0.5f, z);
                        walls.Current.SetActive(true);
                        walls.MoveNext();
                    }
                    else
                    {
                        m_FreePoints.Add(new Vector3(x, 0.5f, z));
                    }
                }
            }

            StartPlate.transform.position = new Vector3(m_FreePoints[0].x, 0.1f, m_FreePoints[0].z);
            m_Player.transform.position = new Vector3(m_FreePoints[0].x, 0.5f, m_FreePoints[0].z);
            var enemies = m_Enemies.GetEnumerator();
            enemies.MoveNext();
            for (var i = 0; i < EnemyCount; i++)
            {
                while (true)
                {
                    var enemyPosition = GetRandomFreePoint();
                    if(Vector3.Distance(enemyPosition, StartPlate.transform.position) < 20)
                        continue;
                    enemies.Current.gameObject.transform.position = enemyPosition;
                    enemies.Current.SetActive(true);
                    enemies.MoveNext();
                    break;
                }
            }

            IsLevelReady = true;
        }


        public static Vector3 GetRandomFreePoint()
        {
            return m_FreePoints[Random.Range(0, m_FreePoints.Count)];
        }

        private void LateUpdate()
        {
            m_FoundPathInThisFrame = false;
        }
    }
}