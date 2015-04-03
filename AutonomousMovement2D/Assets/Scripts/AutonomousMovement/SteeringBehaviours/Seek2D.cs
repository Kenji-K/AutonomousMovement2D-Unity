using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Seek2D : SteeringBehaviour2D {
        public Vector2 TargetPoint;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.HideWeight;
                Probability = World2D.Instance.DefaultSettings.HideProb;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, TargetPoint);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, Vector2 targetPoint) {
            Vector2 desiredVelocity = (targetPoint - agent.GetComponent<Rigidbody2D>().position).normalized * agent.MaxSpeed;
            return (desiredVelocity - agent.GetComponent<Rigidbody2D>().velocity);
        }
    }
}
