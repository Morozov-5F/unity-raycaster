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

    // Update is called once per frame
    void Update()
    {
        Vector3 a = new Vector3();
        int width = _renderTexture.width;
        int height = _renderTexture.height;
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                a.x = (float) x / (width - 1);
                a.y = (float) y / (height - 1);

                var ray = _camera.ViewportPointToRay(a);
                
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
