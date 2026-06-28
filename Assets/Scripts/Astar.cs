using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openNodes = new();
        List<Node> closedNodes = new();
        
        Node startingNode = new()
        {
            position = startPos,
            GScore = 0,
            HScore = Vector2.Distance(startPos, endPos)
        };

        openNodes.Add(startingNode);

        while(openNodes.Count > 0)
        {
            // Fetch lowest F score node
            Node currentNode = FindLowesFScore(openNodes);

            if(currentNode.position == endPos) return ReconstructPath(currentNode);

            // Update both lists 
            closedNodes.Add(currentNode);
            openNodes.Remove(currentNode);

            // Fetch neighbor cells
            List<Cell> neighborCells = grid[currentNode.position.x, currentNode.position.y].GetNeighbours(grid);
            foreach (Cell cell in neighborCells)
            {
                // Calculate the distance between the neighbor cell and the current cell
                Vector2Int direction = cell.gridPosition - currentNode.position;
                Wall currentWall, neighborWall;

                // Map the direction
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

                if(grid[currentNode.position.x, currentNode.position.y].HasWall(currentWall) || cell.HasWall(neighborWall)) continue;

                // Store the position of the cell
                Vector2Int neighborCell = cell.gridPosition;

                // Continue if node already exists
                if(IsNodeInList(closedNodes, neighborCell)) continue;

                // Calculate G score
                float newGscore = currentNode.GScore + Vector2.Distance(currentNode.position, neighborCell);

                // Fetch the neighbor node
                Node neighborNode = FetchNode(openNodes, neighborCell);

                if(neighborNode == null)
                {
                    neighborNode = new Node()
                    {
                        position = neighborCell,
                        parent = currentNode,
                        GScore = newGscore,
                        HScore = Vector2.Distance(neighborCell, endPos)  
                    };

                    openNodes.Add(neighborNode);
                }
                else if(newGscore < neighborNode.GScore)
                {
                    neighborNode.parent = currentNode;
                    neighborNode.GScore = newGscore;
                }
            }   
        }

        return null;
    }

    private static bool IsNodeInList(List<Node> nodes, Vector2Int position)
    {
        foreach (Node node in nodes)
            if(node.position == position)
                return true;

        return false;
    }

    private static Node FetchNode(List<Node> nodes, Vector2Int position)
    {
        foreach (Node node in nodes)
            if(node.position == position)
                return node;
        
        return null;
    }

    private static Node FindLowesFScore(List<Node> nodes)
    {
        Node fNode = null;
        float fScore = float.MaxValue;

        foreach (Node node in nodes)
        {
            if(node.FScore < fScore)
            {
                fScore = node.FScore;
                fNode = node;
            }
        }

        return fNode;
    }

    private static List<Vector2Int> ReconstructPath(Node endNode)
    {
        List<Vector2Int> path = new();
        Node currentNode = endNode;

        while(currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

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
