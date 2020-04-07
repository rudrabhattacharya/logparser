using IISLogParser;
using Parser.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parser.Implementation
{
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
}