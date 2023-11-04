using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpaltenCheckMethoden
{
    public class Methods
    {
        public (string, int, Dictionary<string, int>) SpaltenMapping(List<List<List<string>>> mapping, string spalte, int spalteni, int linie, int fehleranzahl, bool run2, int seperatormultiple, string spaltendelimiter, int delimiterlength, string seconddelimiter, Dictionary<string, int> fehlermeldungen)
        {
            MethodOperation method = new MethodOperation();
            if (run2)
            {
                if ((spalteni % seperatormultiple == 0 && seperatormultiple / 2 % 2 != 0) || (seperatormultiple / 2 % 2 == 0 && spalteni != 2 && (spalteni + 2) % seperatormultiple != 0) || spalteni == 0 || spalteni >= delimiterlength - 2)
                {
                    return (spalte, fehleranzahl, fehlermeldungen);
                }
                double newspalteni = spalteni / seperatormultiple;
                spalteni = (int)Math.Floor(newspalteni);
            }
            var result = (spalte, fehleranzahl, fehlermeldungen);
            int methodnumber = 0;
            if (bool.Parse(mapping[spalteni][methodnumber][0]))
            {
                result = method.ContainsCheck(result.Item1, spalteni, linie, result.Item2, spaltendelimiter, bool.Parse(mapping[spalteni][methodnumber][1]), mapping[spalteni][methodnumber][2], mapping[spalteni][methodnumber][3], mapping[spalteni][methodnumber][4], mapping[spalteni][methodnumber][5], mapping[spalteni][methodnumber][6], mapping[spalteni][methodnumber][7], result.Item3);
            }
            methodnumber++;
            if (bool.Parse(mapping[spalteni][methodnumber][0]))
            {
                result = method.StarsWithCheck(result.Item1, spalteni, linie, result.Item2, seconddelimiter, bool.Parse(mapping[spalteni][methodnumber][1]), mapping[spalteni][methodnumber][2], result.Item3);
            }
            methodnumber++;
            if (bool.Parse(mapping[spalteni][methodnumber][0]))
            {
                result = method.SpaltenCharactereCheck(result.Item1, spalteni, linie, result.Item2, bool.Parse(mapping[spalteni][methodnumber][1]), int.Parse(mapping[spalteni][methodnumber][2]), int.Parse(mapping[spalteni][methodnumber][3]), result.Item3);
            }
            methodnumber++;
            if (bool.Parse(mapping[spalteni][methodnumber][0]))
            {
                result = method.RoundCheck(result.Item1, spalteni, linie, result.Item2, mapping[spalteni][methodnumber][2], bool.Parse(mapping[spalteni][methodnumber][1]), result.Item3);
            }
            methodnumber++;
            if (bool.Parse(mapping[spalteni][methodnumber][0]))
            {
                result = method.EmptyCheck(result.Item1, spalteni, linie, result.Item2, bool.Parse(mapping[spalteni][methodnumber][1]), result.Item3);
            }
            methodnumber++;
            if (bool.Parse(mapping[spalteni][methodnumber][0]))
            {
                result = method.TypeCheck(result.Item1, spalteni, linie, result.Item2, bool.Parse(mapping[spalteni][methodnumber][1]), result.Item3, mapping[spalteni][methodnumber][2]);
            }
            spalte = result.Item1;
            fehleranzahl = result.Item2;
            fehlermeldungen = result.Item3;
            return (spalte, fehleranzahl, fehlermeldungen);

        }
        public class MethodOperation 
        {
            public (string, int, Dictionary<string, int>) SpaltenCharactereCheck(string spalten, int i, int line, int fehlerzahl, bool autokorrektur, int minimumlength, int maximumlength, Dictionary<string, int> fehlermeldungen)
            {
                if (spalten.Length < minimumlength)
                {
                    string fehlermeldung =
                        $"Fehler(Spalte {i + 1} ist kürzer als {minimumlength} Zeichen) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    int placeholdersToAdd = minimumlength - spalten.Length;
                    if (autokorrektur)
                    {
                        spalten = new string('_', placeholdersToAdd) + spalten;
                        Console.WriteLine($"Korrektur: {spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }
                else if (spalten.Length > maximumlength)
                {
                    string fehlermeldung =
                        $"Fehler(Spalte {i + 1} ist länger als {maximumlength} Zeichen) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    if (autokorrektur)
                    {
                        spalten = spalten.Substring(0, maximumlength);
                        Console.WriteLine($"Korrektur: {spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }

                return (spalten, fehlerzahl, fehlermeldungen);
            }
            public (string, int, Dictionary<string, int>) StarsWithCheck(string spalten, int i, int line, int fehlerzahl, string seconddelimiter, bool autokorrektur, string startswith, Dictionary<string, int> fehlermeldungen)
            {
                if (!spalten.StartsWith(startswith))
                {
                    string fehlermeldung =
                        $"Fehler(Spalte {i + 1} muss mit {startswith} beginnen) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    if (autokorrektur)
                    {
                        spalten = $"{startswith} {spalten}";
                        Console.WriteLine($"Korrektur: {spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }

                return (spalten, fehlerzahl, fehlermeldungen);
            }
            public (string, int, Dictionary<string, int>) ContainsCheck(string spalten, int i, int line, int fehlerzahl, string spaltendelimiter, bool autokorrektur, string contains1, string newvalue1, string contains2, string newvalue2, string contains3, string newvalue3, Dictionary<string, int> fehlermeldungen)
            {
                if (spalten.Contains(contains1) && !String.IsNullOrEmpty(contains1))
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} enthält {contains1}) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    if (autokorrektur)
                    {
                        spalten = spalten.Replace(contains1, newvalue1);
                        Console.WriteLine($"Korrektur: {spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }

                if (spalten.Contains(contains2) && !String.IsNullOrEmpty(contains2))
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} enthält {contains2}) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    if (autokorrektur)
                    {
                        spalten = spalten.Replace(contains2, newvalue2);
                        Console.WriteLine($"Korrektur: {spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }

                if (spalten.Contains(contains3) && !String.IsNullOrEmpty(contains3))
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} enthält {contains3}) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    if (autokorrektur)
                    {
                        spalten = spalten.Replace(contains3, newvalue3);
                        Console.WriteLine($"Korrektur: {spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }


                return (spalten, fehlerzahl, fehlermeldungen);
            }
            public (string, int, Dictionary<string, int>) RoundCheck(string spalten, int i, int line, int fehlerzahl, string rounding, bool autokorrektur, Dictionary<string, int> fehlermeldungen)
            {
                if (spalten.Contains(",") && float.TryParse(spalten, out float number))
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} ist eine Kommazahl) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    int roundedNumber = 0;
                    if (rounding == "roundup")
                    {
                        roundedNumber = (int)Math.Ceiling(number);
                    }
                    else if (rounding == "rounddown")
                    {
                        roundedNumber = (int)Math.Floor(number);
                    }
                    else
                    {
                        roundedNumber = (int)Math.Round(number);
                    }

                    if (autokorrektur)
                    {
                        spalten = roundedNumber.ToString();
                        Console.WriteLine($"Korrektur:{spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }
                return (spalten, fehlerzahl, fehlermeldungen);
            }
            public (string, int, Dictionary<string, int>) EmptyCheck(string spalten, int i, int line, int fehlerzahl, bool autokorrektur, Dictionary<string, int> fehlermeldungen)
            {
                if (string.IsNullOrEmpty(spalten))
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} ist leer) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlerzahl++;
                    if (autokorrektur)
                    {
                        spalten = "0";
                        Console.WriteLine($"Korrektur:{spalten}");
                    }
                    else
                    {
                        fehlermeldungen.Add(fehlermeldung, line);
                    }
                }
                return (spalten, fehlerzahl, fehlermeldungen);
            }
            public (string, int, Dictionary<string, int>) TypeCheck(string spalten, int i, int line, int fehlerzahl, bool autokorrektur, Dictionary<string, int> fehlermeldungen, string type)
            {
                bool isint = false;
                bool isfloat = false;
                static bool IsAlphanumeric(string s)
                {
                    return Regex.IsMatch(s, "^[a-zA-Z0-9]+$");
                }
                if (int.TryParse(spalten, out int spaltenint))
                {
                    isint = true;
                }
                if (float.TryParse(spalten, out float spaltenfloat))
                {
                    isfloat = true;
                }
                if (type.Contains("isint") && !isint)
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} ist kein Int) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlermeldungen.Add(fehlermeldung, line);

                }
                if (type.Contains("isfloat") && !isfloat)
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} ist kein float) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung);
                    fehlermeldungen.Add(fehlermeldung, line);

                }
                if (type.Contains("isalphanumeric") && !IsAlphanumeric(spalten))
                {
                    string fehlermeldung = $"Fehler(Spalte {i + 1} ist nicht alphanumerisch) in Zeile:{line} | {spalten}";
                    Console.WriteLine(fehlermeldung); fehlermeldungen.Add(fehlermeldung, line);
                }
                return (spalten, fehlerzahl, fehlermeldungen);
            }
        }
    }
}