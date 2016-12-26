using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using UnityEngine;

public class LanguageManager {

    private Dictionary<string, string> Strings;

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
        TextAsset file = Resources.Load(path) as TextAsset;
        xml.LoadXml(file.text);

        Strings = new Dictionary<string, string>();
        XmlNode element = xml.DocumentElement[language];
        if (element != null) {
            IEnumerator elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext()) {
                // Se ignorar los comentarios
                if (elemEnum.Current.GetType() != typeof(XmlComment)) {
                    XmlElement xmlItem = (XmlElement)elemEnum.Current;
                    Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
                }
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