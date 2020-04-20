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
    /// the number of hits against the application
    /// </summary>
    public class ClientIPCountResolver : IActionResolver
    {
        public void Resolve(IList<string> files)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ClientIP,HitCount");
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
                                var cip = logEvent.cIp;
                                if (!string.IsNullOrEmpty(cip))
                                {
                                    if (hashTable.ContainsKey(cip))
                                    {
                                        hashTable[cip] = (int)hashTable[cip] + 1;
                                    }
                                    else
                                    {
                                        hashTable.Add(cip, 1);
                                    }
                                }
                               
                            }

                        }

                    }

                }
            }
            var dict = hashTable.Cast<DictionaryEntry>().ToDictionary(d => d.Key, d => d.Value);
            var sortedDictionary = dict.OrderByDescending(x => x.Value);

            foreach (var keyVal in sortedDictionary)
            {
                builder.AppendLine($"{keyVal.Key},{keyVal.Value}");
                Console.WriteLine($"--- {keyVal.Key} has count {keyVal.Value}");
            }
            sw.Stop();
            Console.WriteLine($"Total time taken for processing is {sw.ElapsedMilliseconds / 1000} seconds");
            File.WriteAllText($"./{DateTime.Now.ToShortDateString().Replace(' ', '-')}_output.csv", builder.ToString());
        }
    }
}
