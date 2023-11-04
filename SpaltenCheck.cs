using System.Text;
using Microsoft.VisualBasic.FileIO;
using SpaltenCheckMethoden;
using Mappings;
using Prechecks;
using System;
using System.Collections.Generic;
using System.IO;
using csvCheck;
using System.Windows.Controls;

namespace SpaltenCheck
{
    public class Checker
    {
        public void SpaltenDelimiterÜberprüfung(string csvFilePath, string csvOut, string txtpackaes, string csvfalselines, TextBox outputFalseText, bool automaticdelimiterkorrektur, double writespeed) 
        {
            Precheck prechecking = new Precheck();
            Map maps = new Map();
            int methodenanzahl = 0;
            List<List<List<string>>> mapping1 = new List<List<List<string>>>();
            List<List<string>> packages = new List<List<string>>(); 
        packages = maps.ReadLinesFromTextFiles(txtpackaes);
            List<List<string>> columns1 = new List<List<string>>();
            List<List<string>> columns2 = new List<List<string>>();
            List<string> prespalten = new List<string>();
            List<string> errorLines = new List<string>();
            List<int> falseSideSeperatorList = new List<int>();
            List<int> falseMainSeperatorList = new List<int>();
            List<int> sharedIllegal = new List<int>();
            Dictionary<string, int> fehlermeldungen = new Dictionary<string, int>();
          string[] candidateSeparators = { ";","," };
          string[] candidateSideSeparators = { "\"","'" };
            int fehleranzahl = 0;
            bool run2;
        var aus1 = prechecking.Seperatorchecker(csvFilePath, candidateSeparators);
            string seperatorMain = aus1.Item1;
            int columnCountMain = aus1.Item2; 
        var aus2 = prechecking.Seperatorchecker(csvFilePath, candidateSideSeparators);
            string seperatorUmValue = aus2.Item1; 
            int columnCountUmValue = aus2.Item2;
            int seperatormultiplier = (aus2.Item2-1) / aus1.Item2; 
        var resultpre = prechecking.VorZeilen(csvFilePath, seperatorMain, columnCountMain, prespalten);
            int prespaltencount = resultpre.Item1;
            string[] headerrow = resultpre.Item2;
            prespalten = resultpre.Item3;
            int endLineCout = prespaltencount + 1;
            run2 = false;
        mapping1 = maps.MapList(columnCountMain, methodenanzahl, mapping1, run2, seperatormultiplier, headerrow, packages);
        falseMainSeperatorList = prechecking.Illegaldetect(seperatorMain, columnCountMain, falseMainSeperatorList, csvFilePath, prespaltencount, prespalten, automaticdelimiterkorrektur);
        falseSideSeperatorList = prechecking.Illegaldetect(seperatorUmValue, columnCountUmValue, falseSideSeperatorList, csvFilePath, prespaltencount, prespalten, automaticdelimiterkorrektur);
        var res = prechecking.SharedIllegalDetect(falseSideSeperatorList, falseMainSeperatorList, sharedIllegal,
                    fehlermeldungen, seperatorMain, seperatorUmValue);
            sharedIllegal = res.Item1;
            fehlermeldungen = res.Item2;
            run2 = false;
            //stand
        var result1 = Filereader<string>(prespalten, prespaltencount, endLineCout, falseMainSeperatorList, columns1, seperatorUmValue, seperatorMain, columnCountMain, csvFilePath, run2, automaticdelimiterkorrektur, fehleranzahl, mapping1, seperatormultiplier, columnCountUmValue, fehlermeldungen, sharedIllegal, errorLines);
            columns1 = result1.Item1;
            endLineCout = result1.Item3;
            fehleranzahl += result1.Item4;
            fehlermeldungen = result1.Item5;
            errorLines = result1.Item6;
            run2 = true;
        var result2 = Filereader<string>(prespalten, prespaltencount, endLineCout, falseMainSeperatorList, columns2, seperatorMain, seperatorUmValue, columnCountUmValue, csvFilePath, run2, automaticdelimiterkorrektur, fehleranzahl, mapping1, seperatormultiplier, columnCountMain, fehlermeldungen, sharedIllegal, errorLines);
            columns2 = result2.Item1;
            prespalten = result2.Item2;
            endLineCout = result2.Item3;
            fehleranzahl += result2.Item4;
            fehlermeldungen = result2.Item5;
         errorLines = CsvWriter(csvOut, prespalten, prespaltencount, endLineCout, falseMainSeperatorList, columnCountMain, columnCountUmValue, seperatormultiplier, seperatorMain, seperatorUmValue, sharedIllegal, columns1, columns2, csvfalselines, errorLines, fehlermeldungen);    
            Console.WriteLine($"Korrektur abgeschlossen. Es wurden {fehleranzahl} Fehler erkannt");
            Console.WriteLine("Ausgelassende fehler:");
            foreach (var fehler in fehlermeldungen)
            {
                Console.WriteLine($"{fehler.Key}");
            }
            ControlWriter controlWriter = new ControlWriter(outputFalseText, writespeed);
            foreach (var line in errorLines) 
            {
                controlWriter.WriteLine(line);
            }
        }
        static (List<List<string>>, List<string>, int, int, Dictionary<string, int>, List<string>) Filereader<T>(List<string> prespalten1, int prespaltencount1, int endline, List<int> illegaldelimiterlist1, List<List<string>> columns1, string falsedelimiter, string realdelimiter, int spaltendelimiterlength, string csvFilePath, bool run2, bool automaticdelimiterkorrektur, int fehleranzahl,  List<List<List<string>>> mapping, int seperatormultiple, int delimitersecondlength, Dictionary<string, int> fehlermeldungen, List<int> sharedIllegalList, List<string> errorLines)
        {
            Methods Method = new Methods();
            using (TextFieldParser parser = new TextFieldParser(csvFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(realdelimiter);
                parser.TrimWhiteSpace = false;  
                parser.HasFieldsEnclosedInQuotes = false;
                int spaltenindex = 0;
                int phzahl = 0;
                for (int i = 0; i < spaltendelimiterlength; i++)
                {
                    columns1.Add(new List<string>());
                }
                foreach (var element in prespalten1)
                {
                    parser.ReadFields();
                }
                int line = prespaltencount1 + 1;
                while (!parser.EndOfData)
                {
                    if (((!illegaldelimiterlist1.Contains(line) && !run2) || (run2 && illegaldelimiterlist1.Contains(line))) && !sharedIllegalList.Contains(line))
                    {
                        string[] spalten = parser.ReadFields();
                        for (int i = 0; i < spaltendelimiterlength; i++)
                        {
                            if (spalten[i].Contains(falsedelimiter) && (automaticdelimiterkorrektur || (run2 && 0 == i % seperatormultiple)))
                            {
                                spalten[i] = spalten[i].Replace(falsedelimiter, "");
                            }
                            if (!run2 && spalten[i].Contains(falsedelimiter))
                            {
                                int removeindexofstring = seperatormultiple / 2;
                                
                                spalten[i] = $"{spalten[i][removeindexofstring..^removeindexofstring]}";
                               
                            }
                            string spaltenin = spalten[i];
                            var ausgabe = Method.SpaltenMapping(mapping, spaltenin, i, line, fehleranzahl, run2, seperatormultiple, realdelimiter, spaltendelimiterlength, falsedelimiter, fehlermeldungen);
                            spalten[i] = ausgabe.Item1;
                            fehleranzahl = ausgabe.Item2;
                            fehlermeldungen = ausgabe.Item3;
                            columns1[i].Add(spalten[i]);
                        }
                        endline++;
                    }
                    else if (!run2)
                    {
                        phzahl++;
                        for (int i = 0; i < spaltendelimiterlength; i++)
                        {
                            columns1[i].Add($"placeholder {phzahl}");
                        }
                        string linie = parser.ReadLine();
                        if (sharedIllegalList.Contains(line) && !run2)
                        {
                            errorLines.Add(linie);
                        }
                    }
                    else
                    {
                        parser.ReadLine();
                    }
                    line++;
                }
                return (columns1, prespalten1, endline, fehleranzahl, fehlermeldungen, errorLines);
            }
        }
        static List<string> CsvWriter(string csvOut, List<string> prespalten, int prespaltencount, int endLineCout, List<int> falseMainSeperatorList, int columnCountMain, int columnCountUmValue, int seperatormultiplier, string seperatorMain, string seperatorUmValue, List<int> sharedIllegal, List<List<string>> columns1, List<List<string>> columns2, string csvfalselines, List<string> errorLines, Dictionary<string, int> fehlermeldungen)
        {
            using (StreamWriter writer = new StreamWriter(csvOut))
            {
                foreach (var spalte in prespalten)
                {
                    writer.WriteLine(spalte);
                }
                int cglist = 0;
                for (int line = 0; line + prespaltencount + 1 < endLineCout; line++)
                {
                    StringBuilder row = new StringBuilder();
                    if (!falseMainSeperatorList.Contains(line + prespaltencount + 1))
                    {
                        for (int column = 0; column < columnCountMain; column++)
                        {
                            for (int i = 0; i < seperatormultiplier/2; i++)
                            {
                                row.Append(seperatorUmValue);
                            }
                            row.Append(columns1[column][line]);
                            for (int i = 0; i < seperatormultiplier/2; i++)
                            {
                                row.Append(seperatorUmValue);
                            }
                            if (column < columnCountMain - 1)
                            {
                                    row.Append(seperatorMain);
                            }
                        } 
                    }
                    else if (!sharedIllegal.Contains(line + prespaltencount + 1))
                    {
                        for (int column = 0; column < columnCountUmValue; column++)
                        {
                            if (column != 0 && column % seperatormultiplier == 0 && column != columnCountUmValue - 1)
                            {
                                row.Append(seperatorMain);
                            }
                            row.Append(columns2[column][cglist]);
                            if (column < columnCountUmValue - 1)
                            {
                              row.Append(seperatorUmValue);
                            }
                        }
                        cglist++;
                    }
                    if (!fehlermeldungen.ContainsValue(line+prespaltencount+1))
                    {
                        writer.WriteLine(row);
                    }
                    else if (!sharedIllegal.Contains(line + prespaltencount + 1) && !row.ToString().Contains("placeholder"))
                    {
                        errorLines.Add(row.ToString());
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(csvfalselines))
            {
                foreach (var line in errorLines)
                {
                    writer.WriteLine(line);
                }
            }
            return errorLines;
        }
    } 
}