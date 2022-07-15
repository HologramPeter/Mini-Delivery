using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine.SceneManagement;


public class ERC1155BalanceOfExample2 : MonoBehaviour
{
    //public GameObject Sphere;
    [SerializeField] public RawImage img1;
    //[SerializeField] public RawImage img2;
    [SerializeField] public TextMeshProUGUI balance;
    public UnityEngine.UI.Button btn1;
    public UnityEngine.UI.Button yesbtn;
    [SerializeField] public TextMeshProUGUI ConfirmedPurchase;

    async System.Threading.Tasks.Task<object> Start()
    {
        string chain = "ethereum";
        string network = "rinkeby";
        string contract = "0x88B48F654c30e99bc2e4A1559b4Dcf1aD93FA656";
        string tokenId1 = "30948256496342367792501964843211623706276934530243934524698709118650385694721";
        //string tokenId2 = "30948256496342367792501964843211623706276934530243934524698709117550874066945";


        //string playeraccount = PlayerPrefs.GetString("Account");
        //string playeraccount = "0x2f85438D3789bC6362A14be3DB86584b90e466db"; 
        string account = "0x446C17a97e2b7461FBce7a1B5D2293BF832A380C"; 
        BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, contract, account, tokenId1);
        Debug.Log("balanceOf"+ balanceOf);

        if (balanceOf > 0)
        {
            Debug.Log("balanceOf > 0");
            //Sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            balance.text = "Current balance: ETH " + balanceOf.ToString();

            yesbtn.onClick.AddListener(Confirmed);
            //ERC1155.safeTransferFrom(account, playeraccount, tokenId1, 0.1, playeraccount);

        }

        await GetImage(chain, network, contract, tokenId1, img1, btn1);

        return true;
    }

    public void Confirmed()
    {
        ConfirmedPurchase.text = "Oh yeah! You owned ChimChim now. ";
    }

    public class Response
    {
        public string image;
    }

    private async System.Threading.Tasks.Task<object> GetImage(string chain, string network, string contract, string tokenId, RawImage img, UnityEngine.UI.Button btn)
    {
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

        //Sphere.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
        img.texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;

        btn.gameObject.SetActive(true);

        return true; 
    }
    
}
