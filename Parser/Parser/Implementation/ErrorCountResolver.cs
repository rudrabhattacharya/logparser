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
}
