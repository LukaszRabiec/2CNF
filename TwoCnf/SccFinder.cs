namespace TwoCnf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SccFinder
    {
        private readonly int[,] _matrix;
        private readonly int _verticesAmount;

        private DepthFirstSearch _dfs;
        private DepthFirstSearch _dfsTransposed;


        public SccFinder(ImplicationGraph implicationGraph)
        {
            _matrix = implicationGraph.Matrix;
            _verticesAmount = implicationGraph.VerticesAmount;
            _dfs = new DepthFirstSearch(implicationGraph.Neighbours);
        }

        public List<int>[] FindScc()
        {
            _dfs.Search();

            var outTimeIndexes = CreateIteratedArray();
            var neighboursTransposed = CreateTransposedNeighbours(outTimeIndexes);

            _dfsTransposed = new DepthFirstSearch(neighboursTransposed);
            _dfsTransposed.Search();

            RestoreStartingIndexesInDfsTransposed(outTimeIndexes);

            var scc = GetSccFromTreeForest();
            ReorderScc(scc);

            return scc;
        }

        private int[] CreateIteratedArray()
        {
            return Enumerable.Range(0, _verticesAmount).ToArray();
        }

        private List<int>[] CreateTransposedNeighbours(int[] outTimeIndexes)
        {
            var neighboursTransposed = InitializeListsArray();
            var outTime = (int[])_dfs.OutTime.Clone();

            Array.Sort(outTime, outTimeIndexes);
            Array.Reverse(outTimeIndexes);

            for (int i = 0; i < _verticesAmount; i++)
            {
                for (int j = 0; j < _verticesAmount; j++)
                {
                    if (_matrix[outTimeIndexes[i], outTimeIndexes[j]] == 1)
                    {
                        neighboursTransposed[j].Add(i);
                    }
                }
            }

            return neighboursTransposed;
        }

        private List<int>[] InitializeListsArray()
        {
            var collection = new List<int>[_verticesAmount];

            for (int vertice = 0; vertice < _verticesAmount; vertice++)
            {
                collection[vertice] = new List<int>();
            }

            return collection;
        }

        private void RestoreStartingIndexesInDfsTransposed(int[] indexes)
        {
            var outTimeTmp = (int[])_dfsTransposed.OutTime.Clone();
            var treesTmp = (int[])_dfsTransposed.Trees.Clone();

            for (int i = 0; i < indexes.Length; i++)
            {
                _dfsTransposed.OutTime[indexes[i]] = outTimeTmp[i];
                _dfsTransposed.Trees[indexes[i]] = treesTmp[i] == -1 ? -1 : indexes[treesTmp[i]];
            }
        }

        private List<int>[] GetSccFromTreeForest()
        {
            var treeForest = InitializeListsArray();

            for (int vertice = 0; vertice < _verticesAmount; vertice++)
            {
                int treeId = vertice;

                while (_dfsTransposed.Trees[treeId] != -1)
                {
                    treeId = _dfsTransposed.Trees[treeId];
                }

                treeForest[treeId].Add(vertice);
            }

            return treeForest.Where(list => list.Count != 0).ToArray();
        }

        private void ReorderScc(List<int>[] scc)
        {
            // Order with the second DFS (transposed)
            // Max from out time in each scc

            var maxTimeScc = new int[scc.Length];

            for (int sccId = 0; sccId < scc.Length; sccId++)
            {
                maxTimeScc[sccId] = int.MaxValue;

                foreach (var vertice in scc[sccId])
                {
                    maxTimeScc[sccId] = Math.Min(maxTimeScc[sccId], _dfsTransposed.OutTime[vertice]);
                }
            }

            Array.Sort(maxTimeScc, scc);
        }
    }
}
