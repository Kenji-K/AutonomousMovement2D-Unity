using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Flee2D : SteeringBehaviour2D {
        public Vector2 TargetPoint;
        public float PanicDistance = -1; //TODO -> Decidir si va en config settings

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.FleeWeight;
                Probability = World2D.Instance.DefaultSettings.FleeProb;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, TargetPoint, PanicDistance);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, Vector2 targetPoint, float panicDistance = -1) {
            if (panicDistance > 0 && (targetPoint - agent.rigidbody2D.position).sqrMagnitude > panicDistance * panicDistance) {
                return new Vector2(0, 0);
            }

            Vector2 desiredVelocity = (agent.rigidbody2D.position - targetPoint).normalized * agent.MaxSpeed;
            return (desiredVelocity - agent.rigidbody2D.velocity);
        }
    }
}
