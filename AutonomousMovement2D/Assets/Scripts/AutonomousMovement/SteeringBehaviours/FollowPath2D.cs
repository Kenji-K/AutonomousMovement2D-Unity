using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class FollowPath2D : SteeringBehaviour2D {
        public NavMesh2D NavMesh;
        public float WaypointSeekDistance = 0.5f;
        public bool DrawGizmos = false;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.FollowPathWeight;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, NavMesh, WaypointSeekDistance);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, NavMesh2D navMesh, float waypointSeekDistance = 0.5f) {
            if (navMesh == null || !navMesh.CurrentWaypoint.HasValue) return Vector2.zero;

            var currentWaypoint = navMesh.CurrentWaypoint.Value;
            if ((agent.GetComponent<Rigidbody2D>().position - currentWaypoint).magnitude < waypointSeekDistance) {
                navMesh.SetNextWaypoint();
            }

            if (!navMesh.IsFinished()) {
                return Seek2D.GetVelocity(agent, navMesh.CurrentWaypoint.Value);
            } else {
                return Arrive2D.GetVelocity(agent, navMesh.CurrentWaypoint.Value, 2f);
            }
        }

        public void OnDrawGizmos() {
            if (DrawGizmos && agent != null && NavMesh.WaypointList != null) {
                foreach (var waypoint in NavMesh.WaypointList) {
                    if (NavMesh.IsFinished()) {
                        Gizmos.color = Color.red;
                    } else { 
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawWireSphere(waypoint, WaypointSeekDistance);
                } 
            }
        }
    }
}
