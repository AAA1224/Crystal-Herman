using System.Collections;
using System.Xml;
using System.IO;
using UnityEngine;

namespace KoboldTools
{
    [RequireComponent(typeof(ILocalisation))]
    public class LocalisationReaderXML : MonoBehaviour
    {
        public string relativeFolderPath = "lang/";
        private ILocalisation loca;
        private XmlNodeList supportedLanguages;

        public IEnumerator Start()
        {
            loca = transform.GetComponent<ILocalisation>();
            string path = relativeFolderPath;
            path = path.Replace("\\", "/");

            Debug.Log("Searching for localised text data in '{0}'" + path);

            //fetch all language files
            yield return StartCoroutine(loadText(
                Path.Combine(path, "languages.xml").Replace("\\","/"),
                (xml) =>
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);
                    supportedLanguages = xmlDoc.GetElementsByTagName("language");
                }
            ));

            foreach (XmlNode node in supportedLanguages)
            {
                string filePath = Path.Combine(path, node.Attributes["uid"].Value + ".xml").Replace("\\", "/");
                yield return StartCoroutine(loadText(
                    filePath,
                    (xml) => { fetchLocalisedDataXML(xml, loca); }
                ));
            }

            /*Debug.Log("[Localisation] Search for localised text data in '" + path + "'");
            string[] xmlfiles = Directory.GetFiles(path, "*.xml");
            if (xmlfiles.Length == 0)
            {
                Debug.LogError("[Localisation] Could not read language files. There are no xml language files in '" + path + "'.");
            }

            //parse each language files
            foreach (string file in xmlfiles)
            {
                string filePath = file.Replace("\\", "/");
                Debug.Log("[Localisation] Reading XML File '" + filePath + "'");
                yield return StartCoroutine(loadText(
                    filePath,
                    (xml) => { fetchLocalisedDataXML(xml, loca); }
                ));
            }*/

            //invoke language changed event
            loca.eLanguageChanged.Invoke();
        }

        /// <summary>
        /// parse an xml file for localisation data
        /// </summary>
        /// <param name="xmlData">xml data</param>
        /// <param name="loca">localisation object for adding the read data</param>
        /// <returns></returns>
        private void fetchLocalisedDataXML(string xmlData, ILocalisation loca)
        {
            string language = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            //get language of the file
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("language");
            language = nodeList[0].Attributes["ISO6393"].Value;
            loca.addLanguage(new GameLanguage(language, nodeList[0].InnerText));

            //get textlines
            nodeList = xmlDoc.DocumentElement.SelectNodes("/pm/textlines/textline");
            foreach (XmlNode node in nodeList)
            {
                string textline = "";
                if (node.ChildNodes.Count > 1)
                {
                    for (int i = 0; i<node.ChildNodes.Count; i++)
                    {
                        textline += node.ChildNodes[i].InnerText;
                        if (i<node.ChildNodes.Count-1)
                        {
                            textline += "\n";
                        }
                    }
                }
                else
                {
                    textline = node.InnerText;
                }
                loca.addLocalisedText(node.Attributes["uid"].Value, language, textline);
                //Debug.Log("[Localisation] textline: " + node.Attributes["uid"].Value + " -> " + textline);
            }
        }

        /// <summary>
        /// loads a file from the streaming asset path
        /// </summary>
        /// <param name="path">the path</param>
        /// <param name="callback">callback method with file content</param>
        /// <returns></returns>
        private IEnumerator loadText(string path, System.Action<string> callback)
        {
            string url = Path.Combine(Application.streamingAssetsPath, path);
            Debug.Log("Reading XML file at '{0}'" + url);

            if (url.Contains("://"))
            {
                WWW www = new WWW(url);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    callback(www.text);
                }
                else
                {
                    Debug.LogWarning(www.error);
                }
            }
            else
            {
                callback(File.ReadAllText(url));
            }
        }
    }
}
