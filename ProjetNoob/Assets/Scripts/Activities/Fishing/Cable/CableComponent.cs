using UnityEngine;
using System;
using System.Collections;


public class CableComponent : MonoBehaviour
{
    #region Class members

    [SerializeField] private Transform endPoint;
    [SerializeField] private Material cableMaterial;

    // Cable config
    [SerializeField] private float cableLength = 0.5f;
    [SerializeField] private int totalSegments = 5;
    [SerializeField] private float segmentsPerUnit = 2f;
    private int segments = 0;
    [SerializeField] private float cableWidth = 0.1f;

    // Solver config
    [SerializeField] private int verletIterations = 1;
    [SerializeField] private int solverIterations = 1;

    //[Range(0,3)]
    [SerializeField] private float stiffness = 1f;

    private LineRenderer line;
    private CableParticle[] points;

    private Transform objectToFollow;

    public Transform ObjectToFollow
    {
        get
        {
            return objectToFollow;
        }

        set
        {
            objectToFollow = value;
        }
    }

    public float CableLength
    {
        get
        {
            return cableLength;
        }

        set
        {
            cableLength = value;
        }
    }

    #endregion


    #region Initial setup

    void Start()
    {
        InitCableParticles();
        InitLineRenderer();
    }

    /**
	 * Init cable particles
	 * 
	 * Creates the cable particles along the cable length
	 * and binds the start and end tips to their respective game objects.
	 */
    void InitCableParticles()
    {
        // Calculate segments to use
        if (totalSegments > 0)
            segments = totalSegments;
        else
            segments = Mathf.CeilToInt(cableLength * segmentsPerUnit);

        Vector2 cableDirection = (endPoint.position - transform.position).normalized;
        float initialSegmentLength = cableLength / segments;
        points = new CableParticle[segments + 1];

        // Foreach point
        for (int pointIdx = 0; pointIdx <= segments; pointIdx++)
        {
            // Initial position
            Vector2 initialPosition = (Vector2)transform.position + (cableDirection * (initialSegmentLength * pointIdx));
            points[pointIdx] = new CableParticle(initialPosition);
        }

        // Bind start and end particles with their respective gameobjects
        CableParticle start = points[0];
        CableParticle end = points[segments];
        start.Bind(this.transform);
        end.Bind(endPoint.transform);
    }

    /**
	 * Initialized the line renderer
	 */
    void InitLineRenderer()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
        line.startWidth = cableWidth;
        line.endWidth = cableWidth;
        line.positionCount = segments + 1;
        line.material = cableMaterial;
        line.GetComponent<Renderer>().enabled = true;
    }

    #endregion


    #region Render Pass

    void Update()
    {
        if (ObjectToFollow)
        {
            RenderCable();
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    /**
	 * Render Cable
	 * 
	 * Update every particle position in the line renderer.
	 */
    void RenderCable()
    {
        for (int pointIdx = 0; pointIdx < segments + 1; pointIdx++)
        {
            line.SetPosition(pointIdx, points[pointIdx].Position);
        }
    }

    #endregion


    #region Verlet integration & solver pass

    void FixedUpdate()
    {
        if (ObjectToFollow)
        {
            if (ObjectToFollow.GetComponent<Hook>().RefActivity.PossibleHookedFish)
            {
                endPoint.position = ObjectToFollow.position - Vector3.up*0.15f;
            }
            else
            {
                endPoint.position = ObjectToFollow.position;
            }
        }
        for (int verletIdx = 0; verletIdx < verletIterations; verletIdx++)
        {
            VerletIntegrate();
            SolveConstraints();
        }

     
    }

    /**
	 * Verler integration pass
	 * 
	 * In this step every particle updates its position and speed.
	 */
    void VerletIntegrate()
    {
        Vector2 gravityDisplacement = Time.fixedDeltaTime * Time.fixedDeltaTime * Physics.gravity;
        foreach (CableParticle particle in points)
        {
            particle.UpdateVerlet(gravityDisplacement);
        }
    }

    /**
	 * Constrains solver pass
	 * 
	 * In this step every constraint is addressed in sequence
	 */
    void SolveConstraints()
    {
        // For each solver iteration..
        for (int iterationIdx = 0; iterationIdx < solverIterations; iterationIdx++)
        {
            SolveDistanceConstraint();
        }
    }

    #endregion


    #region Solver Constraints

    /**
	 * Distance constraint for each segment / pair of particles
	 **/
    void SolveDistanceConstraint()
    {
        float segmentLength = cableLength / segments;
        for (int SegIdx = 0; SegIdx < segments; SegIdx++)
        {
            CableParticle particleA = points[SegIdx];
            CableParticle particleB = points[SegIdx + 1];

            // Solve for this pair of particles
            SolveDistanceConstraint(particleA, particleB, segmentLength);
        }
    }

    /**
	 * Distance Constraint 
	 * 
	 * This is the main constrains that keeps the cable particles "tied" together.
	 */
    void SolveDistanceConstraint(CableParticle particleA, CableParticle particleB, float segmentLength)
    {
        // Find current vector between particles
        Vector2 delta = particleB.Position - particleA.Position;
        // 
        float currentDistance = delta.magnitude;
        float errorFactor = (currentDistance - segmentLength) / currentDistance;

        // Only move free particles to satisfy constraints
        if (particleA.IsFree() && particleB.IsFree())
        {
            particleA.Position += errorFactor * 0.5f * delta;
            particleB.Position -= errorFactor * 0.5f * delta;
        }
        else if (particleA.IsFree())
        {
            particleA.Position += errorFactor * delta;
        }
        else if (particleB.IsFree())
        {
            particleB.Position -= errorFactor * delta;
        }
    }

    #endregion
}
