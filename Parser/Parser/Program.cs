using IISLogParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        public interface IActionResolver
        {
            void Resolve(IList<string> files);
        }

        public class ErrorCountResolver : IActionResolver
        {
            public void Resolve(IList<string> files)
            {
                foreach (var item in files)
                {
                    if (File.Exists(item))
                    {
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();

                        List<IISLogEvent> logs = new List<IISLogEvent>();

                        using (ParserEngine parser = new ParserEngine(item))
                        {
                            Hashtable hashTable = new Hashtable();
                            while (parser.MissingRecords)
                            {
                                logs = parser.ParseLog().ToList();

                                // bifurcate via status code
                                // to find out files that served with errors. search for status as 500
                                foreach (IISLogEvent logEvent in logs)
                                {
                                    //if (logEvent.scStatus.Equals(500))
                                    {
                                        if (hashTable.ContainsKey(logEvent.scStatus))
                                        {
                                            hashTable[logEvent.scStatus] = (int)hashTable[logEvent.scStatus] + 1;
                                        }
                                        else
                                        {
                                            hashTable.Add(logEvent.scStatus, 1);
                                        }
                                    }
                                }
                            }
                            foreach (var key in hashTable.Keys)
                            {
                                Console.WriteLine($"--- {key} has count {hashTable[key]}");
                            }
                        }
                        sw.Stop();
                        Console.WriteLine($"Total time taken for processing is {sw.ElapsedMilliseconds / 1000} seconds");
                    }
                }
            }
        }



        /// <summary>
        /// Provides the count of requests by ip
        /// </summary>
        public class HitCountByIpResolver : IActionResolver
        {
            public void Resolve(IList<string> files)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var item in files)
                {
                    if (File.Exists(item))
                    {
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();

                        List<IISLogEvent> logs = new List<IISLogEvent>();

                        using (ParserEngine parser = new ParserEngine(item))
                        {
                            Hashtable hashTable = new Hashtable();
                            while (parser.MissingRecords)
                            {
                                logs = parser.ParseLog().ToList();
                                // to find out files that match a certain pattern, get the csuri
                                // bifurcate via csuristem
                                foreach (IISLogEvent logEvent in logs)
                                {
                                    //if (logEvent.scStatus.Equals(500))
                                    {
                                        if (hashTable.ContainsKey(logEvent.cIp))
                                        {
                                            hashTable[logEvent.cIp] = (int)hashTable[logEvent.cIp] + 1;
                                        }
                                        else
                                        {
                                            hashTable.Add(logEvent.cIp, 1);
                                        }
                                    }
                                }

                            }
                            //if(File.Exists())

                            builder.AppendLine("AppName,HitCount");
                            foreach (var key in hashTable.Keys)
                            {
                                builder.AppendLine($"{key},{hashTable[key]}");
                                Console.WriteLine($"--- {key} has count {hashTable[key]}");
                            }

                        }
                        sw.Stop();
                        Console.WriteLine($"Total time taken for processing is {sw.ElapsedMilliseconds / 1000} seconds");
                    }
                }
                File.WriteAllText(@"./output.txt", builder.ToString());
            }
        }

        /// <summary>
        /// finds out the visits to each site based on hour
        /// </summary>
        public class TrafficDensityResolver : IActionResolver
        {
            public void Resolve(IList<string> files)
            {
                foreach (var item in files)
                {
                    if (File.Exists(item))
                    {
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();

                        List<IISLogEvent> logs = new List<IISLogEvent>();

                        using (ParserEngine parser = new ParserEngine(item))
                        {
                            Hashtable hashTable = new Hashtable();
                            while (parser.MissingRecords)
                            {
                                logs = parser.ParseLog().ToList();
                                // to find out files that match a certain pattern, get the csuri
                                // bifurcate via csuristem
                                foreach (IISLogEvent logEvent in logs)
                                {
                                    var splitParts = logEvent.csUriStem.Split('/');
                                    //DateTime datetime = logEvent.DateTimeEvent;
                                    DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(logEvent.DateTimeEvent, INDIAN_ZONE);
                                    //DateTimeOffset offset = new DateTimeOffset(logEvent.DateTimeEvent, TimeSpan.FromHours(-5).Add(TimeSpan.FromMinutes(-30)));
                                    // 5p  - 2020-02-2010 - 01:00:00
                                    if (splitParts.Length > 1)
                                    {
                                        string sitekey = splitParts[1].ToLower();
                                        string subsitekey = datetime.ToShortDateString() + " " + datetime.Hour + ":00:00";
                                        if (hashTable.ContainsKey(sitekey))
                                        {
                                            Hashtable innerHashtable = (Hashtable)hashTable[sitekey];
                                            if (innerHashtable.ContainsKey(subsitekey))
                                            {
                                                innerHashtable[subsitekey] = Convert.ToInt32(innerHashtable[subsitekey]) + 1;
                                            }
                                            else
                                            {
                                                // first entry for this time period
                                                innerHashtable.Add(subsitekey, 1);
                                            }
                                            hashTable[sitekey] = innerHashtable;
                                        }
                                        else
                                        {
                                            var innerHashtable = new Hashtable();
                                            innerHashtable.Add(subsitekey, 1);
                                            hashTable.Add(sitekey, innerHashtable);
                                        }
                                        //}
                                    }
                                    //if (logEvent.scStatus.Equals(500))                                    
                                }

                            }
                            foreach (var key in hashTable.Keys)
                            {
                                Console.WriteLine($"------ {key} -------");

                                Hashtable innerHashtable = (Hashtable)hashTable[key];
                                foreach (var hour in innerHashtable.Keys)
                                {
                                    Console.WriteLine($"----hit count for {hour} was {(int)innerHashtable[hour]}----");
                                }
                                Console.WriteLine($"--------------");
                            }
                        }
                        sw.Stop();
                        Console.WriteLine($"Total time taken for processing is {sw.ElapsedMilliseconds / 1000} seconds");
                    }
                }
            }
        }

        /// <summary>
        /// the number of hits against the application
        /// </summary>
        public class ApplicationHitCountResolver : IActionResolver
        {
            public void Resolve(IList<string> files)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("AppName,HitCount");
                Hashtable hashTable = new Hashtable();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                foreach (var item in files)
                {
                    if (File.Exists(item))
                    {
                        Console.WriteLine($"Reading file {item}");


                        List<IISLogEvent> logs = new List<IISLogEvent>();

                        using (ParserEngine parser = new ParserEngine(item))
                        {

                            while (parser.MissingRecords)
                            {
                                logs = parser.ParseLog().ToList();
                                // to find out files that match a certain pattern, get the csuri
                                // bifurcate via csuristem
                                foreach (IISLogEvent logEvent in logs)
                                {
                                    var splitParts = logEvent.csUriStem.Split('/');

                                    if (splitParts.Length > 1)
                                    {
                                        string key = splitParts[1].ToLower();
                                        if (key.Equals(string.Empty))
                                        {
                                            key = "/";
                                        }
                                        if (hashTable.ContainsKey(key))
                                        {
                                            hashTable[key] = (int)hashTable[key] + 1;
                                        }
                                        else
                                        {
                                            hashTable.Add(key, 1);
                                        }
                                        //}
                                    }
                                }

                            }

                        }

                    }
                }
                var dict = hashTable.Cast<DictionaryEntry>().ToDictionary(d => d.Key, d => d.Value);
                var sortedDictionary = dict.OrderByDescending(x => x.Value).Take(35);

                foreach (var keyVal in sortedDictionary)
                {
                    builder.AppendLine($"{keyVal.Key},{keyVal.Value}");
                    Console.WriteLine($"--- {keyVal.Key} has count {keyVal.Value}");
                }
                sw.Stop();
                Console.WriteLine($"Total time taken for processing is {sw.ElapsedMilliseconds / 1000} seconds");
                File.WriteAllText($"./{DateTime.Now.ToString().Replace(' ', '-')}_output.csv", builder.ToString());
            }
        }


        public class ActionResolverFactory
        {
            public IActionResolver GetActionResolver(string[] args)
            {
                IActionResolver resolver = null;
                switch (args[3])
                {
                    case "errcount":
                        resolver = new ErrorCountResolver();
                        break;
                    case "hitcount":
                        resolver = new ApplicationHitCountResolver();
                        break;
                    case "hitcountbyip":
                        resolver = new HitCountByIpResolver();
                        break;
                    case "densityperhr":
                        resolver = new TrafficDensityResolver();
                        break;
                    default:
                        break;
                }
                return resolver;
            }
        }

        public interface IConsoleResolver
        {
            void Resolve(string[] args);
        }

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

        public class FolderConsoleResolver : IConsoleResolver
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


        private static bool helpRequired(string param)
        {
            return (param == "--help" || param == "/?" || param == "-h");
        }
    }
}
