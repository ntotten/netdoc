using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDoc
{
    public class DocParser
    {

        public DocumentData Parse(string path)
        {
            return Parse(new string[] { path });
        }

        public DocumentData Parse(string[] paths)
        {
            var validFiles = new List<string>();

            foreach (var path in paths)
            {
                if (Directory.Exists(path))
                {
                    AddValidFilesInDirectory(path, validFiles);
                    var directories = Directory.GetDirectories(path);
                    foreach (var directory in directories)
                    {
                        AddValidFilesInDirectory(directory, validFiles);
                    }
                }
                else
                {
                    AddValidFile(path, validFiles);
                }
            }

            return ParseFiles(validFiles.ToArray());
        }

        private static void AddValidFilesInDirectory(string path, List<string> validFiles)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                AddValidFile(file, validFiles);
            }
        }

        private static void AddValidFile(string file, List<string> validFiles)
        {
            if (Path.GetExtension(file) == ".cs")
            {
                validFiles.Add(file);
            }
        }

        private DocumentData ParseFiles(string[] paths)
        {
            var trees = new List<SyntaxTree>();
            foreach (var path in paths)
            {
                var tree = SyntaxTree.ParseFile(path);
                trees.Add(tree);
            };

            return TreeParser.Parse(trees);
        }

    }
}
