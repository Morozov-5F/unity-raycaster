using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class RaycastRenderer : MonoBehaviour
{
    public Image imageToRender;
    public int targetResolution = 400;
    
    
    private Texture2D _renderTexture;
    private Camera _camera;

    private int _x, _y, _xdir, _ydir;
    private float _timeSinceLastMove;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        var h = Mathf.Tan(Mathf.PI * _camera.fieldOfView / 360);
        _viewportHeight = h * 2;
        _viewportWidth = _viewportHeight * _camera.aspect;
        
        var width = targetResolution > _camera.pixelWidth ? _camera.pixelWidth : targetResolution;
        _renderTexture = new Texture2D(width, (int) (width / _camera.aspect))
        {
            filterMode = FilterMode.Point, alphaIsTransparency = false
        };

        for (var i = 0; i < _renderTexture.width; ++i)
        {
            for (var j = 0; j < _renderTexture.height; ++j)
            {
                _renderTexture.SetPixel(i, j, Color.white);
            }            
        }

        imageToRender.material.mainTexture = _renderTexture;
        _timeSinceLastMove = Time.time;

        _xdir = _ydir = 1;
    }

    private RaycastHit[] _hits = new RaycastHit[1];
    
    private float _viewportHeight;
    private float _viewportWidth;
    
    // Update is called once per frame
    void Update()
    {
        var width = _renderTexture.width;
        var height = _renderTexture.height;
        
        var cameraTransform = _camera.transform;
        var cameraPosition = cameraTransform.position;
        var horizontal = cameraTransform.right * _viewportWidth;
        var vertical = cameraTransform.up * _viewportHeight;
        var lowerLeft = cameraPosition - 0.5f * horizontal - 0.5f * vertical + cameraTransform.forward; 
        
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                var u = (float) x / (width - 1);
                var v = (float) y / (height - 1);

                var rayDirection = lowerLeft + horizontal * u + vertical * v - cameraTransform.position;
                var ray = new Ray(cameraPosition, rayDirection);
                Color newColor;
                if (0 == Physics.RaycastNonAlloc(ray, _hits, 10.0f, 1 << 8))
                {
                    var t = 0.5f * (ray.direction.normalized.y + 1.0f);
                    newColor = Color.Lerp(new Color(1.0f, 1.0f, 1.0f), new Color(0.5f, 0.7f, 1.0f), t);
                }
                else
                {
                    newColor = Color.red;
                }
                _renderTexture.SetPixel(x, y, newColor);
            }
        }
        _renderTexture.Apply();
    }

}
