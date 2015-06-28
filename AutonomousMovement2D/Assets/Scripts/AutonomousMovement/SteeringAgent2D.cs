using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kensai.Util.Extensions;

namespace Kensai.AutonomousMovement {
    [RequireComponent(typeof(Rigidbody2D))]
    public class SteeringAgent2D : MonoBehaviour {
        private List<SteeringBehaviour2D> steeringBehaviours = new List<SteeringBehaviour2D>(16);
        public List<SteeringBehaviour2D> SteeringBehaviours {
            get { return steeringBehaviours; }
            set { steeringBehaviours = value; }
        }

        private List<ITargettedSteeringBehaviour> targettedSteeringBehaviours = new List<ITargettedSteeringBehaviour>(8);
        public List<ITargettedSteeringBehaviour> TargettedSteeringBehaviours {
            get { return targettedSteeringBehaviours; }
            set { targettedSteeringBehaviours = value; }
        }

        private Vector2 heading = Vector2.up;
        public Vector2 Heading {
            get { return heading; }
            private set { heading = value; }
        }

        private HashSet<SteeringAgent2D> neighbors = new HashSet<SteeringAgent2D>(new List<SteeringAgent2D>(100));
        /// <summary>
        /// Contains other Steering Agents that are close to the current one, as defined by NeighborRadius
        /// </summary>
        public IEnumerable<SteeringAgent2D> Neighbors {
            get { return neighbors; }
            //set { neighbors = value; }
        }

        private HashSet<SteeringAgent2D> targetAgents = new HashSet<SteeringAgent2D>(new List<SteeringAgent2D>(100));
        public HashSet<SteeringAgent2D> TargetAgents {
            get {
                targetAgents.Clear();
                for (int i = 0; i < TargettedSteeringBehaviours.Count; i++) {
                    if (TargettedSteeringBehaviours[i].TargetAgent1 != null) {
                        targetAgents.Add(TargettedSteeringBehaviours[i].TargetAgent1);
                    }
                    if (TargettedSteeringBehaviours[i].TargetAgent2 != null) {
                        targetAgents.Add(TargettedSteeringBehaviours[i].TargetAgent2);
                    }
                }

                return targetAgents;
            }
        }

        public float MaxSpeed = 5;
        private float maxForce = 3;
        public float MaxForce {
            get { return maxForce * World2D.Instance.DefaultSettings.SteeringForceTweaker; }
            set { maxForce = value; }
        }
        private Rigidbody2D rigidbody2D;
        public Rigidbody2D Rigidbody2D {
            get { return rigidbody2D; }
            private set { rigidbody2D = value; }
        }
        public float Radius = 1;
        public float NeighborRadius = 3;
        public bool DrawGizmos = false;

        private Vector2 steeringForce;
        private Smoother<Vector2> headingSmoother;
        private int behavioursRequiringNeighbors = 0;
        private Vector2 previousPosition;

        void Reset() {
            NeighborRadius = World2D.Instance.DefaultSettings.DefaultNeighborRadius;
        }

        void Awake() {
            var circleCollider = GetComponent<CircleCollider2D>();

            //TODO -> Revise how scale affects these things
            if (circleCollider != null && circleCollider.enabled) {
                Radius = circleCollider.radius;
            } else if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().enabled) {
                Radius = Mathf.Max(GetComponent<Collider2D>().bounds.extents.x, GetComponent<Collider2D>().bounds.extents.y);
            } else if (GetComponent<Renderer>() != null && GetComponent<Renderer>().enabled) {
                Radius = Mathf.Max(GetComponent<Renderer>().bounds.extents.x, GetComponent<Renderer>().bounds.extents.y);
            }

            rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        }

        void Start() {
            if (World2D.Instance == null) { 
                throw new Exception("There is no instance of World2D. You must attach a World2D script to the scene."); 
            }

            if (World2D.Instance.DefaultSettings.SmoothHeadingOn) {
                headingSmoother = new Smoother<Vector2>(World2D.Instance.DefaultSettings.NumSamplesForSmoothing);
            }

            if (World2D.Instance.SpacePartition != null) {
                World2D.Instance.SpacePartition.AddEntity(this);
            }

            previousPosition = rigidbody2D.position;

            World2D.Instance.AgentList.Add(this);
        }

        void FixedUpdate() {

            if (World2D.Instance != null && World2D.Instance.wrapAround) { 
                WrapAround(rigidbody2D.position, World2D.Instance.worldSizeX, World2D.Instance.worldSizeY);
            }

            if (World2D.Instance.SpacePartition != null) {
                World2D.Instance.SpacePartition.UpdateEntity(this, previousPosition);
            }

            Profiler.BeginSample("Get Neighbors");
            if (behavioursRequiringNeighbors > 0) {
                GetNeighbors();
            }

            Profiler.EndSample();
            Profiler.BeginSample("Calculate compound");
            steeringForce = SteeringBehaviours.CalculateCompound(MaxForce, SteeringBehaviourExtensions.SteeringCombinationType.PrioritizedWeightedSum);

            Profiler.EndSample();
            Profiler.BeginSample("The rest");
            rigidbody2D.AddForce(steeringForce, ForceMode2D.Impulse);
            //var acceleration = steeringForce / rigidbody2D.mass;
            //rigidbody2D.velocity += acceleration;
            rigidbody2D.velocity = rigidbody2D.velocity.Truncate(MaxSpeed);

            if (rigidbody2D.velocity.sqrMagnitude > 0.000001) {
                Heading = rigidbody2D.velocity.normalized;

                if (World2D.Instance.DefaultSettings.SmoothHeadingOn) {
                    Heading = headingSmoother.Update(Heading).normalized;
                }
            }

            transform.up = Heading;
            previousPosition = rigidbody2D.position;

            Profiler.EndSample();
        }

        void OnDrawGizmos() {
            if (DrawGizmos) {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(rigidbody2D.position, rigidbody2D.velocity + rigidbody2D.position);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(rigidbody2D.position, steeringForce + rigidbody2D.position);
                Gizmos.DrawWireSphere(rigidbody2D.position, NeighborRadius);
            }
        } 

        void OnDestroy() {
            if (World2D.Instance != null) { 
                World2D.Instance.AgentList.Remove(this);
            }
        }

        public void RegisterSteeringBehaviour(SteeringBehaviour2D behaviour) {
            SteeringBehaviours.Add(behaviour);
            if (behaviour is ITargettedSteeringBehaviour) TargettedSteeringBehaviours.Add((ITargettedSteeringBehaviour)behaviour);
            if (behaviour.RequiresNeighborList) behavioursRequiringNeighbors++;
        }

        public void DeregisterSteeringBehaviour(SteeringBehaviour2D behaviour) {
            SteeringBehaviours.Remove(behaviour);
            if (behaviour is ITargettedSteeringBehaviour) TargettedSteeringBehaviours.Remove((ITargettedSteeringBehaviour)behaviour);
            if (behaviour.RequiresNeighborList) behavioursRequiringNeighbors--;
        }

        private void WrapAround(Vector2 position, float xWorldSize, float yWorldSize) {
            float x = position.x, y = position.y;
            if (rigidbody2D.position.x > xWorldSize) x = 0.0f;
            if (rigidbody2D.position.x < 0) x = xWorldSize;
            if (rigidbody2D.position.y > yWorldSize) y = 0.0f;
            if (rigidbody2D.position.y < 0) y = yWorldSize;
            rigidbody2D.position = new Vector2(x, y);
        }

        private IEnumerable<SteeringAgent2D> GetNeighbors() {
            neighbors.Clear();

            if (World2D.Instance.SpacePartition == null) { 
                foreach (var agent in World2D.Instance.AgentList) {
                    if (agent == this) continue;

                    var distance = (agent.rigidbody2D.position - rigidbody2D.position).magnitude + Radius + agent.Radius;

                    if (distance <= NeighborRadius) {
                        neighbors.Add(agent);
                    }
                }
            } else {
                var testRect = new Rect(rigidbody2D.position.x - NeighborRadius,
                                        rigidbody2D.position.y - NeighborRadius,
                                        rigidbody2D.position.x + NeighborRadius,
                                        rigidbody2D.position.y + NeighborRadius);
                foreach (var cell in World2D.Instance.SpacePartition.Cells) {
                    if (cell.Rect.Overlaps(testRect)) {
                        for (int i = 0; i < cell.Members.Count; i++) {
                            if (cell.Members[i] == this) continue;
                            var distance = (cell.Members[i].rigidbody2D.position - rigidbody2D.position).magnitude + Radius + cell.Members[i].Radius;
                            if (distance <= NeighborRadius) {
                                neighbors.Add(cell.Members[i]);
                            }
                        }
                    }
                }
            }

            return Neighbors;
        }
    }
}
