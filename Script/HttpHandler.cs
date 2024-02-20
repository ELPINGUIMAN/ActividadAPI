using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HttpHandler : MonoBehaviour
{
    public RawImage[] images;
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI[] cardNameTexts;
    public Button nextUserButton;
    public Button prevUserButton;

    private string fakeApiUrl = "https://my-json-server.typicode.com/ELPINGUIMAN/ActividadAPI";
    private string rickyMortyApiurl = "https://rickandmortyapi.com/api";

    private int currentUserId = 1;

    private void Start()
    {
        nextUserButton.onClick.AddListener(NextUser);
        prevUserButton.onClick.AddListener(PrevUser);
    }

    public void SendRequest()
    {
        StartCoroutine(GetUserData(currentUserId));
    }

    IEnumerator GetUserData(int uid)
    {
        UnityWebRequest request = UnityWebRequest.Get(fakeApiUrl + "/users/" + uid);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                UserData user = JsonUtility.FromJson<UserData>(request.downloadHandler.text);

                userNameText.text = "Usuario: " + user.username;

                int maxImages = Mathf.Min(user.deck.Length, images.Length);

                for (int i = 0; i < maxImages; i++)
                {
                    StartCoroutine(GetCharacter(user.deck[i], i));
                }
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator GetCharacter(int id, int imageIndex)
    {
        UnityWebRequest request = UnityWebRequest.Get(rickyMortyApiurl + "/character/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                CharacterData character = JsonUtility.FromJson<CharacterData>(request.downloadHandler.text);

                cardNameTexts[imageIndex].text = character.name;

                StartCoroutine(DownloadImage(character.image, imageIndex));
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator DownloadImage(string url, int imageIndex)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            images[imageIndex].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    
    private void NextUser()
    {
        if (currentUserId <= 3)
        {
            currentUserId++;
            SendRequest();
        }
    }

    
    private void PrevUser()
    {
        if (currentUserId > 1)
        {
            currentUserId--;
            SendRequest();
        }
    }

    [System.Serializable]
    public class JsonData
    {
        public InfoData info;
        public CharacterData[] results;
        public UserData[] users;
    }

    [System.Serializable]
    public class UserData
    {
        public int id;
        public string username;
        public int[] deck;
    }
    [System.Serializable]
    public class CharacterData
    {
        public int id;
        public string name;
        public string species;
        public string image;
    }
    [System.Serializable]
    public class InfoData
    {
        public int count;
        public int pages;
        public string next;
        public string prev;
    }
}