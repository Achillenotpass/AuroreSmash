using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TextBlink : MonoBehaviour
{
    Text _lbl;
    public float BlinkFadeInTime = 0.5f;
    public float BlinkStayTime = 0.8f;
    public float BlinkFadeOutTime = 0.7f;
    private float _timeChecker = 0;
    private Color _color;
    void Start()
    {
        _lbl = GetComponent<Text>();
        _color = _lbl.color;
    }


    void Update()
    {
        _timeChecker += Time.deltaTime;
        if (_timeChecker < BlinkFadeInTime)
        {
            _lbl.color = new Color(_color.r, _color.g, _color.b, _timeChecker / BlinkFadeInTime);
        } else if (_timeChecker < BlinkFadeInTime + BlinkStayTime)

        {
            _lbl.color = new Color(_color.r, _color.g, _color.b, _timeChecker / BlinkFadeInTime);
        }
            
        else if (_timeChecker < BlinkFadeInTime + BlinkStayTime + BlinkFadeOutTime)
        {
            _lbl.color = new Color(_color.r, _color.g, _color.b, 1 - (_timeChecker - (BlinkFadeInTime + BlinkStayTime))/BlinkFadeOutTime);
        }
        else
        {
            _timeChecker = 0;
        }
        }
    }


