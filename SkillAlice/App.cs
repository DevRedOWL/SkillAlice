using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SkillAlice
{
    class App
    {

        /** Initialize **/
        public static void Init()
        {
            Request.POST(Hidden.Gate + Hidden.Methods.Get,
               // Convert to HttpContent
               Request.ToHttpContent(new KeyValuePair<string, string>[]{
                        new KeyValuePair<string, string>("token", Hidden.Token)   // App token
               }),
                // Success callback
                (data) =>
                {
                    Models.TimeModel state = JsonConvert.DeserializeObject<Models.TimeModel>(data);
                    Hidden.LastUnix = state.unix;
                },
                (code) =>
                {
                    Console.WriteLine(code);
                });
            // Open user settings
            FileStream fs = new FileStream($"{Hidden.Path}/UserInfo.ini", FileMode.OpenOrCreate); // Создаем поток
            BinaryFormatter bf = new BinaryFormatter(); // Включаем бинарное форматирование
            try
            {
                Models.UserModel.List = (Dictionary<string, Models.UserModel>)bf.Deserialize(fs);
            }
            catch (Exception)
            {
                Models.UserModel.List.Add("Empty", new Models.UserModel());
                bf.Serialize(fs, Models.UserModel.List); // Сериализуем
            }
            fs.Close();
        }

        /** Check for updates **/
        public static void CheckForUpdates()
        {
            // Get updates from server
            Request.POST(Hidden.Gate + Hidden.Methods.Get,
                // Convert to HttpContent
                Request.ToHttpContent(new KeyValuePair<string, string>[]{
                        new KeyValuePair<string, string>("token", Hidden.Token)   // App token
                }),
                // Success callback
                (data) =>
                {
                    // Make Model
                    Models.GetModel state = JsonConvert.DeserializeObject<Models.GetModel>(data);
                    // Check if this case was not already handled
                    if (Hidden.LastUnix != state.unix)
                    {
                        // Debug log
                        Console.WriteLine("New request: " + state.request.original_utterance + data);
                        // Handle this case
                        Handle(state);
                    }
                },
                // Error callback
                (code) =>
                {
                    Console.WriteLine(code);
                }
            );
        }

        /** Handle cases **/
        public static void Handle(Models.GetModel state)
        {
            Hidden.LastUnix = state.unix;    // Set Unix time  
            
            // Check if user exist
            if (!Models.UserModel.List.ContainsKey(state.session.user_id))
                Models.UserModel.List.Add(state.session.user_id, new Models.UserModel());
            
            // Creating object                                                                                                                                             
            Models.PostModel AnswerModel = new Models.PostModel()
            {
                response = new Models.PostModel.Response()
                {
                    text = "",
                    tts = ""
                },
                session = new Models.PostModel.Session()
                {
                    session_id = state.session.session_id,
                    skill_id = state.session.skill_id,
                    message_id = state.session.message_id,
                    user_id = state.session.user_id
                },
                version = state.version
            };

            /** Logics **/

            // Если пользователь в меню выбора и выбирает положительный ответ
            if (Dictionary.Correct.Contains(state.request.nlu.tokens[0].ToLower()) && (Models.UserModel.List[state.session.user_id].SelectingFilm || Models.UserModel.List[state.session.user_id].SelectingAge))
            {
                // Если меню выбора фильма
                if (Models.UserModel.List[state.session.user_id].SelectingFilm)
                {
                    // Get film name
                    string FilmName = Request.ParseName(Models.UserModel.List[state.session.user_id].SelectingFilmName);
                    Models.UserModel.List[state.session.user_id].SelectingFilm = false;
                    // Request for film info
                    Request.GET("https://discovery-stb3.ertelecom.ru/api/v3/pages/search?text=" + FilmName + "&limit=5",
                    // Success callback
                    (Good) =>
                    {
                        Console.WriteLine("\nЧо да как то\n");
                        AnswerModel.response.tts = AnswerModel.response.text = $"\"{FilmName}\"\n============\nИНФО";                       
                        SendAnswer(AnswerModel); // Send answer
                    },
                    (Bad) =>
                    {

                    });
                }

                else
                {

                }
            }

            // Если пользователь в меню выбора и выберет отрицательный ответ
            else if (Dictionary.Uncorrect.Contains(state.request.nlu.tokens[0].ToLower()) && (Models.UserModel.List[state.session.user_id].SelectingFilm || Models.UserModel.List[state.session.user_id].SelectingAge))
            {
                // Если меню выбора фильма
                if (Models.UserModel.List[state.session.user_id].SelectingFilm)
                {
                    // Get film name
                    string FilmName = Request.ParseName(Models.UserModel.List[state.session.user_id].SelectingFilmName);
                    Models.UserModel.List[state.session.user_id].SelectingFilm = false;
                    SendAnswer(AnswerModel); // Send answer
                }
                else
                {
                    
                }
            }

            // Если неизвестно, что выбирает юзер
            else
            {
                // Get film name
                string FilmName = Request.ParseName(state.request.command);
                Models.UserModel.List[state.session.user_id].SelectingFilmName = FilmName;
                Models.UserModel.List[state.session.user_id].SelectingFilm = true;
                AnswerModel.response.tts = AnswerModel.response.text = $"Вы выбрали фильм \"{FilmName}\"?";
                AnswerModel.response.buttons = new[]
                        {
                            new Models.PostModel.Response.Buttons
                            {
                                title = "Верно",
                                hide = true
                            },
                            new Models.PostModel.Response.Buttons
                            {
                                title = "Нет",
                                hide = true
                            }
                        };
                SendAnswer(AnswerModel); // Send answer
            }
                        
        }

        public static void SendAnswer(Models.PostModel AnswerModel)
        {
            // Send answer
            Request.POST(Hidden.Gate + Hidden.Methods.Send,
            // Convert to HttpContent
            Request.ToHttpContent(AnswerModel),
            // Success callback
            (suc) =>
            {

            },
            // Error callback
            (err) =>
            {
                Console.WriteLine(err);
            });
        }
    }
}
