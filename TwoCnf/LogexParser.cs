namespace TwoCnf
{
    using System;
    using System.Collections.Generic;

    public class LogexParser
    {
        public List<Tuple<string, string>> Parse(string expression, string expressionVariable = "x", char andChar = '&', char orChar = '|')
        {
            var clearedExpression = ClearSpaces(expression);
            var parsedConjuctions = ParseConjuctions(clearedExpression, andChar);
            var parsedDisjunctions = ParseDisjunctions(parsedConjuctions, expressionVariable, orChar);

            parsedDisjunctions.Reverse();

            return parsedDisjunctions;
        }

        private string ClearSpaces(string expression)
        {
            return expression.Replace(" ", string.Empty);
        }

        private string[] ParseConjuctions(string readedData, char andChar)
        {
            return readedData.Split(andChar);
        }

        private List<Tuple<string, string>> ParseDisjunctions(string[] parsedConjuction, string expressionVariable, char orChar)
        {
            var parsedDisjunctions = new List<Tuple<string, string>>();

            foreach (var disjunction in parsedConjuction)
            {
                var removedBrackets = disjunction.Replace("(", string.Empty).Replace(")", string.Empty);
                var removedCharX = removedBrackets.Replace(expressionVariable, string.Empty);
                var removedNegation = removedCharX.Replace("!", "-");
                var parsedDisjunction = removedNegation.Split(orChar);

                parsedDisjunctions.Add(GetTupleFromArray(parsedDisjunction));
            }

            return parsedDisjunctions;
        }

        private Tuple<string, string> GetTupleFromArray(string[] parsedDisjunction)
        {
            var item1 = parsedDisjunction[0];
            var item2 = parsedDisjunction[1];

            return new Tuple<string, string>(item1, item2);
        }
    }
}
