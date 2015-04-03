using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class Pursue2D : SteeringBehaviour2D {
        public SteeringAgent2D Evader = null;
        public float TurnCoefficient = 0;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.PursuitWeight;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, Evader, TurnCoefficient);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, SteeringAgent2D evader, float turnCoefficient = 0f) {
            if (evader == null) return Vector2.zero;

            var toEvader = evader.GetComponent<Rigidbody2D>().position - agent.GetComponent<Rigidbody2D>().position;
            float relativeHeading = Vector2.Dot(agent.Heading, evader.transform.up);

            if ((Vector2.Dot(toEvader, agent.transform.up) > 0) &&
                relativeHeading < -0.95) { //acos(0.95) = 18 degs
                    return Seek2D.GetVelocity(agent, evader.GetComponent<Rigidbody2D>().position);
            }

            //Not considered ahead so we predict where the evader will be
            float lookAheadTime = toEvader.magnitude / (agent.MaxSpeed + evader.GetComponent<Rigidbody2D>().velocity.magnitude);
            if (turnCoefficient != 0) {
                lookAheadTime += TurnaroundTime(agent, evader.GetComponent<Rigidbody2D>().position, turnCoefficient);
            }

            return Seek2D.GetVelocity(agent, evader.GetComponent<Rigidbody2D>().position + evader.GetComponent<Rigidbody2D>().velocity * lookAheadTime);
        }

        public static float TurnaroundTime(SteeringAgent2D agent, Vector2 targetPos, float turnCoefficient) {
            Vector2 toTarget = (targetPos - agent.GetComponent<Rigidbody2D>().position).normalized;
            var dot = Vector2.Dot(agent.Heading, toTarget);
            return (dot - 1) * -turnCoefficient;
        }
    }
}
