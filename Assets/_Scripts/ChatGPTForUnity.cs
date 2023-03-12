using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;

public class ChatGPTForUnity : MonoBehaviour
{
    [SerializeField] private string APIKey;
    [TextArea(3, 10)]
    [SerializeField] private string prompt;
    [TextArea(3, 40)]
    [SerializeField] private string result;

    private readonly string chatGPTUrlAPI = "https://api.openai.com/v1/completions";
    private readonly string scriptsFolder = "Assets/_Scripts";
    private readonly string directory = "ChatGPT";

    RequestBodyChatGPT requestBodyChatGPT;
    ResponseBodyChatGPT responseBodyChatGPT;

    public void SendRequest()
    {
        result = string.Empty;

        requestBodyChatGPT = new RequestBodyChatGPT();
        requestBodyChatGPT.model = "text-davinci-003";
        requestBodyChatGPT.prompt = prompt;
        requestBodyChatGPT.max_tokens = 2048;
        requestBodyChatGPT.temperature = 0;

        StartCoroutine(SendRequestAPI());
    }

    private IEnumerator SendRequestAPI()
    {
        string jsonData = JsonUtility.ToJson(requestBodyChatGPT);
        byte[] rawData = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest requestChatGPT = new UnityWebRequest(chatGPTUrlAPI, "POST");

        requestChatGPT.uploadHandler = new UploadHandlerRaw(rawData);
        requestChatGPT.downloadHandler = new DownloadHandlerBuffer();

        //-H 'Content-Type: application/json' \
        //-H 'Authorization: Bearer YOUR-API_KEY' \

        requestChatGPT.SetRequestHeader("Content-Type", "application/json");
        requestChatGPT.SetRequestHeader("Authorization", "Bearer " + APIKey);

        result = "Loading...";

        yield return requestChatGPT.SendWebRequest();

        if (requestChatGPT.result == UnityWebRequest.Result.Success)
        {
            responseBodyChatGPT = JsonUtility.FromJson<ResponseBodyChatGPT>(requestChatGPT.downloadHandler.text);
            result = responseBodyChatGPT.choices[0].text;
        }
        else
        {
            result = "Error: " + requestChatGPT.result;
        }

        requestChatGPT.Dispose();
    }

    public void Clear()
    {
        prompt = String.Empty;
        result = String.Empty;
    }

    public void SaveScript()
    {
        if (!Directory.Exists(scriptsFolder + "/" + directory))
        {
            Directory.CreateDirectory(scriptsFolder + "/" + directory);
        }

        string className = ParseClassName(result);
        string scriptPath = scriptsFolder + "/" + directory + "/" + className + ".cs";
        using FileStream fs = new FileStream(scriptPath, FileMode.Create);
        using StreamWriter writer = new StreamWriter(fs);

        writer.Write(result);
    }

    public string ParseClassName(string result)
    {
        int indexClass = result.IndexOf("class");
        int indexDots = result.IndexOf(":");
        string className = result.Substring(indexClass + 6, indexDots - indexClass - 6 - 1);
        return className;
    }

}
[Serializable]
public class RequestBodyChatGPT
{
    public string model;
    public string prompt;
    public int max_tokens;
    public int temperature;
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);


[Serializable]
public class ResponseBodyChatGPT
{
    public string id ;
    public string @object ;
    public int created ;
    public string model ;
    public List<Choice> choices ;
    public Usage usage ;

    [Serializable]
    public class Choice
    {
        public string text ;
        public int index ;
        public object logprobs ;
        public string finish_reason ;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens ;
        public int completion_tokens ;
        public int total_tokens ;
    }
}





