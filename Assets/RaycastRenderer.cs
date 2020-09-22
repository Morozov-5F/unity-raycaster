using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class RaycastRenderer : MonoBehaviour
{
    public Image imageToRender;
    public int downscaleFactor;
    
    
    private Texture2D _renderTexture;
    private Camera _camera;

    private int _x, _y, _xdir, _ydir;
    private float _timeSinceLastMove;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _renderTexture = new Texture2D(_camera.pixelWidth / downscaleFactor, _camera.pixelHeight / downscaleFactor);
        _renderTexture.filterMode = FilterMode.Point;
        _renderTexture.alphaIsTransparency = false;

        for (int i = 0; i < _renderTexture.width; ++i)
        {
            for (int j = 0; j < _renderTexture.height; ++j)
            {
                _renderTexture.SetPixel(i, j, Color.white);
            }            
        }

        imageToRender.material.mainTexture = _renderTexture;
        _timeSinceLastMove = Time.time;

        _xdir = _ydir = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _timeSinceLastMove < 0.05f)
        {
            return;
        }
        
        _renderTexture.SetPixel(_x, _y, Color.white);

        _x = _x + _xdir;
        if (_x == _renderTexture.width - 1 || _x == 0)
        {
            _xdir = -_xdir;
            _y = _y + _ydir;
            if (_y == 0 || _y == _renderTexture.height - 1)
            {
                _ydir = -_ydir;
            }
        }

        _renderTexture.SetPixel(_x, _y, Color.black);
        _renderTexture.Apply();
        
        _timeSinceLastMove = Time.time;
    }
}
