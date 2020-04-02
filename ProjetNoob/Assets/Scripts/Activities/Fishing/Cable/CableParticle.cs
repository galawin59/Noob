using UnityEngine;
using System.Collections;

public class CableParticle
{
	#region Class member variables

	private Vector2 _position, _oldPosition;
	private Transform _boundTo = null;
	private Rigidbody2D _boundRigid = null;

	#endregion


	#region Properties

	public Vector2 Position {
		get { return _position; }
		set { _position = value; }
	}
		
	public Vector2 Velocity {
		get { return (_position - _oldPosition); }
	}

	#endregion


	#region Constructor

	public CableParticle(Vector2 newPosition)
	{
		_oldPosition = _position = newPosition;
	}

	#endregion


	#region Public functions

	public void UpdateVerlet(Vector2 gravityDisplacement)
	{
		if (this.IsBound())
		{
			if (_boundRigid == null) {
				this.UpdatePosition(_boundTo.position);		
			}
			else
			{
				switch (_boundRigid.interpolation) 
				{
				case RigidbodyInterpolation2D.Interpolate:
					this.UpdatePosition(_boundRigid.position + (_boundRigid.velocity * Time.fixedDeltaTime) / 2);
					break;
				case RigidbodyInterpolation2D.None:
				default:
					this.UpdatePosition(_boundRigid.position + _boundRigid.velocity * Time.fixedDeltaTime);
					break;
				}
			}
		}
		else 
		{
            Vector2 newPosition = this.Position + this.Velocity + gravityDisplacement;
			this.UpdatePosition(newPosition);
		}
	}

	public void UpdatePosition(Vector2 newPos) 
	{
		_oldPosition = _position;
		_position = newPos;
	}

	public void Bind(Transform to)
	{
		_boundTo = to;
		_boundRigid = to.GetComponent<Rigidbody2D>();
		_oldPosition = _position = _boundTo.position;
	}
		
	public void UnBind()
	{
		_boundTo = null;
		_boundRigid = null;
	}
		
	public bool IsFree()
	{
		return (_boundTo == null);
	}
		
	public bool IsBound()
	{
		return (_boundTo != null);
	}

	#endregion
}