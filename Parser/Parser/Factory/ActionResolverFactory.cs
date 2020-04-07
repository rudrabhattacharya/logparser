using Parser.Implementation;
using Parser.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parser.Factory
{
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
}
