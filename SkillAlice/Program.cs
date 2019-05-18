using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkillAlice
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                // Get
                Request.POST(Hidden.Gate + Hidden.Methods.Get,
                    // Convert to HttpContent
                    Request.ToHttpContent(new KeyValuePair<string, string>[]{
                        new KeyValuePair<string, string>("token", Hidden.Token)   // App token
                    }),
                    // Success callback
                    (data) =>
                    {
                        // Make Model
                        //Console.WriteLine(data);
                        Models.GetModel state = JsonConvert.DeserializeObject<Models.GetModel>(data);
                        // Check if unix time already ready
                        if (Hidden.LastUnix != state.unix)
                        {
                            // Set Unix time
                            Hidden.LastUnix = state.unix;
                            // Debug log
                            Console.WriteLine("New request: " + state.request.original_utterance + data);
                            // Set
                            Request.POST(Hidden.Gate + Hidden.Methods.Send,
                                // Convert to HttpContent
                                Request.ToHttpContent(
                                    // Create an object
                                    new Models.PostModel() {
                                        response = new Models.PostModel.Response()
                                        {
                                            text = state.request.original_utterance,
                                            tts = state.request.original_utterance,
                                            buttons = new[]
                                            {
                                                new Models.PostModel.Response.Buttons
                                                {
                                                    title = "тест",
                                                    hide = true
                                                },
                                                new Models.PostModel.Response.Buttons
                                                {
                                                    title = "тест2",
                                                    hide = true
                                                } 
                                            }
                                        },
                                        session = new Models.PostModel.Session()
                                        {                                            
                                            session_id = state.session.session_id,
                                            skill_id = state.session.skill_id,
                                            message_id = state.session.message_id,
                                            user_id = state.session.user_id
                                        },
                                        version = state.version
                                    }),
                                // Success callback
                                (suc) =>
                                {
                                    // Успешный ответ
                                },
                                // Error callback
                                (err) =>
                                {
                                    Console.WriteLine(err);
                                }
                            );
                        }
                        else
                        {
                            //Console.WriteLine("Такой уних уже существует");
                        }
                    },
                    // Error callback
                    (code) =>
                    {
                        Console.WriteLine(code);
                    }
                );

                Thread.Sleep(100);
            }
            

            

            Console.Read();
        }


        
        

    }
}
