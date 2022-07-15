using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingScript : MonoBehaviour
{
    #region appearance
    //color and appearance
    [Header("Appearance")]
    [SerializeField] Color[] _presetColor;
    [SerializeField] int[] materialIndex;

    [SerializeField] GameObject minimapIcon;

    [Header("Highlight")]
    Color highlightColor = Color.white;
    [SerializeField] Color flashingColor = Color.red;
    [SerializeField] int flashingCount = 5;

    Color[] _highlightColor;
    Color[] _flashingColor;
    Color[] _activeColor;

    protected int SkipNum;

    Color[] ChangeHue(Color[] _a, Color b)
    {
        Color[] a = new Color[_a.Length];
        for (int i = 0; i < _a.Length; i++)
        {
            Color.RGBToHSV(_a[i], out float _, out float cS, out float cV);
            Color.RGBToHSV(b, out float bH, out float bS, out float _);
            a[i] = Color.HSVToRGB(bH, bS, cV);
        }
        return a;
    }

    public void SetColor(Color[] _color)
    {
        //Debug.Log(_renderers.Length);
        if (_iconRenderer!=null && isActive)
        {
            _iconRenderer.color = _color[0];
        }
        else
        {
            _iconRenderer.color = Color.white;
        }
        
        if (_renderers.Length == 0) return;

        for (int i = 0; i < materialIndex.Length; i++)
        {
            _renderers[0].GetPropertyBlock(_propBlock, materialIndex[i]);
            _propBlock.SetColor("_Color", _color[i]);

            
            foreach (Renderer _renderer in _renderers)
            {
                _renderer.SetPropertyBlock(_propBlock, materialIndex[i]);
            }
        }
    }

    /// <summary>
    /// depreciated
    /// </summary>
    public void ToggleInteractHighlight()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        _activeColor = isActive ? _highlightColor : _presetColor;

        SetColor(_activeColor);
    }

    void LogColor(Color[] c)
    {
        foreach (Color _c in c)
        {
            Debug.Log(_c.ToString());
        }
    }

    #endregion

    protected void Awake()
    {
        _highlightColor = ChangeHue(_presetColor, highlightColor);
        _flashingColor = ChangeHue(_presetColor, flashingColor);
    }


    // Start is called before the first frame update
    protected void Start()
    {
        _propBlock = new MaterialPropertyBlock();

        Renderer[] allRenderers = transform.GetComponentsInChildren<Renderer>();
        _renderers = new Renderer[allRenderers.Length - SkipNum];

        Array.Copy(allRenderers, 0, _renderers, 0, allRenderers.Length - SkipNum);

        if (minimapIcon!=null)
        {
            _iconRenderer = minimapIcon.transform.GetComponent<SpriteRenderer>();
        }
        
        ChangeColor();
    }


    Renderer[] _renderers;
    SpriteRenderer _iconRenderer;
    MaterialPropertyBlock _propBlock;


    [HideInInspector] public int orderType;

    [HideInInspector] public bool isActive = false;

    //called everytime order is set
    protected void StartOrder(int timer, Color colorPair)
    {
        _highlightColor = ChangeHue(_presetColor, colorPair);
        isActive = true;
        ChangeColor();
        StartCoroutine(ApplicationModel.GM.GameInvoke(FlashColor, timer - flashingCount));
    }

    //called everytime order is stopped/completed
    protected virtual void ResetState()
    {
        CancelInvoke();
        StopAllCoroutines();
        isActive = false;
        ChangeColor();
    }

    void FlashColor()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        float progress;
        Color[] t_color = new Color[_activeColor.Length];

        //flashing
        for (int i = 0; i < flashingCount; i++)
        {
            progress = 0f;
            //fadeToColor
            while (progress < 0.5f)
            {
                while (OrderManager.isPaused)
                {
                    yield return new WaitForEndOfFrame();
                }
                progress += Time.deltaTime;
                for (int j = 0; j < t_color.Length; j++)
                {
                    t_color[j] = Color.Lerp(_activeColor[j], _flashingColor[j], progress);
                }
                SetColor(t_color);
            }
            //revertFromColor
            while (progress > 0.001f)
            {
                while (OrderManager.isPaused)
                {
                    yield return new WaitForEndOfFrame();
                }
                progress -= Time.deltaTime;
                for (int j = 0; j < t_color.Length; j++)
                {
                    t_color[j] = Color.Lerp(_activeColor[j], _flashingColor[j], progress);
                }
                SetColor(t_color);
                yield return new WaitForEndOfFrame();
            }
        }

        ResetState();
    }

}
