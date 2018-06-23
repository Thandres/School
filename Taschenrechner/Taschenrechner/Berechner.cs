using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Taschenrechner
{
    /*
     Erhält Ausdrücke in Dezimalform ohne Klammern und berechnet diese
         */
    class Berechner
    {
       
        private const string RECHNUNG_PATTERN_TEMPLATE = "-?[\\da-f]+,?[\\da-f]*[hdo]?[XXX]-?[\\da-f]+,?[\\da-f]*[hdo]?";
        private const string HEX_PATTERN = "[a-f]|[0-9]";
        private const string ZAHL_OHNE_vORZEICHEN_PATTERN = "\\d+";
        private const string VORKOMMAZAHL_PATTERN = "[\\da-f]+";

        /*
       Berechnet das ergebnis einer Mathematischen Anweisung, die keine Klammern enthält
       Gibt das Ergebnis der Berechnung zurück
       Parameter: nobrackets -> eine Mathematische Formel ohne Klammern Bsp.: 3+4 oder 34/28+9
           */
        public string calculateExpressionWithoutBraces(string noBraces)
        {
            string modifiedInput = noBraces;

            modifiedInput = calculateOperations(modifiedInput, "*");
            modifiedInput = calculateOperations(modifiedInput, "/");
            modifiedInput = calculateOperations(modifiedInput, "+");
            modifiedInput = calculateOperations(modifiedInput, "-");

            return modifiedInput;
        }

        /*
         führt alle Operationen eines Typs in einer übergebenen Rechnung durch und gibt das Ergebnis als verkürzten String zurück
             */
        private string calculateOperations(string input, string operation)
        {
            // pattern ist ein String der immer nur zwei Zahlen kombininiert über einen Operator enthält. Bsp: 2+4 oder 58*6
            string pattern = RECHNUNG_PATTERN_TEMPLATE.Replace("XXX", operation);
            // überprüft nach jedem replace schritt ob noch eine Operation vorhanden ist
            while (Regex.IsMatch(input, pattern))
            {
                Match ausdruck = Regex.Match(input, pattern);
                string[] zahlen = ausdruck.ToString().Split(operation);
                double temp = 0.0;
                // Welche operation soll durchgeführt werden?
                switch (operation)
                {
                    case "*":
                        temp = Double.Parse(zahlen[0]) * Double.Parse(zahlen[1]);
                        break;
                    case "/":
                        temp = Double.Parse(zahlen[0]) / Double.Parse(zahlen[1]);
                        break;
                    case "+":
                        temp = Double.Parse(zahlen[0]) + Double.Parse(zahlen[1]);
                        break;
                    case "-":
                        temp = Double.Parse(zahlen[0]) - Double.Parse(zahlen[1]);
                        break;
                }
                input = input.Replace(ausdruck.Value, temp.ToString());
            }
            return input;
        }
        // Konvertiert die überreichte zahl ins angegebene System. Wenn kein System angegeben ist wird es als Dezimalzahl gehandelt
        public string convertToDecimal(string zahl, string zSystem) {
            string dezZahl;
            switch (zSystem) {
                case "b":
                    dezZahl = convertFromBinary(zahl);
                    break;
                case "h":
                    dezZahl = convertFromHex(zahl);
                    break;
                case "o":
                    dezZahl = convertFromOctal(zahl);
                    break;
                default:
                return zahl;
            }
            return dezZahl;
        }

        // Teilt Octalzahl in seine Wertigkeiten um und konvertiert dann zu dezimalzahl -> 843 = 1.Wertigkeit: 8 2.Wertigkeit: 4 3.Wertigkeit: 3
        private string convertFromOctal(string zahl)
        {
            int power = calculatePower(zahl);
            double tempDezZahl = 0;
            for (int i = 0; i < zahl.Length; i++, power--)
            {
                tempDezZahl += Convert.ToDouble(Int32.Parse(zahl[i].ToString()) * Math.Pow(8, power));
            }
            return tempDezZahl.ToString();
        }
        // Teilt Binärzahl in seine Wertigkeiten um und konvertiert dann zu dezimalzahl -> 101 = 1.Wertigkeit: 1 2.Wertigkeit: 0 3.Wertigkeit: 1
        private string convertFromBinary(string zahl)
        {
            char[] binaerZahl = zahl.ToCharArray(0, zahl.Length);
            int power = binaerZahl.Length - 1;
            int tempDezZahl = 0;
            // binärzahl von links nach rechts abgehen und entsprechende 2er Potenz addieren
            for (int i = 0; i < binaerZahl.Length; i++, power--)
            {
                if (binaerZahl[i] == '1')
                {
                    tempDezZahl += Convert.ToInt32(Math.Pow(2, power));
                }
            }
            return tempDezZahl.ToString();
        }
        // Teilt Hexadezimalzahl in seine Wertigkeiten um und konvertiert dann zu dezimalzahl -> f15 = 1.Wertigkeit: f 2.Wertigkeit: 1 3.Wertigkeit: 5
        private string convertFromHex(string zahl)
        {
            zahl = zahl.ToLower();
            MatchCollection hexWertigkeiten = Regex.Matches(zahl, HEX_PATTERN);
            int power = calculatePower(zahl);
            double tempDezZahl = 0;
            foreach (Match hexWertigkeit in hexWertigkeiten)
            {
                switch (hexWertigkeit.Value)
                {
                    case "a":
                        tempDezZahl += 10 * Math.Pow(16, power);
                        break;
                    case "b":
                        tempDezZahl += 11 * Math.Pow(16, power);
                        break;
                    case "c":
                        tempDezZahl += 12 * Math.Pow(16, power);
                        break;
                    case "d":
                        tempDezZahl += 13 * Math.Pow(16, power);
                        break;
                    case "e":
                        tempDezZahl += 14 * Math.Pow(16, power);
                        break;
                    case "f":
                        tempDezZahl += 15 * Math.Pow(16, power);
                        break;
                    default:
                        tempDezZahl += Int32.Parse(hexWertigkeit.Value) * Math.Pow(16, power);
                        break;
                }
                power--;
            }
            return tempDezZahl.ToString();
        }

        public string[] convertResult(string input)
        {
            string ohneVorzeichen = Regex.Match(input, ZAHL_OHNE_vORZEICHEN_PATTERN).Value;
            string[] resultString = new string[3];
            resultString[0] = convertToBinary(ohneVorzeichen);
            resultString[1] = convertToHexadecimal(ohneVorzeichen);
            resultString[2] = convertToOctal(ohneVorzeichen);
            return resultString;
        }
        // kann nur ganzzahlige dezimalzahlen konvertieren und rundet nach interner logik
        private string convertToBinary(string dezZahl) {
            int zahl = Convert.ToInt32(Double.Parse((dezZahl)));
            string binZahl = "";
            while (zahl > 0) {
                binZahl = (zahl % 2) + binZahl;
                zahl /= 2;
            }
            return binZahl;
        }

        private string convertToOctal(string dezZahl){
            int zahl = Convert.ToInt32(Double.Parse(dezZahl));
            string octalZahl = "";
            while(zahl > 0)
            {
                octalZahl = zahl % 8 + octalZahl;
                zahl /= 8;
            }
            return octalZahl;
        }

        // kann nur ganzzahlig und rundet nach interner logik
        private string convertToHexadecimal(string dezZahl) {
            int zahl = Convert.ToInt32(Double.Parse((dezZahl)));
            string hexZahl = "";
            while (zahl > 0)
            {
                int rest = zahl % 16;
                switch (rest)
                {
                    case 15:
                        hexZahl = "f" + hexZahl;
                        break;
                    case 14:
                        hexZahl = "e" + hexZahl;
                        break;
                    case 13:
                        hexZahl = "d" + hexZahl;
                        break;
                    case 12:
                        hexZahl = "c" + hexZahl;
                        break;
                    case 11:
                        hexZahl = "b" + hexZahl;
                        break;
                    case 10:
                        hexZahl = "a" + hexZahl;
                        break;
                    default:
                        hexZahl = rest + hexZahl;
                        break;
                }
                zahl /= 16;
            }
            return hexZahl;
        }

        // gibt den höchsten Exponenten an den die Zahl hat
        private int calculatePower(string zahl) {
            return Regex.Match(zahl, VORKOMMAZAHL_PATTERN).Length-1;
        }
    }
}
