using System.Collections.Generic;
using System.IO;
using System.Xml;

using UnityEngine;

public class LanguageManager {

    //public static LanguageManager _instance;
    //private static string language = "Spanish";
    //private static string file = "Resources/language.xml";

    private Dictionary<string, string> Strings;

    /*public void OnEnable() {
        if (_instance == null) {
            _instance = this;// 
            new LanguageManager(Path.Combine(Application.dataPath, file), language);
        }
    }*/

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="path"> ruta al fichero xml </param>
    /// <param name="language"> idioma a usar </param>
    public LanguageManager(string path, string language) {
        setLanguage(path, language);
    }

    /// <summary>
    /// Metodo que permite cambiar entre varios idiomas
    /// </summary>
    /// <param name="path"> ruta al fichero xml </param>
    /// <param name="language"> idioma a usar </param>
    public void setLanguage(string path, string language) {
        var xml = new XmlDocument();
        xml.Load(path);

        Strings = new Dictionary<string, string>();
        var element = xml.DocumentElement[language];
        if (element != null) {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext()) {
                var xmlItem = (XmlElement)elemEnum.Current;
                Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
            }
        }
        else {
            Debug.LogError("El idioma especificado no existe: " + language);
        }
    }

    /// <summary>
    /// Devuelve la cadena correspondiente al idioma seleccionado
    /// </summary>
    /// <param name="name"> identificador de la cadena </param>
    /// <returns></returns>
    public string getString(string name) {
        if (!Strings.ContainsKey(name)) {
            Debug.LogError("La cadena especificada no existe: " + name);

            return "";
        }

        return (string)Strings[name];
    }
}