using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockLauncher.Localization.Language
{
    public static class AvailableLanguages
    {
        public static List<string> GetAll()
        {
            return new List<string>()
            {
                "en-US",
                "en-UD",
                "es-ES",
                "fr-FR",
                "fi-FI",
                "ms-MY",
                "pl-PL",
                "pt-BR",
                "pt-PT",
                "ru-RU",
                "tr-TR",
                "uk-UA",
                "zh-CN",
                "zh-TW"
            };
        }
    }
}
