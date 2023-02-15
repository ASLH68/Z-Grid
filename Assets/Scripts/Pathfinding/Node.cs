using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Node
    {
        private Vector3Int _nodePos;
        //Distance from start node
        private int _gCost;
        //Distance from end node
        private int _hCost;
        private int _fCost;
        private Node _previousNode;

        public int GCost => _gCost;
        public int HCost => _hCost;
        public int FCost => _fCost;
        public Node PreviousNode => _previousNode;
        public Vector3Int Position => _nodePos;


        public Node(Vector3Int nodePos, Vector3Int endPos, int[,] weights, bool allowBreakWalls, Node previous = null)
        {
            _previousNode = previous;

            _nodePos = nodePos;

            _gCost = previous == null ? 0 : (previous.GCost + CalculateDistance(previous.Position, weights, allowBreakWalls));
            
            _hCost = CalculateDistance(endPos, weights, allowBreakWalls);

            _fCost = _gCost + _hCost;
        }

        public void SimplifyAgainstNode(Node newNode)
        {
            if (newNode.FCost < _fCost)
            {
                _previousNode = newNode.PreviousNode;

                _gCost = newNode.GCost;
                _hCost = newNode.HCost;
                _fCost = _gCost + _hCost;
            }
        }

        private int CalculateDistance(Vector3Int targetPos, int[,] weights, bool allowBreakWalls)
        {
            Vector3Int curPos = _nodePos;
            int distance = 0;

            while (curPos != targetPos)
            {
                int tempDist = 0;
                if (curPos.x != targetPos.x)
                {
                    curPos += (int)Mathf.Sign(targetPos.x - curPos.x) * Vector3Int.right;
                    tempDist = 10;
                }
                else if (curPos.z != targetPos.z)
                {
                    curPos += (int)Mathf.Sign(targetPos.z - curPos.z) * Vector3Int.forward;
                    //tempDist = tempDist == 10 ? 14 : 10;
                    tempDist = 10;
                }
                if (allowBreakWalls)
                {
                    distance += tempDist * (1 + weights[curPos.x, curPos.z]);
                }
                else
                {
                    distance += tempDist;
                }
            }
            return distance;
        }

        public static int NodeDistance(List<Node> nodes)
        {
            int total = 0;

            foreach (Node curNode in nodes)
            {
                total += curNode.FCost;
            }

            return total;
        }
    }
}