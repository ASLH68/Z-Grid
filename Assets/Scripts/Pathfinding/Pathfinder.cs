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
        public static List<Node> Pathfind(Vector3Int startPos, Vector3Int endPos, int[,] mapData, bool allowDestroyWalls, int intelligence)
        {
            List<Node> openNodes = new();
            List<Node> solvedNodes = new();

            Node activeNode = new(startPos, endPos, mapData/*null*/, allowDestroyWalls);
            openNodes.Add(activeNode);

            while (activeNode.Position != endPos)
            {
                foreach (Vector3Int curDir in _NEARBYDIRS)
                {
                    Vector3Int nextPos = activeNode.Position + curDir;

                    if (!(curDir.x == 0 && curDir.z == 0)
                        && MapManager.main.InBounds(nextPos))
                    {
                        //Debug.Log("Checking tile " + nextPos);

                        //Debug.Log(nextPos + " has a value of " + mapData[nextPos.x, nextPos.z]);
                        Node newNode = new(nextPos, endPos, mapData/*null*/, allowDestroyWalls, activeNode);

                        bool addToOpenNodes = true;
                        foreach (Node curNode in solvedNodes)
                        {
                            if (curNode.Position == newNode.Position)
                            {
                                curNode.SimplifyAgainstNode(newNode);
                                addToOpenNodes = false;
                                //Debug.Log("Simplified " + newNode.Position + " to " + newNode.FCost);
                            }
                        }

                        if (addToOpenNodes)
                        {
                            if ((allowDestroyWalls && mapData[nextPos.x, nextPos.z] >= 0)
                                || (!allowDestroyWalls && mapData[nextPos.x, nextPos.z] == 0))
                            {
                                openNodes.Add(newNode);
                                //Debug.Log("Added " + newNode.Position + " to open nodes with a cost of " + newNode.FCost + "");
                            }
                        }
                    }
                }
                solvedNodes.Add(activeNode);
                openNodes.Remove(activeNode);

                if (openNodes.Count == 0
                    || openNodes.Count > Mathf.Max(intelligence, 50))
                {
                    break;
                }

                activeNode = FindOptimalNode(openNodes);
                //Debug.Log("Found optimal node at " + activeNode.Position + " with a cost of " + activeNode.FCost);

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
    }
}