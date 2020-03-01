using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitaire solitaire;

    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();

        foreach (Sprite face in solitaire.cardFaces)
        {
            if(this.name == face.name)
            {
                cardFace = face;
            }
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectable.faceUp)
        {
            spriteRenderer.sprite = cardFace;
        } else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}
