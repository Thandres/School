using System;
using System.Collections.Generic;
using System.Text;

namespace Taschenrechner
{
    class JankParser
    {
        private Berechner berechner;
        public JankParser(ref Berechner berechner)
        {
            this.berechner = berechner;
        }
        // zu bedienen:
        // calculateExpressionWithoutBraces(string noBraces)
        // convertToDecimal(string zahl, string zSystem)
        // convertResult(string input)

        public string returnSolution(string eingabe) {
            string bracesSolved = bracebasher(eingabe);
            return berechner.calculateExpressionWithoutBraces(bracesSolved);
        }

        // erst alle offenen klammern indexieren und dann vom höchsten level rekursiv auflösen
        private string bracebasher(string input) {
            SortedList<Tuple<int, int>, int> klammerRangeToKlammerLevelSortedList = scanForBraces(input);
            int highestLevel = 0;
            for (int i = 1; klammerRangeToKlammerLevelSortedList.ContainsValue(i); i++) {
                highestLevel = i;
            }
            string modifiedInput = input;
            if (highestLevel > 0)
            {
                List<string> expressionsWithBraces = new List<string>();
                foreach (var entry in klammerRangeToKlammerLevelSortedList)
                {
                    if (entry.Value == highestLevel)
                    {
                        expressionsWithBraces.Add(input.Substring(entry.Key.Item1, entry.Key.Item2 - entry.Key.Item1+1));
                    }
                }
                var resultList = new List<string>();
                foreach (string expressionWithBraces in expressionsWithBraces)
                {
                    resultList.Add(berechner.calculateExpressionWithoutBraces(expressionWithBraces.Substring(1, expressionWithBraces.Length - 2)));
                }
                
                for (int i = 0; i < resultList.Count; i++)
                {
                    modifiedInput = modifiedInput.Replace(expressionsWithBraces[i], resultList[i]);
                }
                // nach einem durchlauf wird 1 Klammerlevel aufgelöst, wenn das klammerlevel 1 oder 0 war gibt es keine klammern mehr
                if (highestLevel > 1)
                {
                    return bracebasher(modifiedInput);
                }
                else
                {
                    return modifiedInput;
                }
            }
            else
            {
                return input;
            }
       
            
        }
        // speichert alle Klammern mit start- und end-index kombiniert mit klammerlevel ab
        // KlammerLevelStack dient als startindex-Halter, damit beim einer geschlossenen klammer start und endindex zur verfügung stehen
        private SortedList<Tuple<int, int>, int> scanForBraces(string input) {
            SortedList<Tuple<int, int>, int> klammerRangeToKlammerlevelSortedList = new SortedList<Tuple<int, int>, int>();
            List<int> klammerLevelStack = new List<int>();
            int klammerLevel = 0;
            for (int i = 0; i < input.Length; i++) {
                switch (input[i]) {
                    case '(':
                        klammerLevel++;                       
                        klammerLevelStack.Add(i);
                        break;
                    case ')':       
                        klammerRangeToKlammerlevelSortedList.Add(Tuple.Create(klammerLevelStack[klammerLevel-1], i), klammerLevel);
                        klammerLevel--;
                        klammerLevelStack.RemoveAt(klammerLevel);
                        break;
                }
            }
            return klammerRangeToKlammerlevelSortedList;
        }
    }
}
