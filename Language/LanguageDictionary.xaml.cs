using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BedrockLauncher.Localization.Language
{
    public partial class LanguageDictionary : ResourceDictionary
    {
        public string Language { get; set; }

        public LanguageDictionary(LanguageDefinition language)
        {         
            this.Language = language.Locale;
            this.Source = new Uri(language.Path, (language.IsExternal ? UriKind.Absolute : UriKind.Relative));
        }

        public LanguageDictionary()
        {
            var language = LanguageDefinition.Default;

            this.Language = language.Locale;
            this.Source = new Uri(language.Path, (language.IsExternal ? UriKind.Absolute : UriKind.Relative));
        }
    }
}
