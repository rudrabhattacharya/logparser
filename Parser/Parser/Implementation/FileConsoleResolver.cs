using Parser.Factory;
using Parser.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser.Implementation
{
    public class FileConsoleResolver : IConsoleResolver
    {
        public void Resolve(string[] args)
        {
            if (args.Length > 1)
            {
                var filepath = args[1];
                IList<string> files = new List<string>();

                if (Directory.Exists(filepath))
                {
                    // this is a directory. find all files and iterate through the list
                    foreach (string filename in Directory.GetFiles(filepath, "*.log"))
                    {
                        files.Add(filename);
                    }
                }
                else
                {
                    files.Add(filepath);
                }
                IActionResolver resolver = new ActionResolverFactory().GetActionResolver(args);
                resolver.Resolve(files);

            }
        }
    }
}
