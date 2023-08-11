using Firebase;
using Firebase.RemoteConfig;
using Firebase.Storage;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class VerifyAssets : MonoBehaviour
{
    public GameObject LoadTextInfo;
    public GameObject LoadBar;
    
    private const string STORAGE_URL = "tu_url_aqui";
    private string remoteVersion = "";
}
