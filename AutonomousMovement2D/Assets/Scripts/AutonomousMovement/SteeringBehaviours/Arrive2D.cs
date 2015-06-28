using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Arrive2D : SteeringBehaviour2D {
        public Vector2 TargetPoint;
        public float SlowingDistance = 1f;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.ArriveWeight;
                Probability = World2D.Instance.DefaultSettings.ArriveProb;
                SlowingDistance = World2D.Instance.DefaultSettings.ArriveSlowingDistance;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, TargetPoint, SlowingDistance);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, Vector2 targetPoint, float slowingDistance = 1f) {
            if (slowingDistance < 0) throw new InvalidOperationException("The slowing distance can't be negative.");

            //var toTarget = targetPoint - agent.Rigidbody2D.position;
            //var distance = toTarget.magnitude;

            //if (distance <= 0) {
            //    return new Vector2(0, 0);
            //} else {
            //    var rampedSpeed = agent.MaxSpeed * (distance / slowingDistance);
            //    var clippedSpeed = Mathf.Min(rampedSpeed, agent.MaxSpeed);
            //    Vector2 desiredVelocity = (clippedSpeed / distance) * toTarget;
            //    return (desiredVelocity - agent.Rigidbody2D.velocity);
            //}

            var toTarget = targetPoint - agent.Rigidbody2D.position;
            if (toTarget.magnitude <= 0) {
                return new Vector2(0, 0);
            } else {
                return ((Mathf.Min(agent.MaxSpeed * (toTarget.magnitude / slowingDistance), agent.MaxSpeed) / toTarget.magnitude) * toTarget - agent.Rigidbody2D.velocity);
            }
        }

        public override int CalculationOrder {
            get { return 10; }
        }
    }
}
