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
    /// finds out the visits to each site based on hour
    /// </summary>
    public class TrafficDensityResolver : IActionResolver
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
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
}
