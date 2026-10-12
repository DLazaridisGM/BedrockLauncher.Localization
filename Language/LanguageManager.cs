using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Collections;
using Path = System.IO.Path;
using System.Resources;
using System.Net;
using System.Windows.Resources;

namespace BedrockLauncher.Localization.Language
{
    public static class LanguageManager
    {

        private static List<LanguageDefinition> AvailableInternalLanguages 
        {
            get
            {

                var languages = new List<LanguageDefinition>();
                foreach (var lang in AvailableLanguages.GetAll())
                {
                    try
                    {
                        var def = new LanguageDefinition(lang);
                        languages.Add(def);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
                return languages;
            }
        }

        private static void LanguageChange(LanguageDefinition definition)
        {
            var possible = Application.Current.Resources.MergedDictionaries.Where(x => x is LanguageDictionary).FirstOrDefault();
            Application.Current.Resources.MergedDictionaries.Remove(possible);

            LanguageDictionary dict = new LanguageDictionary(definition);
            LanguageDictionary default_dic = new LanguageDictionary(AvailableInternalLanguages.FirstOrDefault());
            Application.Current.Resources.MergedDictionaries.Add(default_dic);
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public static bool TryGetResource(string name, out string contents)
        {
            try
            {
                var currentLang = GetResourceDictionaries().Where(x => x.Locale == Localization.Properties.Settings.Default.Language).FirstOrDefault();
                var currentLocale = currentLang.Locale.Replace("-", "_");

                contents = GetResource(name, currentLang);
                return true;

            }
            catch
            {
                try
                {
                    contents = GetResource(name, LanguageDefinition.Default);
                    return true;
                }
                catch
                {
                    contents = string.Empty;
                    return false;
                }
            }

            string GetResource(string _name, LanguageDefinition currentLang)
            {
                var currentLocale = currentLang.Locale.Replace("-", "_");
                string ext = "_ext";

                if (!currentLang.IsExternal)
                {
                    string resourceName = $"{ext}.{currentLocale}.{_name}";
                    var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                    if (names.ToList().Exists(x => x.EndsWith(resourceName)))
                    {
                        string resourcePath = names.ToList().FirstOrDefault(x => x.EndsWith(resourceName));
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                    else throw new Exception($"Can not find the following resource: \"{resourceName}\"");
                }
                else
                {
                    var directory = Path.Combine(Path.GetDirectoryName(currentLang.Path), currentLang.Locale);
                    return File.ReadAllText(Path.Combine(directory, _name));
                }
            }
        }

        public static object GetResource(string resourceName)
        {
            try
            {
                return Application.Current.TryFindResource(resourceName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something went wrong with the following resource: \"{resourceName}\" \r\n {ex}");
                return resourceName;
            }
        }


        #region Public Methods

        public static List<LanguageDefinition> GetResourceDictionaries()
        {
            string ExternalPath = System.IO.Path.Combine(AppContext.BaseDirectory, "data", "lang");
            List<LanguageDefinition> Languages = AvailableInternalLanguages;


            if (!Directory.Exists(ExternalPath)) return Languages;

            var ExternalLangFolder = new DirectoryInfo(ExternalPath);
            foreach (var File in ExternalLangFolder.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                if (File.Extension != ".xaml") continue;
                if (LanguageDefinition.TryParse(File, out LanguageDefinition ExternalLang)) Languages.Add(ExternalLang);
            }

            return Languages;
        }

        public static void SetLanguage(string language)
        {
            var AvailableLanguages = GetResourceDictionaries();

            if (language == "none" || language == "default")
            {
                CultureInfo ci = CultureInfo.InstalledUICulture;
                if (AvailableLanguages.Exists(x => x.Locale == ci.Name)) SetLocaleLanguage(ci.Name);
                else SetToDefaultLanguage();
            }
            else
            {
                if (AvailableLanguages.Exists(x => x.Locale == language)) SetLocaleLanguage(language);
                else SetToDefaultLanguage();
            }

            void SetToDefaultLanguage()
            {
                LanguageChange(LanguageDefinition.Default);
                Localization.Properties.Settings.Default.Language = "default";
                Localization.Properties.Settings.Default.Save();
            }

            void SetLocaleLanguage(string locale)
            {
                var Lang = AvailableLanguages.Where(x => x.Locale == locale).FirstOrDefault();
                LanguageChange(Lang);
                Localization.Properties.Settings.Default.Language = locale;
                Localization.Properties.Settings.Default.Save();
            }
        }
        public static void Init()
        {
            string language = Localization.Properties.Settings.Default.Language;
            SetLanguage(language);
        }

        #endregion
    }


}
