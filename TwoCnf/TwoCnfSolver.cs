using System;
using System.Collections.Generic;

namespace TwoCnf
{
    using System.Linq;

    public class TwoCnfSolver
    {
        private int _indexesAmount;
        private SolutionState[] _solutionStates;

        public bool[] FindSolution(List<Tuple<string, string>> parsedDisjunctions)
        {
            var graph = new ImplicationGraph(parsedDisjunctions);
            _indexesAmount = graph.IndexesAmount;
            var sccFinder = new SccFinder(graph);

            var sccCollection = sccFinder.FindScc();

            if (NoSolutions(sccCollection))
            {
                throw new ArgumentException("There is no solutions for this expression (the same component can not contain contradictory values).");
            }

            SetSolutionStates(sccCollection);
            var booleans = GetBooleansFromStates();
            var solution = GetSolutionForEachIndex(booleans);

            return solution;
        }

        private bool NoSolutions(List<int>[] sccCollection)
        {
            foreach (List<int> scc in sccCollection)
            {
                // If the same SCC contains 'x' and '!x' then return false;
                foreach (var vertice in scc)
                {
                    if (vertice > _indexesAmount)
                    {
                        if (scc.Contains(vertice - _indexesAmount))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (scc.Contains(vertice + _indexesAmount))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void SetSolutionStates(List<int>[] sccCollection)
        {
            _solutionStates = new SolutionState[2 * _indexesAmount];

            foreach (var scc in sccCollection)
            {
                if (AllVerticesInSccAreUnset(scc))
                {
                    foreach (var vertice in scc)
                    {
                        _solutionStates[vertice] = SolutionState.False;
                        SetVerticeToGivenValue(vertice, SolutionState.True);
                    }
                }
                else
                {
                    var alreadySetValue = GetValeFromAlreadySetVertice(scc);
                    SetUnsetVerticesAndItsNegations(scc, alreadySetValue);
                }
            }
        }

        private bool AllVerticesInSccAreUnset(List<int> scc)
        {
            return scc.All(vertice => _solutionStates[vertice] == SolutionState.Unset);
        }

        private void SetVerticeToGivenValue(int vertice, SolutionState value)
        {
            if (vertice < _indexesAmount)
            {
                _solutionStates[vertice + _indexesAmount] = value;
            }
            else
            {
                _solutionStates[vertice - _indexesAmount] = value;
            }
        }

        private SolutionState GetValeFromAlreadySetVertice(List<int> scc)
        {
            var value = SolutionState.Unset;

            foreach (var vertice in scc)
            {
                if (_solutionStates[vertice] != SolutionState.Unset)
                {
                    value = _solutionStates[vertice];
                    break;
                }
            }

            return value;
        }

        private void SetUnsetVerticesAndItsNegations(List<int> scc, SolutionState alreadySetValue)
        {
            var negation = GetNegationOfValue(alreadySetValue);

            foreach (var vertice in scc)
            {
                if (_solutionStates[vertice] == SolutionState.Unset)
                {
                    _solutionStates[vertice] = alreadySetValue;
                    SetVerticeToGivenValue(vertice, negation);
                }
            }
        }

        private SolutionState GetNegationOfValue(SolutionState alreadySetValue)
        {
            return alreadySetValue == SolutionState.True ? SolutionState.False : SolutionState.True;
        }

        private bool[] GetBooleansFromStates()
        {
            var booleans = new bool[_solutionStates.Length];

            for (int vertice = 0; vertice < booleans.Length; vertice++)
            {
                bool boolean;

                switch (_solutionStates[vertice])
                {
                    case SolutionState.True:
                        boolean = true;
                        break;
                    case SolutionState.False:
                        boolean = false;
                        break;
                    case SolutionState.Unset:
                        throw new ArgumentException("Solution States must be true or false!");
                    default:
                        throw new ArgumentException("Solution States must be true or false!");
                }

                booleans[vertice] = boolean;
            }

            return booleans;
        }

        private bool[] GetSolutionForEachIndex(bool[] booleans)
        {
            var solution = new bool[_indexesAmount];

            if (BooleansAreIncorrect(booleans))
            {
                throw new ArgumentException("One of expression value and it's negation have the same value!");
            }

            for (int index = 0; index < _indexesAmount; index++)
            {
                solution[index] = booleans[index];
            }

            return solution;
        }

        private bool BooleansAreIncorrect(bool[] booleans)
        {
            for (int index = 0; index < _indexesAmount; index++)
            {
                if (booleans[index] == booleans[index + _indexesAmount])
                {
                    return true;
                }
            }

            return false;
        }

        private enum SolutionState
        {
            Unset,
            True,
            False
        }
    }
}
