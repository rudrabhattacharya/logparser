using Parser.Implementation;
using Parser.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parser.Factory
{
    public class ConsoleResolverFactory
    {
        public IConsoleResolver GetConsoleResolver(string[] args)
        {
            IConsoleResolver resolver = null;
            switch (args[0])
            {
                case "-f":
                    resolver = new FileConsoleResolver();
                    break;
                case "-d":
                    resolver = new FolderConsoleResolver();
                    break;
                default:
                    break;
            }
            return resolver;
        }
    }
}
