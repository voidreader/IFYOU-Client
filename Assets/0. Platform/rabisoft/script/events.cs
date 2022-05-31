using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class events : MonoBehaviour
{
    private GameObject sliderBar;
    private GameObject btns;

    void Awake()
    {
        GameObject sliderBar = GameObject.Find("bottomSlider") as GameObject;
        GameObject btns = GameObject.Find("btns") as GameObject;
    }

    void Start()
    {
        sliderBar = GameObject.Find("bottomSlider");
        btns = GameObject.Find("btns");
    }

    public void Update()
    {
    }

    public void lolisRot(bool isOn)
    {
        if (isOn)
        {
            sliderBar.transform.DOLocalMoveY(-1085, 0.5f).SetEase(Ease.OutQuad);
            btns.transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        }
        else
        {
            sliderBar.transform.DOMoveY(0, 0.5f).SetEase(Ease.OutQuad);
            btns.transform.DORotate(new Vector3(0, 0, -180), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        }
    }
}