using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public bool InputAllowed = true;

    public float ThrustForce;
    public Rigidbody2D Engine;
    public JetAnimator Jet;
    public AudioClip CrashClip;

    [Header("Pitch Setting")]
    public float maxAscendingAngle;
    public float maxDescendingAngle;

    public float AscendResponse;
    public float DescendResponse;

    public Rigidbody2D Rigidbody { get; set; }
    public AudioSource Audio { get; set; }

    private Quaternion PitchUpRotation;
    private Quaternion PitchDownRotation;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Audio = GetComponent<AudioSource>();

        PitchUpRotation = Quaternion.Euler(0, 0, 30);
        PitchDownRotation = Quaternion.Euler(0, 0, -90);
    }

    private bool WasScreenTouched()
    {
        if (Input.touchCount == 1)
            return Input.GetTouch(0).phase == TouchPhase.Began;
        else
            return false;
    }

    private void ApplyThrust()
    {
        Rigidbody.AddForce(Vector2.up * ThrustForce, ForceMode2D.Force);
        Rigidbody.velocity = Vector2.zero;
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.rotation.eulerAngles), PitchUpRotation, AscendResponse);

        Jet.AnimateThrust();
    }

    private void HeadTowardsEarth()
    {
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.rotation.eulerAngles), PitchDownRotation, DescendResponse);
    }

    private void ProcessInput()
    {
        if (WasScreenTouched())
            ApplyThrust();
    }

    private void DropEngine()
    {
        Destroy(Jet.gameObject);
        Engine.transform.parent = null;
        Engine.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    private void Death()
    {
        Audio.PlayOneShot(CrashClip, GameManager.Hr.SettingsManager.CurrentVolume);

        InputAllowed = false;
        DropEngine();
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.Hr.LevelManager.EndLevel();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Death();
    }

    public void OnDestroy()
    {
        if (Engine != null)
            Destroy(Engine.gameObject);
    }

    private void CheckHeight()
    {
        if (transform.position.y > GameManager.Hr.CameraWorldBounds.center.y + GameManager.Hr.CameraWorldBounds.extents.y)
            Death();
    }

    void FixedUpdate()
    {
        if (InputAllowed)
        {
            ProcessInput();
            HeadTowardsEarth();
            CheckHeight();
        }
    }
}
