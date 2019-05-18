using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkillAlice
{
    class Program
    {
        static void Main(string[] args)
        {
            GetData();
            Console.Read();
        }

        static async void GetData()
        {
            var authContent = new[]{
                        new KeyValuePair<string, string>("token", Hidden.Token),   // App token
                    };
            
            var formContent = new FormUrlEncodedContent(authContent); // Serealize request params

            bool c = await Request.Get(Hidden.Gate, formContent, (s) => { Console.WriteLine(s); }, (e) => { Console.WriteLine(e); });
        }
    }
}
