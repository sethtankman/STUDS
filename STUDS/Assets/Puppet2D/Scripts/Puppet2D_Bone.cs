using UnityEngine;
using System.Collections;
namespace Puppet2D
{
	[ExecuteInEditMode]
	public class Puppet2D_Bone : MonoBehaviour
	{
		private Mesh _mesh;
		private Mesh _meshJoint;
		public float Radius = 1f;
		public Color Color = Color.white;
		public int Order = 0;
				// Use this for initialization
		void Start()
		{
			string path = "bone";
			_mesh = (Mesh)Resources.Load(path, typeof(Mesh));
			path = "joint";
			_meshJoint = (Mesh)Resources.Load(path, typeof(Mesh));
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void OnDrawGizmos()
		{
			DrawThisBone(Color*1.35f);

		}
		private void DrawThisBone(Color col)
		{
			if (this.enabled)
			{
				Gizmos.color = col;
				if (_mesh == null)
				{
					string path = "bone";
					_mesh = (Mesh)Resources.Load(path, typeof(Mesh));
				}
				if (_meshJoint == null)
				{
					string path = "joint";
					_meshJoint = (Mesh)Resources.Load(path, typeof(Mesh));
				}

				foreach (Transform child in transform)
				{
					if (child.transform.GetComponent("Puppet2D_Bone") != null)
					{
						Vector3 nudge = (child.position - transform.position).normalized * (Radius * .7f);

						float scaler = Vector3.Distance(child.position - nudge, transform.position + nudge);
						//Gizmos.DrawWireMesh(transform.position, new Vector3(scaler, scaler, scaler)) ;
						Vector3 look = child.position - transform.position;
						Quaternion rot = Quaternion.identity;
						if (look != Vector3.zero)
							rot = Quaternion.LookRotation(look, Vector3.forward);

						Gizmos.color = col;
						Gizmos.DrawMesh(_mesh, 0, transform.position + nudge, rot, new Vector3(Radius, Radius, scaler));
					}
					
				}
				Gizmos.color = col;
				Gizmos.DrawMesh(_meshJoint, 0, transform.position, Quaternion.identity, new Vector3(Radius, Radius, Radius) * .7f);
				Gizmos.color = new Color(col.r * .333f, col.g * .333f, col.b * .333f, 1f);
				Gizmos.DrawMesh(_meshJoint, 0, transform.position, Quaternion.identity, new Vector3(Radius, Radius, Radius) * .5f);
			}
		}
		private void OnDrawGizmosSelected()
		{
			DrawThisBone(new Color(1f, .588f, 0f, 1f) * 1.35f);
		}
		private void OnHideGizmos()
		{

		}
	}

}
