using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.IO;
using System.Net;

public class ERC1155BalanceOfExample : MonoBehaviour
{
    public GameObject Sphere;
    //[SerializeField] public RawImage img;

    async void Start()
    {
        string chain = "ethereum";
        string network = "rinkeby";
        string contract = "0x88B48F654c30e99bc2e4A1559b4Dcf1aD93FA656";
        //string account = PlayerPrefs.GetString("Account");
        string account = "0x446C17a97e2b7461FBce7a1B5D2293BF832A380C";

        //string tokenId = "0x446C17A97E2B7461FBCE7A1B5D2293BF832A380C000000000000010000000001";
        string tokenId = "30948256496342367792501964843211623706276934530243934524698709116451362439169";

        BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId);
        print(balanceOf);

        if (balanceOf > 0)
        {
            Debug.Log("balanceOf > 0");
            //Sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        }

        // fetch uri from chain
        string uri = await ERC1155.URI(chain, network, contract, tokenId);
        print("uri: " + uri);
        uri = uri.Replace("0x{id}", tokenId);

        // fetch json from uri
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        await webRequest.SendWebRequest();
        Response data = JsonUtility.FromJson<Response>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
        Debug.Log(data);

        // parse json to get image uri
        string imageUri = data.image;
        print("imageUri: " + imageUri);

        // fetch image and display in game
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUri);
        await textureRequest.SendWebRequest();

        Sphere.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
        //img.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;


    }
        public class Response
    {
        public string image;
    }

   
}
