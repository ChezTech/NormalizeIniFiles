using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NormalizeIniFiles
{
    class Normalizer
    {
        public void NormalizeFile(string filePath)
        {
            // Read lines, find section, lines in section, sort sections, sort lines
            // TODO: don't forget comments

            var lines = File.ReadAllLines(filePath);

            var parser = new IniParser();
            var iniDict = parser.ParseRawLines(lines);


            var writer = new IniWriter();
            var fileLines = writer.WriteNormalizedIni(iniDict);

            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, fileLines);

            // todo: make arg option
            //var fileBak = Path.ChangeExtension(filePath, ".ini.bak");
            //File.Move(filePath, fileBak);
            File.Move(tempFile, filePath, true);
        }

        class IniWriter
        {
            public List<string> WriteNormalizedIni(IDictionary<IniSection, ISet<IIniLine>> iniDict)
            {
                var fileLines = new List<string>();

                IniSection lastKey = null;

                foreach (var item in iniDict)
                {
                    if (lastKey != item.Key)
                    {
                        lastKey = item.Key;
                        WriteIniLine(fileLines, item.Key);
                    }

                    foreach (var line in item.Value)
                    {
                        WriteIniLine(fileLines, line);
                    }
                }
                return fileLines;
            }

            private void WriteIniLine(List<string> fileLines, IIniLine iniLine)
            {
                if (iniLine.Comment != null)
                    fileLines.Add(iniLine.Comment);
                fileLines.Add(iniLine.Line);
            }
        }

        class IniParser
        {
            private List<char> CommentChars = new List<char>() { ';', '#' };
            private readonly char SectionChar = '[';
            private readonly string DefaultSection = "[<default>]";

            public IDictionary<IniSection, ISet<IIniLine>> ParseRawLines(string[] lines)
            {
                IDictionary<IniSection, ISet<IIniLine>> iniDict = new SortedDictionary<IniSection, ISet<IIniLine>>();
                string lastComment = null;
                ISet<IIniLine> sectionLines = null;
                foreach (var line in lines)
                {
                    if (IsComment(line))
                    {
                        lastComment = line;  // append for mult?
                        continue;
                    }

                    if (IsSection(line))
                    {
                        sectionLines = AddNewSection(iniDict, lastComment, line);
                        lastComment = null;
                        continue;
                    }

                    //  Should always have a section, but just in case the file starts with keys right away
                    if (sectionLines == null)
                    {
                        AddNewSection(iniDict, lastComment, DefaultSection);
                    }

                    // Now, add our 'key' line to our Section
                    var iniLine = new IniKey() { Line = line, Comment = lastComment };
                    sectionLines.Add(iniLine);

                    lastComment = null;
                }

                return iniDict;
            }

            private static ISet<IIniLine> AddNewSection(IDictionary<IniSection, ISet<IIniLine>> iniDict, string lastComment, string line)
            {
                ISet<IIniLine> sectionLines;
                var iniSection = new IniSection() { Line = line, Comment = lastComment };
                sectionLines = new SortedSet<IIniLine>();
                iniDict.Add(iniSection, sectionLines);
                return sectionLines;
            }

            private bool IsComment(string line)
            {
                return CommentChars.Contains(line[0]);
            }

            private bool IsSection(string line)
            {
                return (line[0] == SectionChar);
            }

        }

        interface IIniLine : IComparable<IIniLine>
        {
            string Line { get; set; }
            string Comment { get; set; }
        }

        class IniLineBase : IIniLine
        {
            public string Comment { get; set; }
            public string Line { get; set; }

            public int CompareTo([AllowNull] IIniLine other)
            {
                if (other == null) return 1;
                return Line.CompareTo(other.Line);
            }
        }

        class IniKey : IniLineBase
        {
        }

        class IniSection : IniLineBase
        {
        }
    }
}

