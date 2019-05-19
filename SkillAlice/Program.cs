using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace SkillAlice
{
    class Program
    {
        static void Main(string[] args)
        {          

            // Initialize
            App.Init();
            Task.Factory.StartNew(() => 
            {
                while (true)
                {
                    try {
                        FileStream fs = new FileStream($"{Hidden.Path}/UserInfo.ini", FileMode.Create); // Создаем поток
                        BinaryFormatter bf = new BinaryFormatter(); // Включаем бинарное форматирование
                        bf.Serialize(fs, Models.UserModel.List); // Сериализуем
                        fs.Close();
                        Thread.Sleep(5000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.StackTrace}");
                    }
                }               
            });

            // Recursive update
            while (true)
            {     
                App.CheckForUpdates();
                Thread.Sleep(500);
            }

            
        }


        
        

    }
}
