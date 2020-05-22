using System;
using System.Collections.Generic;
using System.Linq;
using Code;
using UnityEngine;
using UnityEngine.Profiling;

public class AStarPathFinder
{
    public static int MaxPathFindingIterations { get; set; } = 200;

    private static Dictionary<PathNode, PathNode> m_Parents;

    public class PathNode : IComparable
    {
        public Vector3 Position { get; set; }
        public int PathLengthFromStart { get; set; }
        public float HeuristicEstimatePathLength { get; set; }
        public float EstimateFullPathLength => PathLengthFromStart + HeuristicEstimatePathLength;

        public int CompareTo(object obj)
        {
            if (Position == ((PathNode) obj).Position)
                return 0;

            var diff = ((PathNode) obj).EstimateFullPathLength - EstimateFullPathLength;
            if (diff < 0)
                return 1;
            return -1;
        }
    }


    public static List<Vector3> FindPath(Vector3 src, Vector3 dst)
    {
        Profiler.BeginSample("FindPath");

        var x0 = Mathf.RoundToInt(src.x);
        var z0 = Mathf.RoundToInt(src.z);
        var x1 = Mathf.RoundToInt(dst.x);
        var z1 = Mathf.RoundToInt(dst.z);
        src = new Vector3(x0, 0.5f, z0);
        dst = new Vector3(x1, 0.5f, z1);

        m_Parents = new Dictionary<PathNode, PathNode>();
        var closedSet = new HashSet<Vector3>();
        var openSet = new SortedSet<PathNode>();
        PathNode startNode = new PathNode()
        {
            Position = src,
            PathLengthFromStart = 0,
            HeuristicEstimatePathLength = GetHeuristicPathLength(src, dst)
        };
        m_Parents[startNode] = null;
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Profiler.BeginSample("OrderBy");
            var currentNode = openSet.Min;
            Profiler.EndSample();

            if (currentNode.Position == dst || closedSet.Count > MaxPathFindingIterations)
            {
                Profiler.EndSample();
                return GetPathForNode(currentNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Position);
            foreach (var neighbourNode in GetNeighbours(currentNode, dst))
            {
                Profiler.BeginSample("ProcessNeighbour");
                Profiler.BeginSample("ClosedCount");
                var alreadyClosed = closedSet.Contains(neighbourNode.Position);
                Profiler.EndSample();

                if (alreadyClosed)
                {
                    Profiler.EndSample();
                    continue;
                }

                Profiler.BeginSample("openNode");
                var openNode = openSet.FirstOrDefault(node =>
                    node.Position == neighbourNode.Position);
                Profiler.EndSample();

                if (openNode == null)
                    openSet.Add(neighbourNode);
                else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                {
                    openSet.Remove(openNode);
                    openSet.Add(neighbourNode);
                    foreach (var pair in m_Parents.Where(x => x.Value == openNode))
                    {
                        m_Parents[pair.Key] = neighbourNode;
                    }
                }

                Profiler.EndSample();
            }
        }

        Profiler.EndSample();
        return null;
    }

    private static List<PathNode> GetNeighbours(PathNode pathNode, Vector3 goal)
    {
        Profiler.BeginSample("GetNeighbours");
        var result = new List<PathNode>();
        Vector3[] neighbourPoints = new Vector3[4];
        neighbourPoints[0] = new Vector3(pathNode.Position.x + 1, 0.5f, pathNode.Position.z);
        neighbourPoints[1] = new Vector3(pathNode.Position.x - 1, 0.5f, pathNode.Position.z);
        neighbourPoints[2] = new Vector3(pathNode.Position.x, 0.5f, pathNode.Position.z + 1);
        neighbourPoints[3] = new Vector3(pathNode.Position.x, 0.5f, pathNode.Position.z - 1);

        foreach (var point in neighbourPoints)
        {
            if (point.x < 0 || point.z >= LevelController.Map.GetLength(0))
                continue;
            if (point.x < 0 || point.z >= LevelController.Map.GetLength(1))
                continue;
            if (LevelController.Map[Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z)] == 1)
                continue;
            var neighbourNode = new PathNode()
            {
                Position = point,
                PathLengthFromStart = pathNode.PathLengthFromStart + 1,
                HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
            };
            m_Parents[neighbourNode] = pathNode;
            result.Add(neighbourNode);
        }

        Profiler.EndSample();
        return result;
    }

    private static List<Vector3> GetPathForNode(PathNode pathNode)
    {
        var result = new List<Vector3>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode.Position);
            currentNode = m_Parents[currentNode];
        }

        result.Reverse();
        return result;
    }

    private static float GetHeuristicPathLength(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }
}
