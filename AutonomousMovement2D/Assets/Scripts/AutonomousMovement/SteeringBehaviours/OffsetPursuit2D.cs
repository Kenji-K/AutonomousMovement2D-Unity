using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kensai.AutonomousMovement {
    public class OffsetPursuit2D : SteeringBehaviour2D {
        public SteeringAgent2D Target = null;
        public Vector2 Offset = Vector2.zero;
        public float SlowingDistance = 1f;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.OffsetPursuitWeight;
                SlowingDistance = World2D.Instance.DefaultSettings.ArriveSlowingDistance;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, Target, Offset, SlowingDistance);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, SteeringAgent2D target, Vector2 offset, float slowingDistance = 1f) {
            Vector2 worldOffsetPos = target.transform.TransformPoint(offset);
            Vector2 toOffset = worldOffsetPos - agent.rigidbody2D.position;
            float lookAheadTime = toOffset.magnitude / (agent.MaxSpeed + target.rigidbody2D.velocity.magnitude);
            return Arrive2D.GetVelocity(agent, worldOffsetPos + target.rigidbody2D.velocity * lookAheadTime, slowingDistance);
        }
    }
}