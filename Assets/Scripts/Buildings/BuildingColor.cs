using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColor : MonoBehaviour
{
    public Color[] presetColor;
    [SerializeField] int[] materialIndex = { 0 };

    private Renderer [] _renderers;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _renderers = transform.GetComponentsInChildren<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        setColor();
    }

    public void setColor()
    {
        for (int i = 0; i < materialIndex.Length; i++)
        {
            _renderers[0].GetPropertyBlock(_propBlock, materialIndex[i]);
            _propBlock.SetColor("_Color", presetColor[i]);

            foreach (Renderer _renderer in _renderers)
            {
                _renderer.SetPropertyBlock(_propBlock, materialIndex[i]);
            }
        }
    }

    public void flashColor(Color flashcolor)
    {
        for (int i = 0; i < materialIndex.Length; i++)
        {
            _renderers[0].GetPropertyBlock(_propBlock, materialIndex[i]);
            _propBlock.SetColor("_Color", flashcolor);

            foreach (Renderer _renderer in _renderers)
            {
                _renderer.SetPropertyBlock(_propBlock, materialIndex[i]);
            }
        }
    }
}
