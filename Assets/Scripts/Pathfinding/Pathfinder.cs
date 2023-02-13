using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Pathfinder
    {
        private static readonly Vector3Int[] _NEARBYDIRS =
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right
        };

        // Update is called once per frame
        public static List<Node> Pathfind(Vector3Int startPos, Vector3Int endPos, int[,] mapData)
        {
            List<Node> openNodes = new();
            List<Node> solvedNodes = new();

            Node activeNode = new(startPos, endPos);
            openNodes.Add(activeNode);

            while (activeNode.Position != endPos)
            {
                foreach (Vector3Int curDir in _NEARBYDIRS)
                {
                    if (!(curDir.x == 0 && curDir.z == 0)
                        && InBounds(activeNode.Position + curDir, mapData))
                    {
                        Node newNode = new(activeNode.Position + curDir, endPos, activeNode);

                        bool addToOpenNodes = true;
                        foreach (Node curNode in solvedNodes)
                        {
                            if (curNode.Position == newNode.Position)
                            {
                                curNode.SimplifyAgainstNode(newNode);
                                addToOpenNodes = false;
                            }
                        }

                        if (addToOpenNodes)
                        {
                            if (mapData[(activeNode.Position + curDir).x, (activeNode.Position + curDir).z] == 0)
                            {
                                openNodes.Add(newNode);
                            }
                        }
                    }
                }
                solvedNodes.Add(activeNode);
                openNodes.Remove(activeNode);

                if (openNodes.Count == 0
                    || openNodes.Count > 1000)
                {
                    break;
                }

                activeNode = FindOptimalNode(openNodes);

                if (activeNode.Position == endPos)
                {
                    return ReverseNodeListAdd(activeNode);
                }
            }

            return ReverseNodeListAdd(activeNode);
        }

        private static List<Node> ReverseNodeListAdd(Node curNode)
        {
            List<Node> nodeList = new();

            if (curNode.PreviousNode != null)
            {
                nodeList = ReverseNodeListAdd(curNode.PreviousNode);
            }

            nodeList.Add(curNode);

            return nodeList;
        }

        private static Node FindOptimalNode(List<Node> openNodes)
        {
            Node activeNode;
            Node optimalNode = openNodes[0];

            foreach (Node curNode in openNodes)
            {
                if (curNode.FCost < optimalNode.FCost)
                {
                    optimalNode = curNode;

                }
                else if (curNode.FCost == optimalNode.FCost)
                {
                    if (curNode.HCost <= optimalNode.HCost)
                    {
                        optimalNode = curNode;
                    }
                }
            }

            activeNode = optimalNode;
            return activeNode;
        }

        private static bool InBounds(Vector3Int position, int[,] mapData)
        {
            return !(position.x < 0 || position.x >= mapData.GetLength(0) || position.z < 0 || position.z >= mapData.GetLength(1));
        }
    }
}