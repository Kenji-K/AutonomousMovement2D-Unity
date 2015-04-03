using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.AutonomousMovement {
    public class ObstacleAvoidance2D : SteeringBehaviour2D {
        public float minDetectionBoxLength = 1f;

        void Reset() {
            if (World2D.Instance != null) {
                Weight = World2D.Instance.DefaultSettings.ObstacleAvoidanceWeight;
                Probability = World2D.Instance.DefaultSettings.ObstacleAvoidanceProb;
                minDetectionBoxLength = World2D.Instance.DefaultSettings.MinDetectionBoxLength;
            }
        }

        public override Vector2 GetVelocity() {
            return GetVelocity(agent, World2D.Instance.Obstacles, minDetectionBoxLength);
        }

        public static Vector2 GetVelocity(SteeringAgent2D agent, List<CircleCollider2D> obstacles, float minDetectionBoxLength = 1) {var boxLength = minDetectionBoxLength +
                            (agent.GetComponent<Rigidbody2D>().velocity.magnitude / agent.MaxSpeed) * 
                            minDetectionBoxLength;

			var candidates = obstacles.Where(o => (o.transform.position - agent.transform.position).magnitude < boxLength);
            CircleCollider2D closestIntersectingObstacle = null;
            float distanceToClosestIO = float.MaxValue;
            Vector2 localPosOfClosestObstacle = Vector2.zero;

            foreach (var obstacle in candidates) {
                Vector2 localPos = agent.transform.InverseTransformPoint(obstacle.transform.position);

                //If y is negative, then they are behind us, and not important
                if (localPos.y >= 0) {
                    float expandedRadius = obstacle.radius + agent.Radius;

                    if (Mathf.Abs(localPos.x) < expandedRadius) {
                        float cX = localPos.x;
                        float cY = localPos.y;

                        float sqrtPart = Mathf.Sqrt(expandedRadius * expandedRadius - cX * cX);
                        float ip = cY - sqrtPart;

                        if (ip <= 0) {
                            ip = cY + sqrtPart;
                        }

                        if (ip < distanceToClosestIO) {
                            distanceToClosestIO = ip;
                            closestIntersectingObstacle = obstacle;
                            localPosOfClosestObstacle = localPos;
                        }
                    }
                }
            }

            Vector2 steering = Vector2.zero;

            if (closestIntersectingObstacle != null) {
                //float multiplier = 1f + (boxLength - localPosOfClosestObstacle.y) / boxLength;
                //steering.x = (closestIntersectingObstacle.radius - Mathf.Abs(localPosOfClosestObstacle.x)) * multiplier * -Mathf.Sign(localPosOfClosestObstacle.x);
                //const float BRAKING_WEIGHT = 0.2f;
                //steering.y = (closestIntersectingObstacle.radius - localPosOfClosestObstacle.y) * BRAKING_WEIGHT;
                //steering = agent.transform.TransformDirection(steering);

                //var ahead = agent.rigidbody2D.velocity.normalized * distanceToClosestIO;
                //var avoidance = ahead - (Vector2)(closestIntersectingObstacle.transform.position - agent.transform.position);
                //steering = avoidance.normalized * agent.MaxForce;

                //TODO -> Implement better version
                steering = -new Vector2(localPosOfClosestObstacle.x * 4, localPosOfClosestObstacle.y / 2);
                if (localPosOfClosestObstacle.magnitude > closestIntersectingObstacle.radius) {
                    steering = steering.normalized * agent.MaxForce / distanceToClosestIO;
                }

                steering = agent.transform.TransformDirection(steering);

                Debug.DrawLine(closestIntersectingObstacle.transform.position, (Vector2)closestIntersectingObstacle.transform.position + steering, Color.yellow);
            }
            
            return steering;
        }

        public override int CalculationOrder {
            get { return 1; }
        }
    }
}
