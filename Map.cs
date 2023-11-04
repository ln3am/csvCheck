using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using static SpaltenCheckMethoden.Methods;

namespace Mappings
{
    public class Map
    {
        public List<List<string>> ReadLinesFromTextFiles(string directoryPath)
        {
            List<List<string>> listOfLists = new List<List<string>>();
            string[] excludedFileNames = { "package template.txt" };
            string[] fileEntries = Directory.GetFiles(directoryPath)
                .Where(filePath => !excludedFileNames.Contains(Path.GetFileName(filePath)) && Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            foreach (string filePath in fileEntries)
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();
                listOfLists.Add(lines);
            }
            return listOfLists;
        }
        public List<List<List<string>>> MapList(int spaltenanzahl, int methodenanzahl, List<List<List<string>>> mapList, bool run2, int seperatormultiple, string[] headerline, List<List<string>> packages)
        {
            Type myClassType = typeof(MethodOperation);
            MethodInfo[] methods = myClassType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => method.DeclaringType == myClassType)
                .ToArray();
            methodenanzahl = methods.Length;
            string methodnumber = "";
            int methodparameternumber = 0;
            for (int i = 0; i < spaltenanzahl; i++)
            {
                List<List<string>> innerLists = new List<List<string>>();
                for (int j = 0; j <= methodenanzahl; j++)
                {
                    int packagecount = 0;
                    List<string> values = new List<string>();
                    foreach (var package in packages)
                    {
                        packagecount++;
                        int packagescount = packages.Count;
                        methodnumber = j.ToString();
                        methodparameternumber = 10;
                            //change if a new method has more than 10 inputs or replace with an array to keep track of inividual numbers which requires an input for each new method
                        values = Methods(values, package, headerline, methodnumber, methodparameternumber, i, packagecount, packagescount);
                    }
                    innerLists.Add(values);
                }
                mapList.Add(innerLists);
            }
            return mapList;
        }
        static List<string> Methods(List<string> values, List<string> package, string[] headerline, string methodnumber, int methodparameternumber, int i, int packagecount, int packagescount)
        {
            string[] wordsToCheck = package[0].Split(' ');
            bool containsWord = wordsToCheck.Any(word => headerline[i].Contains(word));
            if (containsWord && package[1].Contains(methodnumber))
            {
                int countthrough = package.Count;
                for (int l = 2; l < countthrough; l++)
                {
                    values.Add(package[l]);
                }

                while (values.Count <= methodparameternumber)
                {
                    values.Add("");
                }
            }
            else if (packagescount == packagecount && values.Count == 0)
            {
                values.Add("false");
            }
            return values;
        }
    }
}