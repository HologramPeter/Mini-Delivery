using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHandler : MonoBehaviour
{
    [SerializeField] Material targetMaterial;
    [SerializeField] GameObject bike;
    [SerializeField] GameObject paperBag;
    [SerializeField] GameObject pill;
    [SerializeField] GameObject spray;
    [SerializeField] GameObject mask;
    [SerializeField] GameObject newMask;

    [SerializeField] float BikeVerticalOffset;

    public void SetColor(Color color)
    {
        targetMaterial.SetColor("_Color", color);
    }

    public void SetBikeActive(bool active)
    {
        if (bike.activeInHierarchy == active) return;
        transform.position = transform.position + new Vector3(0, (active?1:-1)*BikeVerticalOffset, 0);
        bike.SetActive(active);
    }

    public void SetBagActive(bool active)
    {
        paperBag.SetActive(active);
    }

    public void SetPillActive(bool active)
    {
        pill.SetActive(active);
    }

    public void SetSprayActive(bool active)
    {
        spray.SetActive(active);
    }

    public void SetMaskActive(bool active)
    {
        Debug.Log("mask" + active.ToString());
        mask.GetComponent<Renderer>().enabled = !active;
        newMask.SetActive(active);
    }

    public void SetTexture(Texture? texture)
    {
        targetMaterial.mainTexture = texture;
    }
    //img.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
}
