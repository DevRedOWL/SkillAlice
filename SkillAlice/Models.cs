using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillAlice.Models
{
    // Model that we get from alice
    public class GetModel
    {
        public Meta meta;
        public Request request;
        public Session session;
        public string version = "1.0";
        public double unix = 0;

        public class Meta
        {
            public string locale = "";
            public string timezone = "";
            public string client_id = "";
            //public Dictionary<string, string> interfaces;                          
        }


        public class Request
        {
            public string command = "";
            public string original_utterance = "";
            public string type = "";
            public Markup markup;
            public Payload payload;
            public Nlu nlu;

            public class Markup
            {
                public bool dangerous_context = true;
            }

            public class Payload { }

            public class Nlu
            {
                public string[] tokens;
                public Entities[] entities;

                public class Entities
                {
                    public Tokens tokens;
                    public string type;
                    public object value;

                    public class Tokens
                    {
                        public string start;
                        public string end;
                    }
                }
            }
        }

        public class Session
        {
            public bool New = true;
            public double message_id = 0;
            public string session_id = "";
            public string skill_id = "";
            public string user_id = "";
        }
    }

    // Model that we send to server
    public class PostModel
    {
        public string version = "1.0";
        public Session session;
        public Response response;            

        public class Response
        {
            public bool end_session = false;
            public string text = "";
            public string tts = "";
            public Buttons[] buttons;

            public class Buttons
            {
                public string title = "";
                //public Payload payload;
                //public string url;
                public bool hide = false;

                public class Payload { }
            }
        }

        public class Session
        {
            public double message_id = 0;
            public string session_id = "";
            public string skill_id = "";          
            public string user_id = "";
        }
        
    }
}
