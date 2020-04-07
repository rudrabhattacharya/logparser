using Parser.Factory;
using Parser.Interface;
using System;

namespace logparser
{
    class Program
    {

        //class Options
        //{
        //    [Option("f",Required =false,HelpText ="Enter the filepath for the log file to be parsed")]
        //    public string Filepath { get; set; }

        //    [Option("d", Required = false, HelpText = "Enter the folder containing the log file to be parsed")]
        //    public string Folderpath { get; set; }
        //}
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Log Parser at " + DateTime.Now.ToShortDateString());
            args = new string[] { "-d", @"G:\Servers_IISLog", "-a", "hitcount" };

            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length == 1 && helpRequired(arguments[0]))
            {
                ShowHelp();
                return;
            }
            IConsoleResolver resolver = new ConsoleResolverFactory().GetConsoleResolver(args);
            if (resolver == null)
                return;
            resolver.Resolve(args);

            Console.WriteLine("ending log parser at " + DateTime.Now.ToShortDateString());
        }

        private static void ShowHelp()
        {
            Console.WriteLine("******************************");
            Console.WriteLine("------Argument list-----------");
            Console.WriteLine("-f (full filepath for log file to be parsed)");
            Console.WriteLine("-d (directory or folder to be searched for log files)");
            Console.WriteLine("-a (action to be carried out)");
            Console.WriteLine("-i (ip number to filter)");
            Console.WriteLine("-p (page/path to filter)");
            Console.WriteLine("-o (output path)");
            Console.WriteLine("----------actions list----------");
            Console.WriteLine("errcount (get bifurcation of errors based on status code)");
            Console.WriteLine("hitcount (get count of requests for each application within the site)");
            Console.WriteLine("hitcountbyip (get count of requests from a particular ip");
            Console.WriteLine("hitcountbyip (get count of requests from a particular ip");
            Console.WriteLine("densityperhr (get count of requests for each application per hour)");
            //Console.WriteLine("-a (action to be carried out)");
            //Console.WriteLine("-a (action to be carried out)");
            //Console.WriteLine("-a (action to be carried out)");
            Console.WriteLine("----------actions list----------");
        }      


        private static bool helpRequired(string param)
        {
            return (param == "--help" || param == "/?" || param == "-h");
        }
    }
}
