using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public bool wallPassed { get; set; }

    public SpriteRenderer Renderer { get; set; }

    private Collider2D Collider { get; set; }

    protected void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();

        SetVerticalSize();
    }

    private void SetVerticalSize()
    {
        float wallHeight = GameManager.Hr.CameraWorldBounds.size.y;

        BoxCollider2D coll = (BoxCollider2D)Collider;
        Renderer.size = new Vector2(1, wallHeight);
        coll.size = new Vector2(coll.size.x, wallHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
