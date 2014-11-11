using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineByteGames.TowerDefense.Utils
{
  internal class MathUtils
  {
    /// <summary> The maximum number of degrees to turn per iteration. </summary>
    public const float MaxDegreesPerSecond = 30;

    /// <summary> The number of ticks that occur. </summary>
    public const float TicksPerSecond = 50f;

    /// <summary> The number of seconds in a tick. </summary>
    public const float SecondsPerTick = 1.0f / TicksPerSecond;

    /// <summary> Convert a number of ticks into a timespan. </summary>
    /// <param name="ticks"> The ticks to convert into a timespan. </param>
    /// <returns> A timespan representing the given number of ticks. </returns>
    public static TimeSpan FromTicks(int ticks)
    {
      return TimeSpan.FromSeconds(SecondsPerTick * ticks);
    }

    /// <summary>
    ///  2D rotate one object to face another.  It aligns the top of the object to be looking at the
    ///  other object's center, not rotating more than a fixed amount per call.
    /// </summary>
    /// <param name="objectToRotate"> The object to rotate. </param>
    /// <param name="target"> The target to rotate towards. </param>
    /// <param name="rateToTurn"> The time to use to interpolate the current rotation to the new
    ///  rotation. </param>
    public static void RotateTowards(GameObject objectToRotate, GameObject target, float rateToTurn)
    {
      var targetTransform = target.transform;
      var objectTransform = objectToRotate.transform;

      // get the desired vector rotation
      Vector3 vectorToTarget = targetTransform.position - objectTransform.position;

      // convert it into an engle
      float desiredAngle = (Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) - Mathf.PI / 2) * Mathf.Rad2Deg;

      objectTransform.rotation = Quaternion.RotateTowards(objectTransform.rotation,
                                                          Quaternion.AngleAxis(desiredAngle, Vector3.forward),
                                                          MaxDegreesPerSecond * rateToTurn);
    }
  }
}