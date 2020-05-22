using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Code
{
    public class LevelController: Controller
    {
        public int MapWidth = 50;
        public int MapHeight = 50;
        public int PercentWalls = 25;
        public GameObject WallCubePrefab;
        public GameObject EnemyPrefab;
        public GameObject StartPlate;
        public int EnemyCount = 10;
        
        public static int[,] Map;
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
            StopAllCoroutines();
            
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
            for (var x = 0; x < MapWidth; x++)
            {
                for (var z = 0; z < MapHeight; z++)
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

            StartCoroutine(UpdateRandomEnemyPath());
        }

        public static Vector3 GetRandomFreePoint()
        {
            return m_FreePoints[Random.Range(0, m_FreePoints.Count)];
        }

        // Если осталось мало противников, то может быть сложно их найти. Поэтому они сами нас найдут. 
        private IEnumerator UpdateRandomEnemyPath()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(10);
                var activeEnemy = m_Enemies.First(x => x.activeInHierarchy)?.GetComponent<EnemyController>();
                if (activeEnemy != null)
                    activeEnemy.Path = AStarPathFinder.FindPath(
                        activeEnemy.gameObject.transform.position,
                        m_Player.transform.position);
            }
        }

    }
}