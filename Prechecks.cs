using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prechecks
{
    public class Precheck
    {
        public (string, int) Seperatorchecker(string csvFilePath, string[] candidateSeparators)
        {
            string insep = "";
            int sepcount = 0;
            Dictionary<string, Dictionary<string, int>> seperatorListDictionary = new Dictionary<string, Dictionary<string, int>>();
            foreach (var seperator in candidateSeparators)
            { 
                using (TextFieldParser parser = new TextFieldParser(csvFilePath)) 
                {
                    parser.HasFieldsEnclosedInQuotes = false;
                    parser.TrimWhiteSpace = false;
                    Dictionary<string, List<string>> linesBySeparatorCountDictionary =
                        new Dictionary<string, List<string>>();
                    var seperatorDictionary = new Dictionary<string, int>();
                    int rowCount = 0;
                    while (!parser.EndOfData && rowCount < 1000)
                    {
                        string line = parser.ReadLine();
                        if (line != null && line.Length > 0)
                        {
                            int separatorCount = line.Split(new[] { seperator }, StringSplitOptions.None).Length;
                            if (separatorCount > 1)
                            {
                                if (!linesBySeparatorCountDictionary.ContainsKey(separatorCount.ToString()))
                                {
                                    linesBySeparatorCountDictionary[separatorCount.ToString()] = new List<string>();
                                }

                                linesBySeparatorCountDictionary[separatorCount.ToString()].Add(rowCount.ToString());
                                rowCount++;
                            }
                        }
                    }
                    if (linesBySeparatorCountDictionary.Count != 0)
                    {
                        var mostCommonSeparator = linesBySeparatorCountDictionary
                            .OrderByDescending(kvp => kvp.Value.Count)
                            .FirstOrDefault();
                        seperatorDictionary.Add(mostCommonSeparator.Key, mostCommonSeparator.Value.Count);
                        seperatorListDictionary.Add(seperator, seperatorDictionary);
                    } 
                }
            } 
            var result = seperatorListDictionary
                .SelectMany(outer => outer.Value, (outer, inner) => new
                    { OuterKey = outer.Key, InnerKey = inner.Key, InnerValue = inner.Value })
                .OrderByDescending(item => item.InnerValue).FirstOrDefault();
            insep = result.OuterKey; 
            sepcount = int.Parse(result.InnerKey); 
            return (insep, sepcount);
        }
        public (int, string[], List<string>) VorZeilen(string csvFilePath, string seperator, int columncount, List<string> prespalten)
        {
            string[] empty = {""};
            using (TextFieldParser parser = new TextFieldParser(csvFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(seperator);
                parser.HasFieldsEnclosedInQuotes = false;
                parser.TrimWhiteSpace = false;
                for (int i = 1; i < 1000; i++)
                {
                    string[] field = parser.ReadFields();
                    prespalten.Add(string.Join(seperator, field));
                    if (field.Length == columncount)
                    {
                        return (i, field, prespalten);
                    }
                }
            }
            return (0, empty, prespalten);
        }
        public List<int> Illegaldetect(string zeichendelimiter, int delimiterlength, List<int> illegallist, string csvFilePath, int vorspaltencount, List<string> prespalten, bool autodelimiterkorrektur)
        {
            using (TextFieldParser parser = new TextFieldParser(csvFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(zeichendelimiter);
                int line = vorspaltencount;
                parser.TrimWhiteSpace = false;
                parser.HasFieldsEnclosedInQuotes = false;
                foreach (var li in prespalten)
                {
                    parser.ReadFields();
                }
                while (!parser.EndOfData)
                {
                    line++;
                    string[] spalten = parser.ReadFields();
                    if (spalten.Length != delimiterlength)
                    {
                        if (autodelimiterkorrektur) 
                        { 
                        Console.WriteLine($"Zeile:{line} besitzt ein illegales {zeichendelimiter} Zeichen");
                        }
                        illegallist.Add(line);
                    }
                }
                return illegallist;
            }
        }

        public (List<int>, Dictionary<string, int>) SharedIllegalDetect(List<int> falseSideSeperatorList, List<int> falseMainSeperatorList, List<int> sharedIllegal, Dictionary<string, int> fehlermeldungen, string seperatorMain, string seperatorUmValue)
        {
            if (!(falseSideSeperatorList.Count == 0 && falseMainSeperatorList.Count == 0))
            {
                foreach (var item1 in falseMainSeperatorList)
                {
                    foreach (var item2 in falseSideSeperatorList)
                    {
                        if (item1 == item2)
                        {
                            sharedIllegal.Add(item1);
                            string fehlermeldung =
                                $"Fehler(Zeile {item1} hat geteilte illegale Zeichen {seperatorMain} und {seperatorUmValue})";
                            Console.WriteLine(fehlermeldung);
                            fehlermeldungen.Add(fehlermeldung, item1);
                        }
                    }
                }
            }
            return (sharedIllegal, fehlermeldungen);
        }
    }
}