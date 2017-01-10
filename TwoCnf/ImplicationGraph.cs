namespace TwoCnf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImplicationGraph
    {
        public int[,] Matrix { get; private set; }
        public List<int>[] Neighbours { get; private set; }
        public int VerticesAmount { get; private set; }
        public int IndexesAmount { get; private set; }

        public ImplicationGraph(List<Tuple<string, string>> parsedDisjunctions)
        {
            InitializeGraph(parsedDisjunctions);
            CreateGraphFromDisjunctions(parsedDisjunctions);
        }

        private void InitializeGraph(List<Tuple<string, string>> disjunctions)
        {
            IndexesAmount = GetMaxIndexFromTuples(disjunctions) + 1;
            VerticesAmount = 2 * IndexesAmount;

            Matrix = new int[VerticesAmount, VerticesAmount];
            Neighbours = new List<int>[VerticesAmount];
            for (int i = 0; i < VerticesAmount; i++)
            {
                Neighbours[i] = new List<int>();
            }
        }

        private int GetMaxIndexFromTuples(List<Tuple<string, string>> tuples)
        {
            var maximums = new List<int>();

            foreach (var tuple in tuples)
            {
                maximums.Add(Math.Abs(Convert.ToInt32(tuple.Item1)));
                maximums.Add(Math.Abs(Convert.ToInt32(tuple.Item2)));
            }

            return maximums.Max();
        }

        private void CreateGraphFromDisjunctions(List<Tuple<string, string>> disjunctions)
        {
            foreach (var disjunction in disjunctions)
            {
                var convertedItem1 = Convert.ToInt32(disjunction.Item1);
                var convertedItem2 = Convert.ToInt32(disjunction.Item2);

                var p = disjunction.Item1.Contains("-") ? -convertedItem1 + IndexesAmount : convertedItem1;
                var q = disjunction.Item2.Contains("-") ? -convertedItem2 + IndexesAmount : convertedItem2;

                // ( p | q ) => ( !p -> q ) & ( !p -> q )
                var pNeg = p < IndexesAmount ? p + IndexesAmount : p - IndexesAmount;
                var qNeg = q < IndexesAmount ? q + IndexesAmount : q - IndexesAmount;

                Matrix[pNeg, q] = 1;
                Matrix[qNeg, p] = 1;

                Neighbours[pNeg].Add(q);
                Neighbours[qNeg].Add(p);
            }
        }
    }
}
