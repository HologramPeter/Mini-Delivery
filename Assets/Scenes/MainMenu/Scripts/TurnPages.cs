using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPages : MonoBehaviour
{
    [SerializeField] GameObject[] childs;

    int activeIndex;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject child in childs)
        {
            child.SetActive(false);
        }

        childs[0].SetActive(true);
        activeIndex = 0;
    }

    public void NextPage()
    {
        SetChildActive((activeIndex + 1) % childs.Length);
    }

    public void PreviousPage()
    {
        SetChildActive((activeIndex - 1 + childs.Length) % childs.Length);
    }

    void SetChildActive(int index)
    {
        childs[activeIndex].SetActive(false);
        childs[index].SetActive(true);
        activeIndex = index;
    }
}
