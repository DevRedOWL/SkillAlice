using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillAlice.Models
{

    [Serializable]
    public class UserModel
    {
        public static Dictionary<string, UserModel> List = new Dictionary<string, UserModel>();
        // Предзагрузка
        public static Dictionary<string, FilmModel> Prerender = new Dictionary<string, FilmModel>();
        // Возрастная категория
        public bool SelectingAge = false;
        public bool Adult = false;
        public bool AskAboutAdult = false;        
        // Выбор фильма
        public bool SelectingFilm = false;
        public string SelectingFilmName = "";
        // Выбор из списка
        public bool SelectingList = false;
    }

    // Time get for init
    public class TimeModel
    {
        public double unix = 0;
    }

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
            public Card card;
            public Buttons[] buttons;

            public class Buttons
            {
                public string title = "";
                //public Payload payload;
                public string url;
                public bool hide = false;

                public class Payload { }
            }

            public class Card
            {
                public string type = "";
                public string image_id = "";
                public string title = "";
                public string description = "";
                public Button button;

                public class Button
                {
                    public string text = "";
                    public string url = "";
                }
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

    public class FilmModel
    {
        public Data data;

        public class Data
        {
            public Showcases[] showcases;
            public int total = 0;
            public object[] links;
            public string type = "";
            public string urn = "";
            public string title = "";
            public References[] references;

            public class Showcases
            {
                public Items[] items;
                public int total = 0;
                public object[] links;
                public string type = "";
                public string urn = "";
                public string title = "";

                public class Items
                {
                    public string id;
                    public string type = "";
                    public string urn = "";
                    public string title = "";
                    public Resources[] resources;
                    public string releasedAt = "";
                    public Rating rating;
                    public Favorite favorite;
                    public string personsUrn = "";
                    public object[] categories;
                    public string offersUrn = "";
                    public Available available;
                    public Offer offer;
                    public string description = "";
                    public string duration = "";
                    public string position = "";
                    public Estimate estimate;

                    public class Resources
                    {
                        public string id = "";
                        public string type = "";
                        public string drm = "";
                    }

                    public class Rating
                    {
                        public string type = "";
                    }

                    public class Favorite
                    {
                        public string type = "";
                    }

                    public class Available
                    {
                        public string type = "";
                    }

                    public class Offer
                    {
                        public string id = "";
                        public string type = "";
                        public string urn = "";
                        public string price = "";
                        public string purchaseAt = "";
                        public string expireAt = "";
                        public Ivod ivod;
                        public Status status;
                        public Provider provider;
                        public Period period;
                        public Adult adult;
                        public Root root;
                        public object[] stocks;
                        public Quality[] quality;
                        public Store store;
                        public Source source;

                        public class Ivod
                        {
                            public string type = "";
                        }
                        public class Status
                        {
                            public string type = "";
                        }
                        public class Provider
                        {
                            public string id = "";
                            public string title = "";
                            public object[] resources;
                        }
                        public class Period
                        {
                            public string type = "";
                            public string value = "";
                        }
                        public class Adult
                        {
                            public string type = "";
                        }
                        public class Root
                        {
                            public string id = "";
                            public string type = "";
                            public string urn = "";
                            public object[] resources;
                        }
                        public class Quality
                        {
                            public string type = "";
                        }
                        public class Store
                        {
                            public string type = "";
                        }
                        public class Source
                        {
                            public string type = "";
                            public string name = "";
                        }
                    }

                    public class Estimate
                    {
                        public string type = "";
                    }

                }
            }

            public class References
            {
                public string title = "";
                public string urn = "";
                public string type = "";
                public int total = 1;
            }
        }
        
    }
    
}
