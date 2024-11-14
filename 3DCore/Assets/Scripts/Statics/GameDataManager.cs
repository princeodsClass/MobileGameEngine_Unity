using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Globalization;

public class GameDataManager : SingletonMono<GameDataManager>
{
    bool _isSignin = false;

    public bool IsSignin()
    {
        return _isSignin;
    }

    private IEnumerator SendRequestByPost<T>(T control, string url, Dictionary<string, string> param, Action<string> result = null, Action<int, string> failCallback = null) where T : Enum
    {
        yield return SendRequest(UnityWebRequest.kHttpVerbPOST, control, url, param, result, failCallback);
    }

    private IEnumerator SendRequestByGet<T>(T control, string url, Dictionary<string, string> param, Action<string> result = null, Action<int, string> failCallback = null) where T : Enum
    {
        yield return SendRequest(UnityWebRequest.kHttpVerbGET, control, url, param, result, failCallback);
    }

    private IEnumerator SendRequestByPut<T>(T control, string url, Dictionary<string, string> param, Action<string> result = null, Action<int, string> failCallback = null) where T : Enum
    {
        yield return SendRequest(UnityWebRequest.kHttpVerbPUT, control, url, param, result, failCallback);
    }

    private IEnumerator SendRequestByDelete<T>(T control, string url, Dictionary<string, string> param, Action<string> result = null, Action<int, string> failCallback = null) where T : Enum
    {
        yield return SendRequest(UnityWebRequest.kHttpVerbDELETE, control, url, param, result, failCallback);
    }

    private IEnumerator SendRequest<T>(string httpMethod, T control, string url, Dictionary<string, string> param, Action<string> result, Action<int, string> failCallback) where T : Enum
    {
        int maxRetryCount = 3;
        int retryCount = 0;

        url = ComType.URL + url;

        WWWForm form = new WWWForm();
        form.AddField("control", control.ToString());

        foreach (KeyValuePair<string, string> field in param)
            form.AddField(field.Key, field.Value);

        do
        {
            UnityWebRequest www;
            if (httpMethod == UnityWebRequest.kHttpVerbPOST)
                www = UnityWebRequest.Post(url, form);
            else if (httpMethod == UnityWebRequest.kHttpVerbPUT)
                www = UnityWebRequest.Put(url, form.data);
            else
                www = new UnityWebRequest(url, httpMethod);

            www.timeout = 500;
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
            {
                if (www.downloadHandler.text == "under_maintenance")
                {
                    failCallback?.Invoke(503, "서버가 유지보수 중입니다.");
                    yield break;
                }

                result?.Invoke(www.downloadHandler.text);
                break;
            }
            else
            {
                failCallback?.Invoke((int)www.responseCode, www.error);
            }

            retryCount++;
            yield return new WaitForSeconds(2.0f);

        } while (retryCount < maxRetryCount);

        if (retryCount >= maxRetryCount)
            failCallback?.Invoke(0, "최대 재시도 횟수 초과");
    }

    private void RequestCallback(int responseCode, string errorText)
    {
        string errorMessage;

        switch (responseCode)
        {
            case 503:
                errorMessage = "The server is undergoing maintenance.\nPlease try again later.";
                break;
            case 404:
                errorMessage = "The URL could not be found.\nPlease check the server address.";
                break;
            case 401:
                errorMessage = "Authentication error.\nPlease make sure you used the correct access token.";
                break;
            default:
                errorMessage = $"Error Type: {responseCode}\nMessage: {errorText}";
                break;
        }

        PopupOneButton pop = MenuManager.Singleton.OpenPopup<PopupOneButton>(EUIPopup.PopupOneButton);
        pop.SetTitle(errorMessage);
        pop.SetButtonText("OK");
        pop.SetTitle("Infomation");
    }

    public IEnumerator CreateAccount(PopupWait4Response wait, PageMenu menu)
    {
        Dictionary<string, string> param = new Dictionary<string, string>
        {
            { "nickname", ComType.DEFAULT_NICKNAME },
            { "osType", ((int)ComUtil.CheckOS()).ToString() },
            { "deviceInfo", ComUtil.GetUniqueID() },
            { "country", CultureInfo.CurrentCulture.Name },
            { "deviceModel", SystemInfo.deviceModel }
        };

        yield return StartCoroutine(SendRequestByPost(EPostAPI.create_account, ComType.API_URL_CREATE_ACCOUNT, param,
            result =>
            {
                PopupOneButton pop = MenuManager.Singleton.OpenPopup<PopupOneButton>(EUIPopup.PopupOneButton);
                pop.SetMessage($"Complete Account creation ({result} )");
                pop.SetButtonText("OK");
                pop.SetTitle("Infomation");

                JObject json = JObject.Parse(result);

                PlayerPrefs.SetInt(ComType.STORAGE_UID, int.Parse(json.GetValue("auid").ToString()));
                menu.InitializePage();
            },
            (responseCode, errorText) => RequestCallback(responseCode, errorText)
        ));

        wait.Close();
    }

    public void DeleteAccount()
    {
        PlayerPrefs.DeleteAll();
        MenuManager.Singleton.SceneNext(ESceneType.MemuScene);
    }
}