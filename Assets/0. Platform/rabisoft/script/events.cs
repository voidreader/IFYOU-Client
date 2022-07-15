using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class events : MonoBehaviour
{
    private GameObject sliderBar;
    private GameObject btns;
    public GameObject chim;
    public GameObject chim2;

    void Awake()
    {
        GameObject sliderBar = GameObject.Find("bottomSlider") as GameObject;
        GameObject btns = GameObject.Find("btns") as GameObject;
    }

    void Start()
    {
        sliderBar = GameObject.Find("bottomSlider");
        btns = GameObject.Find("btns");

        loaddingScreen(); //�ε� ��ũ�� �ð� ���ư��� ȿ��
    }

    public void lolisRot(bool isOn)
    {
        if (isOn)
        {
            sliderBar.transform.DOMoveY(0, 0.5f).SetEase(Ease.OutQuad);
            btns.transform.DORotate(new Vector3(0, 0, -180), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        }
        else
        {
            sliderBar.transform.DOLocalMoveY(-1085, 0.5f).SetEase(Ease.OutQuad);
            btns.transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        }
    }

    public void visualOn()
    {
        btns.GetComponent<Toggle>().isOn = true;
    }

    public void visualOff()
    {
        btns.GetComponent<Toggle>().isOn = false;
    }

    public void loaddingScreen()
    {
        chim.transform.DORotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        chim2.transform.DORotate(new Vector3(0, 0, -360), 10.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }
}