using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Taschenrechner
{
    /*
     Der Parser konvertiert alle Zahlen in das Passende Format für den Berechner und leitet die Ergebnisse weiter
         */
    class EingabenParser
    {
        
        private List<Tuple<string, string>> historie = new List<Tuple<string, string>> ();
        private const string KLAMMER_PATTERN = "[(][^()]+[)]";
        private const string ZAHL_OHNE_VORZEICHEN_PATTERN = "[\\da-f]+,?[\\da-f]*[hbo]";
        private const string ZAHL_PATTERN = "-?[\\da-f]+,?[\\da-f]*[hbo]"; // Das Pattern einer Zahl: Negativ Zahl? Vorkomma-Anteil, Falls vorhanden Nachkommaanteil und falls vorhanden Postfix
        private const string KEINE_OCTALZAHL_PATTERN = "[9]";
        private const string KEINE_BINAERZAHL_PATTERN = "[^01b]";
        private const string KEINE_DEZIMALZAHL_PATTERN = "[a-f]";
        private Berechner berechner;

            // die Referenz des Berechners wird uns hier übergeben
        public EingabenParser(ref Berechner berechner) {
            this.berechner = berechner;
        }

        // berechnet das ergebnis in dem der Eingabe-String nach und nach aufgelöst wird
        // zuerst werden die klammern aufgelöst und danach Punkt vor Strich gerechnet 
        public string returnSolution(string eingabe) {
           
            string modifiedInput = eingabe;
            MatchCollection gemischteZahlen = Regex.Matches(modifiedInput, ZAHL_OHNE_VORZEICHEN_PATTERN);
            foreach (Match zahl in gemischteZahlen) {
                modifiedInput= modifiedInput.Replace(zahl.Value, convertToDecimal(zahl.Value));
            }
            // Löst alle Klammern 
            while(Regex.IsMatch(modifiedInput, KLAMMER_PATTERN)) {
                modifiedInput = braceBasher(modifiedInput);
            }
            modifiedInput = berechner.calculateExpressionWithoutBraces(modifiedInput);
            modifiedInput = convertToAllSystems(modifiedInput);

            writeHistory(eingabe, modifiedInput);
            return modifiedInput;
        }

        // Löst Klammern auf unterster Ebene auf und gibt die eingegebene Rechnung mit aufgelösten Klammern zurück
        // Bsp: 7+(3+(9*9)) wird zu 7+(3+81) aufgelöst 
        private string braceBasher(string input) {
            MatchCollection klammern = Regex.Matches(input, KLAMMER_PATTERN);
            string modifiedInput = input;
            foreach (Match klammer in klammern)
            {
                string klammerRemoved = klammer.Value.Substring(1,klammer.Value.Length-2);
                modifiedInput = modifiedInput.Replace(klammer.Value, berechner.calculateExpressionWithoutBraces(klammerRemoved));
            }
            return modifiedInput;
        }

        // Konvertierung einer Zahl in eine dezimalzahl
        private string convertToDecimal(string zahl)
        {
            string dezZahl;
            //Delegiert die umrechnung an den Berechner und kümmert sich nur darum, dass der Berechner nichts mehr mit dem String machen muss, außer zu rechnen
            if (zahl.EndsWith("b"))
            {
                if (Regex.IsMatch(zahl, KEINE_BINAERZAHL_PATTERN)){
                    Console.WriteLine("{0} ist keine Binärzahl, Sie wurde als Dezimalwert gerechnet", zahl);
                    return zahl.Remove(zahl.Length - 1); // Zahl falsch als binär markiert, wir werten sie als Dezimalzahl
                }
                dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "b");
            }
            else if (zahl.EndsWith("h"))
            {
                dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "h");
            }
            else if (zahl.EndsWith("o")) {
                if (Regex.IsMatch(zahl, KEINE_OCTALZAHL_PATTERN)){
                    Console.WriteLine("{0} ist keine Octalzahl, Sie wurde als Dezimalwert gerechnet", zahl);
                    return zahl.Remove(zahl.Length - 1);// Zahl falsch als octal markiert, wird als dezimalzahl gewertet
                }
                dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "o");
            }
            else { // wurde dezimal zahl übergeben
                if (Regex.IsMatch(zahl, KEINE_DEZIMALZAHL_PATTERN))
                {
                    Console.WriteLine("{0} ist keine Dezimalzahl, Sie wurde als Hexadezimalzahl gerechnet");
                    dezZahl = berechner.convertToDecimal(zahl.Remove(zahl.Length - 1), "h");
                }
                else {
                    return zahl;
                }
                
            }
            return dezZahl;
        }

        // gibt die Zahl an den Berechner weiter zur konvertierung und andere Zahlensysteme und Formatiert das ergebnis 
        private string convertToAllSystems(string input) {
            string[] konvertierteZahlen = berechner.convertResult(input);            
            return string.Format("Dez: {0} | Bin: {1} | Hex: {2} | Oct: {3}", input, konvertierteZahlen[0], konvertierteZahlen[1], konvertierteZahlen[2]); ;
        }

        private void writeHistory(string input, string ergebnis)
        {
            Tuple<string, string> combined = new Tuple<string, string>(input, ergebnis);
            historie.Add(combined);
        }

        public void watchHistoryChannel()
        {
            Console.WriteLine();
            foreach (Tuple<string,string> eintrag in historie) {
                Console.WriteLine(eintrag.Item2 +" = " + eintrag.Item1);
            }
            Console.WriteLine();
        }

    }
}
