using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Evade2D : SteeringBehaviour2D {
        public SteeringAgent2D Pursuer = null;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.EvadeWeight;
                Probability = World2D.Instance.DefaultSettings.EvadeProb;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(this.agent, Pursuer);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, SteeringAgent2D pursuer) {
            if (pursuer == null) return Vector2.zero;
            var toPursuer = pursuer.rigidbody2D.position - agent.rigidbody2D.position;

            //Not considered ahead so we predict where the evader will be
            float lookAheadTime = toPursuer.magnitude / (agent.MaxSpeed + pursuer.rigidbody2D.velocity.magnitude);

            return Flee2D.GetVelocity(agent, pursuer.rigidbody2D.position + pursuer.rigidbody2D.velocity * lookAheadTime);
        }
    }
}
