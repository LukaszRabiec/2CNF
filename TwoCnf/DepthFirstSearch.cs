namespace TwoCnf
{
    using System.Collections.Generic;
    using System.Linq;

    public class DepthFirstSearch
    {
        public int[] OutTime { get; set; }
        public int[] Trees { get; set; }

        private readonly List<int>[] _neighbours;
        private readonly int _verticesAmount;

        private StateColor[] _stateColor;
        private int _time;

        public DepthFirstSearch(List<int>[] neighbours)
        {
            var size = neighbours.Length;
            _neighbours = neighbours;

            OutTime = new int[size];
            Trees = new int[size];
            _stateColor = new StateColor[size];
            _verticesAmount = size;
        }

        public void Search()
        {
            ClearTreesAndStates();
            _time = 0;

            for (int vertice = 0; vertice < _verticesAmount; vertice++)
            {
                if (_stateColor[vertice] == StateColor.White)
                {
                    VisitVertice(vertice);
                }
            }
        }

        private void ClearTreesAndStates()
        {
            Trees = Enumerable.Repeat(-1, _verticesAmount).ToArray();
            _stateColor = Enumerable.Repeat(StateColor.White, _verticesAmount).ToArray();
        }

        private void VisitVertice(int vertice)
        {
            _stateColor[vertice] = StateColor.Red;

            foreach (var neighbour in _neighbours[vertice])
            {
                if (_stateColor[neighbour] == StateColor.White)
                {
                    Trees[neighbour] = vertice;
                    VisitVertice(neighbour);
                }
            }

            _stateColor[vertice] = StateColor.Blue;
            _time++;
            OutTime[vertice] = _time;
        }

        /// <summary>
        /// Processing state of vertice
        /// </summary>
        private enum StateColor
        {
            /// <summary>
            /// Not processed
            /// </summary>
            White,
            /// <summary>
            /// Processing
            /// </summary>
            Red,
            /// <summary>
            /// Precessed
            /// </summary>
            Blue
        }
    }
}
