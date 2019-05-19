using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillAlice
{
    public class Dictionary
    {
        public static string[] Correct = new[] { "верно", "правильно", "ок", "согласен", "ага", "да", "+" };
        public static string[] Uncorrect = new[] { "нет", "нет,", "не верно", "неверно", "неа", "не" };
        public static string[] Filmlist = new[] { "покажи", "фильм", "посмотреть" };
        public static string[] CastAlias = new[] { "кто", "в ролях", "кто в ролях", "кто играет", "роли", "каст", "актеры" };
        public static string[] BackAlias = new[] { "назад", "помощь", "меню", "в меню", "как работать" };
        public static string[] InfoAlias = new[] { "расскажи", "подробнее", "описание", "инфо" };
        public static string[] Suggest = new[] { "что посмотреть", "что глянуть", "посмотреть", "рекомендации" };
    }
}
