using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> unusedList = new();
        List<Node> usedList = new();

        Node startingNode = new()
        {
            position = startPos,
            GScore = 0,
            HScore = Vector2.Distance(startPos, endPos)
        }; 

        unusedList.Add(startingNode);
        
        while(unusedList.Count > 0)
        {
            Node lowestFNode = null;
            float lowestFScore = Mathf.Infinity;
            
            foreach (Node node in unusedList)
            {   
                if(node.FScore < lowestFScore) 
                {
                    lowestFScore = node.FScore;
                    lowestFNode = node;
                }
            }

            Node currentNode = lowestFNode;

            if(currentNode.position == endPos)
            {
                List<Vector2Int> path = new();

                while(currentNode != null) 
                {
                    path.Add(currentNode.position);
                    currentNode = currentNode.parent;
                }
                
                path.Reverse();

                return path;
            }
            
            unusedList.Remove(currentNode);
            usedList.Add(currentNode);

            List<Cell> neighboringCells = grid[currentNode.position.x, currentNode.position.y].GetNeighbours(grid);
            foreach (Cell cell in neighboringCells)
            {
                Vector2Int direction = cell.gridPosition - currentNode.position;
                
                Wall currentWall, neighborWall;

                if(direction == Vector2Int.up) 
                {
                    currentWall = Wall.UP;
                    neighborWall = Wall.DOWN;
                }
                else if(direction == Vector2Int.right) 
                {
                    currentWall = Wall.RIGHT;
                    neighborWall = Wall.LEFT;
                }
                else if(direction == Vector2Int.down) 
                {
                    currentWall = Wall.DOWN;
                    neighborWall = Wall.UP;
                }
                else if(direction == Vector2Int.left) 
                {
                    currentWall = Wall.LEFT;
                    neighborWall = Wall.RIGHT;
                }
                else continue;

                if(grid[currentNode.position.x, currentNode.position.y].HasWall(currentWall) || cell.HasWall(neighborWall)) 
                    continue;
                
                Vector2Int neighbourCell = cell.gridPosition;

                foreach (Node node in usedList)
                {
                    if(node.position == neighbourCell) continue;
                    else break;
                }

                float newGScore = currentNode.GScore + Vector2.Distance(currentNode.position, neighbourCell);

                Node neighborNode = null;
                foreach (Node node in unusedList)
                {
                    if(node.position == neighbourCell) 
                    {
                        neighborNode = node;
                    }
                }

                if(neighborNode == null) 
                {
                    neighborNode = new()
                    {
                        position = neighbourCell,
                        parent = currentNode,
                        GScore = newGScore,
                        HScore = Vector2.Distance(neighbourCell, endPos)
                    };

                    unusedList.Add(neighborNode);
                }
                else if(newGScore < neighborNode.GScore)
                {
                    neighborNode.parent = currentNode;
                    neighborNode.GScore = newGScore;
                }
            }
        }
        
        return null;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
