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

        public string returnSolution(string eingabe)
        {

            string convertedNumbers = eingabe;
            List<string> numberList = scanForNumbers(eingabe);
            foreach (string number in numberList) {
                convertedNumbers = convertedNumbers.Replace(number, convertToDecimal(number));
            }
            string bracesSolved = braceBasher(convertedNumbers);
            string result = berechner.calculateExpressionWithoutBraces(bracesSolved);

            return convertToAllSystems(result);
        }

        // erst alle offenen klammern indexieren und dann vom höchsten level rekursiv auflösen
        private string braceBasher(string input)
        {
            SortedList<Tuple<int, int>, int> klammerRangeToKlammerLevelSortedList = scanForBraces(input);
            int highestLevel = 0;
            for (int i = 1; klammerRangeToKlammerLevelSortedList.ContainsValue(i); i++)
            {
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
                        expressionsWithBraces.Add(input.Substring(entry.Key.Item1, entry.Key.Item2 - entry.Key.Item1 + 1));
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
                    return braceBasher(modifiedInput);
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
        private SortedList<Tuple<int, int>, int> scanForBraces(string input)
        {
            SortedList<Tuple<int, int>, int> klammerRangeToKlammerlevelSortedList = new SortedList<Tuple<int, int>, int>();
            List<int> klammerLevelStack = new List<int>();
            int klammerLevel = 0;
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '(':
                        klammerLevel++;
                        klammerLevelStack.Add(i);
                        break;
                    case ')':
                        klammerRangeToKlammerlevelSortedList.Add(Tuple.Create(klammerLevelStack[klammerLevel - 1], i), klammerLevel);
                        klammerLevel--;
                        klammerLevelStack.RemoveAt(klammerLevel);
                        break;
                }
            }
            return klammerRangeToKlammerlevelSortedList;
        }
        // nach Ascii tabelle sind alle operatoren und klammern kleiner als alle zahlen und buchstaben
        private List<string> scanForNumbers(string input)
        {
            var numberList = new List<string>();
            // iteriere über string, wenn input[i]>47  oder input[i]=44 Zeichen dem String hinzufügen (44 ist ascii Komma)
            string tempZahl = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] > (char)47 || input[i] == (char)44)
                {
                    tempZahl = tempZahl + input[i].ToString();
                }
                else if (tempZahl.Length > 0)
                {
                    numberList.Add(tempZahl);
                    tempZahl = "";
                }
            }
            if (tempZahl.Length > 0) {
                numberList.Add(tempZahl);
            }
            return numberList;
        }

        // konvertiert eine einzelne zahl in eine dezimalzahl
        private string convertToDecimal(string zahl)
        {
            string dezZahl;
            //Delegiert die umrechnung an den Berechner und kümmert sich nur darum, dass der Berechner nichts mehr mit dem String machen muss, außer zu rechnen
            if (zahl.EndsWith("b"))
            {
                if (!checkZahl(zahl.Remove(zahl.Length - 1), (char)49))
                {
                    Console.WriteLine("{0} ist keine Binärzahl, Sie wurde als Dezimalwert gerechnet", zahl);
                    return zahl.Remove(zahl.Length - 1); // Zahl falsch als binär markiert, wir werten sie als Dezimalzahl
                }
                dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "b");
            }
            else if (zahl.EndsWith("h"))
            {
                dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "h");
            }
            else if (zahl.EndsWith("o"))
            {
                if (!checkZahl(zahl.Remove(zahl.Length - 1), (char)56))
                {
                    Console.WriteLine("{0} ist keine Octalzahl, Sie wurde als Dezimalwert gerechnet", zahl);
                    return zahl.Remove(zahl.Length - 1);// Zahl falsch als octal markiert, wird als dezimalzahl gewertet
                }
                dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "o");
            }
            else
            { // wurde dezimal zahl übergeben
                if (!checkZahl(zahl.Remove(zahl.Length - 1), (char)57))
                {
                    Console.WriteLine("{0} ist keine Dezimalzahl, Sie wurde als Hexadezimalzahl gerechnet");
                    dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "h");
                }
                else
                {
                    return zahl;
                }

            }
            return dezZahl;
        }

        // gibt die Zahl an den Berechner weiter zur konvertierung und andere Zahlensysteme und Formatiert das ergebnis 
        private string convertToAllSystems(string input)
        {
            string[] konvertierteZahlen = berechner.convertResult(input);
            return string.Format("Dez: {0} | Bin: {1} | Hex: {2} | Oct: {3}", input, konvertierteZahlen[0], konvertierteZahlen[1], konvertierteZahlen[2]); ;
        }

        private bool checkZahl(string zahl, char asciiTabellenWert)
        {
            foreach (char ziffer in zahl)
            {
                if (ziffer > asciiTabellenWert)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
