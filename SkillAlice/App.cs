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
        public async static void Init()
        {
            await Request.PostImage("http://er-cdn.ertelecom.ru/content/public/r1617238", (data) => Console.WriteLine(data), (code) => Console.WriteLine(code));

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
                    //try
                    //{
                    // Make Model
                    Models.GetModel state = JsonConvert.DeserializeObject<Models.GetModel>(data);
                    // Check if this case was not already handled
                    if (state != null && Hidden.LastUnix != state.unix)
                    {
                        // Debug log
                        //Console.WriteLine("New request: " + state.request.original_utterance + data);
                        // Handle this case
                        Handle(state);
                    }
                    //}
                    //catch (System.NullReferenceException) {  }
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
            if (Models.UserModel.List != null && !Models.UserModel.List.ContainsKey(state.session.user_id))
                Models.UserModel.List.Add(state.session.user_id, new Models.UserModel());

            // Check if base clean
            if (state.request.nlu.tokens == null || state.request.nlu.tokens.Length == 0 )
            {
                Console.WriteLine("empty");
                state.request.nlu.tokens = new string[] { "меню" };
                state.request.command = "меню";
            }

            // Check if recoms
            if (Dictionary.Suggest.Contains(state.request.command.ToLower()))
            {
                state.request.command = "тест";
            }

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
                Console.WriteLine("Choicing");
                // Если меню выбора фильма
                if (Models.UserModel.List[state.session.user_id].SelectingFilm)
                {
                    Console.WriteLine("Selecting");
                    // Get film name
                    string FilmName = Models.UserModel.List[state.session.user_id].SelectingFilmName;
                    Models.UserModel.List[state.session.user_id].SelectingFilm = false;
                    // Request for film info
                    if (FilmName != "&")
                    {
                        Console.WriteLine("Бла");
                        Request.GET("https://discovery-stb3.ertelecom.ru/api/v3/pages/search?text=" + FilmName + "&limit=5",
                        // Success callback
                        (data) =>
                        {
                            var filminfo = JsonConvert.DeserializeObject<Models.FilmModel>(data);
                            Console.WriteLine(filminfo.data.showcases[0].items[0].description);

                            string link = "";
                            foreach (Models.FilmModel.Data.Showcases.Items.Resources r in filminfo.data.showcases[0].items[0].resources)
                            {
                                if (r.type == "hls")
                                {
                                    string urlurl = "https://discovery-stb3.ertelecom.ru/resource/get_url/" + filminfo.data.showcases[0].items[0].id + "/" + r.id;
                                    Console.WriteLine(urlurl);
                                    Request.GET(urlurl, (d) => { link = d; Console.WriteLine(d); }, (e) => { Console.WriteLine(e); });
                                }
                            }
                                
                                    
                            Console.WriteLine(link);

                            AnswerModel.response.tts = AnswerModel.response.text = $"\"{FilmName}\"\n\n{Models.UserModel.Prerender[FilmName].data.showcases[0].items[0].description}";
                            AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[5];                          
                            AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Смотреть", hide = true, url = "http://lawfilter.ertelecom.ru/" };
                            AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "В ролях", hide = true };
                            AnswerModel.response.buttons[2] = new Models.PostModel.Response.Buttons { title = "Подробнее", hide = true };
                            AnswerModel.response.buttons[3] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                            AnswerModel.response.buttons[4] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                            SendAnswer(AnswerModel); // Send answer
                        },
                        (code) =>
                        {
                            AnswerModel.response.tts = AnswerModel.response.text = $"Извините, но такого фильма нет в базе Movix";
                            AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[2];
                            AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                            AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                            SendAnswer(AnswerModel); // Send answer
                        });
                    }
                    else
                    {
                        AnswerModel.response.tts = AnswerModel.response.text = $"К сожалению я не нашла ничего по вашему запросу";
                        AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[2];
                        AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                        AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                        SendAnswer(AnswerModel); // Send answer
                    }
                }
                else
                {
                    Models.UserModel.List[state.session.user_id].SelectingAge = false;
                    Models.UserModel.List[state.session.user_id].Adult = true;
                    AnswerModel.response.tts = AnswerModel.response.text = $"Спасибо, теперь вам доступен 18+ контент";
                    AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[1];
                    AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                    SendAnswer(AnswerModel); // Send answer
                }
            }

            // Если пользователь в меню выбора и выберет отрицательный ответ
            else if (Dictionary.Uncorrect.Contains(state.request.nlu.tokens[0].ToLower()) && (Models.UserModel.List[state.session.user_id].SelectingFilm || Models.UserModel.List[state.session.user_id].SelectingAge))
            {
                // Если меню выбора фильма
                if (Models.UserModel.List[state.session.user_id].SelectingFilm)
                {
                    // Get film name
                    string FilmName = Models.UserModel.List[state.session.user_id].SelectingFilmName;
                    Models.UserModel.List[state.session.user_id].SelectingFilm = false;
                    AnswerModel.response.tts = AnswerModel.response.text = $"Вот фильмы, которые я нашла по запросу \"{FilmName}\":\n\n";
                    var filminfo = new Models.FilmModel();
                    if (Models.UserModel.Prerender.ContainsKey(FilmName))
                    {
                        filminfo = Models.UserModel.Prerender[FilmName];
                    }
                    else
                    {
                        filminfo = null;
                    }
                    // Create buttons array
                    try
                    {
                        AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[filminfo.data.showcases[0].items.Length + 1];
                        foreach (Models.FilmModel.Data.Showcases.Items item in filminfo.data.showcases[0].items)
                        {
                            if (item.title == "")
                            {
                                throw (new System.IndexOutOfRangeException());                               
                            }
                        }
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        AnswerModel.response.tts = AnswerModel.response.text = $"К сожалению, я не нашла больше фильмов по запросу \"{FilmName}\"";
                        AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[2];
                        AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                        AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                        SendAnswer(AnswerModel); // Send answer
                        return;
                    }
                    // Making cycle
                    int iterator = 1;
                    foreach (Models.FilmModel.Data.Showcases.Items item in filminfo.data.showcases[0].items)
                    {
                        // Adding buttons and text
                        if(item.title != "")
                        {
                            AnswerModel.response.buttons[iterator - 1] = new Models.PostModel.Response.Buttons { title = item.title, hide = true };
                            AnswerModel.response.text += $"{iterator++}. {item.title}\n";
                        }
                    }
                    AnswerModel.response.buttons[iterator - 1] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                    SendAnswer(AnswerModel); // Send answer
                }
                else
                {
                    Models.UserModel.List[state.session.user_id].SelectingAge = false;
                    Models.UserModel.List[state.session.user_id].Adult = false;
                    AnswerModel.response.tts = AnswerModel.response.text = $"Вы вошли в безопасный режим";
                    AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[1];
                    AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                    SendAnswer(AnswerModel); // Send answer
                }
            }

            // Если пользователь не указал возраст
            else if (Models.UserModel.List[state.session.user_id].SelectingAge || state.request.command.ToLower() == "возрастная категория")
            {
                AnswerModel.response.text = AnswerModel.response.tts = $"Добро пожаловать, для начала работы с сервисом необходимо указать вашу возрастную категорию. \n\nВы совершеннолетний?";
                Models.UserModel.List[state.session.user_id].SelectingAge = true; Models.UserModel.List[state.session.user_id].SelectingFilm = false;
                AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[2];
                AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Да", hide = true };
                AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "Нет", hide = true };
                SendAnswer(AnswerModel); // Send answer
            }

            // Если пользователь перешел в меню
            else if (Dictionary.BackAlias.Contains(state.request.command.ToLower().Trim(' ')))
            {
                Models.UserModel.List[state.session.user_id].SelectingAge = false; Models.UserModel.List[state.session.user_id].SelectingFilm = false;
                AnswerModel.response.text = AnswerModel.response.tts = $"Вы можете попроить меня:" +
                                                                     $"\n\"Включить [Название фильма]\"" +
                                                                     $"\nПоказать \"Рекомендации\"" +
                                                                     $"\nРассказать, \"Кто играл в [Название фильма]\"";
                SendAnswer(AnswerModel); // Send answer
            }

            // Роли
            else if(state.request.command.ToLower().Split(' ')[0].Trim(' ') == "роли" || state.request.command.ToLower().Split(' ')[0].Trim(' ') == "актеры")
            {
                string FilmName = "https://www.google.ru/search?q=" + "актеры " + Request.ParseName(state.request.command.ToLower().Replace("роли ", "").Replace("актеры ", ""));
                AnswerModel.response.tts = AnswerModel.response.text = $"Список актеров для выбранного фильма:";
                AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[2];
                Console.WriteLine(FilmName);
                AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Просмотр", hide = true, url = FilmName.Replace(' ', '+') };
                AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                SendAnswer(AnswerModel); // Send answer
            }

            // Если неизвестно, что выбирает юзер
            else
            {
                Console.WriteLine("[Ask for film] " + state.request.command + "|" + state.request.command.ToLower()
                    .Replace("найди ", "").Replace("покажи ", "").Replace("посмотреть ", "").Replace("мне ", "").Replace("фильм ", "").Replace("хочу ", "").Replace("смотреть ", "").Replace("показать ", "").Replace("просмотр ", "").Trim(' '));
                // Get film name
                string FilmName = Request.ParseName(state.request.command).ToLower()
                    .Replace("найди ", "").Replace("покажи ", "").Replace("посмотреть ", "").Replace("мне ", "").Replace("фильм ", "").Replace("хочу ", "").Replace("смотреть ", "").Replace("показать ", "").Replace("просмотр ", "").Trim(' ');               
                if (FilmName != "&")
                {
                    Models.UserModel.List[state.session.user_id].SelectingFilmName = FilmName;
                    Models.UserModel.List[state.session.user_id].SelectingFilm = true;
                    AnswerModel.response.tts = AnswerModel.response.text = $"Вы хотите выбрать фильм \"{FilmName}\"?";
                    AnswerModel.response.buttons = new[]
                            {
                            new Models.PostModel.Response.Buttons
                            {
                                title = "Верно",
                                hide = true
                            },
                            new Models.PostModel.Response.Buttons
                            {
                                title = "Нет, посмотреть похожие",
                                hide = true
                            }
                        };
                    SendAnswer(AnswerModel); // Send answer
                    // Prerender this film
                    Request.GET("https://discovery-stb3.ertelecom.ru/api/v3/pages/search?text=" + FilmName + "&limit=5",
                        // Success callback
                        (data) =>
                        {
                            if (!Models.UserModel.Prerender.ContainsKey(FilmName))
                            {
                                Console.WriteLine("Rendering...");
                                var filminfo = JsonConvert.DeserializeObject<Models.FilmModel>(data);
                                Models.UserModel.Prerender[FilmName] = filminfo;
                                Console.WriteLine("[Rendered] " + FilmName);
                            }
                        }, (code) => { Console.WriteLine(code); });
                }
                else
                {
                    AnswerModel.response.tts = AnswerModel.response.text = $"К сожалению я не нашла ничего по вашему запросу";
                    AnswerModel.response.buttons = new Models.PostModel.Response.Buttons[2];
                    AnswerModel.response.buttons[0] = new Models.PostModel.Response.Buttons { title = "Рекомендации", hide = true };
                    AnswerModel.response.buttons[1] = new Models.PostModel.Response.Buttons { title = "Назад", hide = true };
                    SendAnswer(AnswerModel); // Send answer
                }
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
