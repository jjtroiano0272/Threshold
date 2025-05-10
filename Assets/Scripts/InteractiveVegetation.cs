// {/* =====================================================================================
// === 3D GO APPROACH =============================================
// ========================================================================================= */}
using UnityEngine;

public static class k
{
    public static class Layers
    {
        public const int PLAYER = 10; // Replace with actual Player layer index
    }
}

public class SpringValue
{
    public float position;
    public float velocity;
    public float damping = 0.9f;
    public float stiffness = 5f;

    public void ApplyForce(float force)
    {
        velocity += force;
    }

    public void Update(float deltaTime)
    {
        velocity -= position * stiffness * deltaTime;
        velocity *= damping;
        position += velocity * deltaTime;
    }

    public void ApplyForceStartingAtPosition(float force, float startPosition)
    {
        position = startPosition;
        ApplyForce(force);
    }
}

public class InteractiveVegetation : MonoBehaviour
{
    [SerializeField]
    private float BEND_FACTOR = 0.2f;

    [SerializeField]
    private float BEND_FORCE_ON_EXIT = 0.2f;
    private float _enterOffset;
    private bool _isRebounding;
    private bool _isBending;
    private float _exitOffset;
    private SpriteRenderer _renderer;

    // private Material _material;
    private float _colliderHalfWidth;
    private MeshFilter _meshFilter; // only works if using Quad
    private SpringValue _spring = new SpringValue();

    void Awake()
    {
        // _renderer = GetComponent<SpriteRenderer>();
        // _material = _renderer.material;
        _colliderHalfWidth = GetComponent<Collider2D>().bounds.extents.x;
        _meshFilter = GetComponent<MeshFilter>();
        _spring.Update(Time.deltaTime);
    }

    float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
    }

    // simple method to offset the top 2 verts of a quad based on the offset and BEND_FACTOR constant
    void setVertHorizontalOffset(float offset)
    {
        var verts = _meshFilter.mesh.vertices;

        verts[1].x = 0.5f + offset * BEND_FACTOR / transform.localScale.x;
        verts[3].x = -0.5f + offset * BEND_FACTOR / transform.localScale.x;

        _meshFilter.mesh.vertices = verts;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == k.Layers.PLAYER)
        {
            _enterOffset = col.transform.position.x - transform.position.x;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == k.Layers.PLAYER)
        {
            var offset = col.transform.position.x - transform.position.x;

            if (_isBending || Mathf.Sign(_enterOffset) != Mathf.Sign(offset))
            {
                _isRebounding = false;
                _isBending = true;

                // This shit kinda makes no sense. We define it and then use it as a var, then the same exact thing
                _colliderHalfWidth = col.bounds.size.x / 2;

                // figure out how far we have moved into the trigger and then map the offset to -1 to 1.
                // 0 would be neutral, -1 to the left and +1 to the right.
                // var radius = _colliderHalfWidth + col.bounds.size.x * 0.5f;
                var radius = _colliderHalfWidth * 2;
                _exitOffset = Map(offset, -radius, radius, -1f, 1f);
                setVertHorizontalOffset(_exitOffset);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == k.Layers.PLAYER)
        {
            if (_isBending)
            {
                // apply a force in the opposite direction that we are currently bending
                _spring.ApplyForceStartingAtPosition(
                    BEND_FORCE_ON_EXIT * Mathf.Sign(_exitOffset),
                    _exitOffset
                );
            }

            _isBending = false;
            _isRebounding = true;
        }
    }
}

// {/* =====================================================================================
// === 2D SPRITERENDER APPROACH =============================================
// ========================================================================================= */}
