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
        private List<string> result;
        HashSet<Tuple<string, List<string>>> final = new HashSet<Tuple<string, List<string>>>();
        public Graph(List<int> nodes)
        {
            Vertices = nodes.Count;
            //foreach (var v in nodes)
            //{
                adj = new List<int>[nodes.Count+1];
                for (int i = 1; i <= nodes.Count; ++i)
                    adj[i] = new List<int>();
            //}
            
           
        }

        public void AddEdge(int v, int w)
        {
            adj[v].Add(w);
        }
        public List<string> DFS(int v, bool[] visited)
        {
            visited[v] = true;
            
            result.Add(v.ToString());
            Console.Write(v + " ");

            List<int> vList = adj[v];
            foreach (var n in vList)
            {
                if (!visited[n])
                    DFS(n, visited);
            }
            return result;
        }

        public HashSet<Tuple<string, List<string>>> DFS()
        {
            bool[] visited = new bool[Vertices+1];

            for (int i = 1; i <= Vertices; ++i)
            {
                if (!visited[i])
                {
                    Console.Write("\nDFS traversal starting from vertex " + i + ": ");
                    result = new List<string>();
                    final.Add(Tuple.Create(i.ToString(),DFS(i, visited)));
                }

                // Reset visited array for the next DFS traversal
                visited = new bool[Vertices+1];
                //visited[i] = true;
            }
            return final;
        }
    }

}
