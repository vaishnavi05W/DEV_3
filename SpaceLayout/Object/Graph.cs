using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Object
{
    public class Graph
    {
        private int Vertices;
        private List<int>[] adj;

        public Graph(int v)
        {
            Vertices = v;
            adj = new List<int>[v];
            for (int i = 0; i < v; ++i)
                adj[i] = new List<int>();
        }

        public void AddEdge(int v, int w)
        {
            adj[v].Add(w);
        }
        public void DFS(int v, bool[] visited)
        {
            visited[v] = true;
            Console.Write(v + " ");

            List<int> vList = adj[v];
            foreach (var n in vList)
            {
                if (!visited[n])
                    DFS(n, visited);
            }
        }

        public void DFS()
        {
            bool[] visited = new bool[Vertices];

            for (int i = 0; i < Vertices; ++i)
            {
                if (!visited[i])
                {
                    Console.Write("\nDFS traversal starting from vertex " + i + ": ");
                    DFS(i, visited);
                }

                // Reset visited array for the next DFS traversal
                visited = new bool[Vertices];
            }
        }
    }

    //public class Program
    //{
    //    public static void Main()
    //    {
    //        Graph g = new Graph(4);

    //        g.AddEdge(0, 1);
    //        g.AddEdge(0, 2);
    //        g.AddEdge(1, 2);
    //        g.AddEdge(2, 0);
    //        g.AddEdge(2, 3);
    //        g.AddEdge(3, 3);

    //        g.DFS();
    //    }
    //}
}
