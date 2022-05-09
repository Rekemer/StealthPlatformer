using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beep : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public float timeInterval;
    public Color targetColor = Color.red;
    public Color defaultColor = Color.black;
    [SerializeField] private LerpClass.InterType _interType;

    public bool IsTurnedOff { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Flash());
        // InvokeRepeating("Flashing",0f,timeInterval);
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator Flash()
    {
        float timer = 0;
        while (true)
        {
            if (!IsTurnedOff)
            {
                timer += Time.deltaTime;
                if (timer >= timeInterval)
                {
                    Flashing();
                    timer = 0;
                }
            }
            else
            {
                _spriteRenderer.color = defaultColor;
            }
      
            yield return null;
        }
        
    }
    void Flashing()
    {
        Color currentColor = _spriteRenderer.color;
        if (currentColor == targetColor)
        {
            StartCoroutine(FlashRoutine(defaultColor));
        }
        else
        {
            StartCoroutine(FlashRoutine(targetColor));
        }
    }
    
    IEnumerator FlashRoutine(Color targetColour)
    {
        Color currentColor = _spriteRenderer.color;
        float t = 0;
        while (currentColor != targetColour)
        {
            t+=Time.deltaTime;
            var interp = LerpClass.Lerp(t,_interType);
            currentColor = Color.Lerp(currentColor, targetColour, interp);
            _spriteRenderer.color = currentColor;
            yield return null;
        }
    }
    
}
