using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillAlice.Models
{
    class GetModel
    {
        Meta meta;

        double version = 0;

        class Meta
        {
            string locale = "";
            string timezone = "";
            string client_id = "";
            Dictionary<string, string> interfaces;                          
        }

        class Request
        {
            string command = "";
            string original_utterance = "";
            string type = "";
            Markup markup;
            Payload payload;
            Nlu nlu;

            class Markup
            {
                bool dangerous_context = true;
            }

            class Payload { }

            class Nlu
            {
                string[] tokens;
                Entities[] entities;

                class Entities
                {
                    Tokens[] tokens;
                    string type;
                    string value;

                    class Tokens
                    {
                        string start;
                        string end;
                    }
                }
            }
        }

        class Session
        {
            bool New = false;
            string message_id = "";
            string session_id = "";
            string skill_id = "";
            string user_id = "";
        }
    }

    class PostModel
    {

    }
}
