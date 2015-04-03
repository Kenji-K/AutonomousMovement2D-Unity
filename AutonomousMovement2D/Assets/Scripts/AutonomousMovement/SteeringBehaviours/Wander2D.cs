using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Wander2D : SteeringBehaviour2D {
        public float WanderRadius;
        public float WanderDistance;
        public float WanderJitter;
        public bool DrawGizmos = false;

        private Vector2 wanderTarget;
        private Vector2 targetWorld;
        
        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.WanderWeight;
                Probability = World2D.Instance.DefaultSettings.WanderProb;
            }
        }

        public override Vector2 GetVelocity() {
            wanderTarget += new Vector2(UnityEngine.Random.Range(-1f, 1f) * WanderJitter,
                                        UnityEngine.Random.Range(-1f, 1f) * WanderJitter);

            wanderTarget = wanderTarget.normalized * WanderRadius;
            Vector2 targetLocal = wanderTarget + new Vector2(0, WanderDistance);
            targetWorld = agent.transform.TransformPoint(targetLocal);

            return targetWorld - agent.GetComponent<Rigidbody2D>().position;
        }

        void OnDrawGizmos() {
            if (DrawGizmos && agent != null) {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(targetWorld, 0.1f);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(agent.transform.TransformPoint(new Vector2(0, WanderDistance)), WanderRadius);
            }
        }
    }
}
