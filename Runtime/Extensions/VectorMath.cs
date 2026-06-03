using UnityEngine;

namespace Dreamy.Core
{
    /// <summary>
    /// Static helper class for advanced geometric vector operations.
    /// </summary>
    public static class VectorMath
    {
        /// <summary>
        /// Returns the signed angle (degrees) between two vectors on a plane defined by its normal.
        /// </summary>
        public static float GetAngle(Vector3 vector1, Vector3 vector2, Vector3 planeNormal)
        {
            float angle = Vector3.Angle(vector1, vector2);
            float sign = Mathf.Sign(Vector3.Dot(planeNormal, Vector3.Cross(vector1, vector2)));
            return angle * sign;
        }

        /// <summary>
        /// Returns the dot product of <paramref name="vector"/> projected onto the normalized <paramref name="direction"/>.
        /// </summary>
        public static float GetDotProduct(Vector3 vector, Vector3 direction) =>
            Vector3.Dot(vector, direction.normalized);

        /// <summary>
        /// Removes the component of <paramref name="vector"/> that is in <paramref name="direction"/>.
        /// </summary>
        public static Vector3 RemoveDotVector(Vector3 vector, Vector3 direction)
        {
            direction.Normalize();
            return vector - direction * Vector3.Dot(vector, direction);
        }

        /// <summary>
        /// Extracts and returns only the component of <paramref name="vector"/> that aligns with <paramref name="direction"/>.
        /// </summary>
        public static Vector3 ExtractDotVector(Vector3 vector, Vector3 direction)
        {
            direction.Normalize();
            return direction * Vector3.Dot(vector, direction);
        }

        /// <summary>
        /// Rotates <paramref name="vector"/> from the <paramref name="upDirection"/> plane onto <paramref name="planeNormal"/>.
        /// </summary>
        public static Vector3 RotateVectorOntoPlane(Vector3 vector, Vector3 planeNormal, Vector3 upDirection)
        {
            var rotation = Quaternion.FromToRotation(upDirection, planeNormal);
            return rotation * vector;
        }

        /// <summary>
        /// Projects <paramref name="point"/> onto a line defined by <paramref name="lineStart"/> and <paramref name="lineDirection"/>.
        /// </summary>
        public static Vector3 ProjectPointOntoLine(Vector3 lineStart, Vector3 lineDirection, Vector3 point)
        {
            var toPoint = point - lineStart;
            return lineStart + lineDirection * Vector3.Dot(toPoint, lineDirection);
        }

        /// <summary>
        /// Moves <paramref name="current"/> toward <paramref name="target"/> at <paramref name="speed"/> units per second.
        /// </summary>
        public static Vector3 IncrementTowardTarget(Vector3 current, Vector3 target, float speed, float deltaTime)
        {
            return Vector3.MoveTowards(current, target, speed * deltaTime);
        }
    }
}
