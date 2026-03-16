using UnityEngine;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Static gizmo drawing utilities for ConfigurableJoint visualization.
    /// Used by RagdollJointDebugger to show joint axes, limits, and selection highlights.
    /// </summary>
    public static class JointLimitGizmoHelper
    {
        private const float AxisLength = 0.25f;
        private const float ArcRadius = 0.2f;
        private const int ArcSegments = 16;

        /// <summary>
        /// Draw primary (red), secondary (green), and computed third (blue) axes.
        /// </summary>
        public static void DrawJointAxes(ConfigurableJoint joint)
        {
            if (joint == null) return;

            Vector3 pos = joint.transform.position;
            Vector3 primaryWorld = joint.transform.TransformDirection(joint.axis);
            Vector3 secondaryWorld = joint.transform.TransformDirection(joint.secondaryAxis);
            Vector3 thirdWorld = Vector3.Cross(primaryWorld, secondaryWorld).normalized;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + primaryWorld * AxisLength);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + secondaryWorld * AxisLength);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + thirdWorld * AxisLength);
        }

        /// <summary>
        /// Draw angular X limit range as an arc around the primary axis.
        /// </summary>
        public static void DrawAngularXLimitArc(ConfigurableJoint joint)
        {
            if (joint == null) return;
            if (joint.angularXMotion == ConfigurableJointMotion.Free) return;

            Vector3 pos = joint.transform.position;
            Vector3 axis = joint.transform.TransformDirection(joint.axis);
            Vector3 secondary = joint.transform.TransformDirection(joint.secondaryAxis);

            float lowLimit = joint.lowAngularXLimit.limit;
            float highLimit = joint.highAngularXLimit.limit;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f); // orange
            DrawArc(pos, axis, secondary, lowLimit, highLimit, ArcRadius);
        }

        /// <summary>
        /// Draw Y and Z angular limits as arcs.
        /// </summary>
        public static void DrawSwingLimitArcs(ConfigurableJoint joint)
        {
            if (joint == null) return;

            Vector3 pos = joint.transform.position;
            Vector3 primary = joint.transform.TransformDirection(joint.axis);
            Vector3 secondary = joint.transform.TransformDirection(joint.secondaryAxis);
            Vector3 third = Vector3.Cross(primary, secondary).normalized;

            // Y limit arc (around secondary axis)
            if (joint.angularYMotion != ConfigurableJointMotion.Free)
            {
                float yLimit = joint.angularYLimit.limit;
                Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.6f); // light green
                DrawArc(pos, secondary, primary, -yLimit, yLimit, ArcRadius * 0.8f);
            }

            // Z limit arc (around third axis)
            if (joint.angularZMotion != ConfigurableJointMotion.Free)
            {
                float zLimit = joint.angularZLimit.limit;
                Gizmos.color = new Color(0.3f, 0.3f, 1f, 0.6f); // light blue
                DrawArc(pos, third, primary, -zLimit, zLimit, ArcRadius * 0.8f);
            }
        }

        /// <summary>
        /// Draw a selection highlight ring around a position.
        /// </summary>
        public static void DrawHighlightRing(Vector3 position, float radius, Color color)
        {
            Gizmos.color = color;
            int segments = 24;
            for (int i = 0; i < segments; i++)
            {
                float a1 = (i / (float)segments) * Mathf.PI * 2f;
                float a2 = ((i + 1) / (float)segments) * Mathf.PI * 2f;
                Vector3 p1 = position + new Vector3(Mathf.Cos(a1), 0f, Mathf.Sin(a1)) * radius;
                Vector3 p2 = position + new Vector3(Mathf.Cos(a2), 0f, Mathf.Sin(a2)) * radius;
                Gizmos.DrawLine(p1, p2);
            }
        }

        /// <summary>
        /// Draw a small dot marker for unselected joints.
        /// </summary>
        public static void DrawJointDot(Vector3 position, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, 0.02f);
        }

        private static void DrawArc(Vector3 center, Vector3 axis, Vector3 startDir,
            float fromAngle, float toAngle, float radius)
        {
            Vector3 prevPoint = center;
            bool first = true;

            for (int i = 0; i <= ArcSegments; i++)
            {
                float t = i / (float)ArcSegments;
                float angle = Mathf.Lerp(fromAngle, toAngle, t);
                Quaternion rot = Quaternion.AngleAxis(angle, axis);
                Vector3 point = center + rot * startDir * radius;

                if (!first)
                {
                    Gizmos.DrawLine(prevPoint, point);
                }
                else
                {
                    // Draw line from center to arc start
                    Gizmos.DrawLine(center, point);
                    first = false;
                }

                prevPoint = point;
            }

            // Draw line from arc end back to center
            Gizmos.DrawLine(prevPoint, center);
        }
    }
}
