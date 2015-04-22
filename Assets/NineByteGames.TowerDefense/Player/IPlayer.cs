﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NineByteGames.TowerDefense.Player
{
  /// <summary> An object that represents a player. </summary>
  internal interface IPlayer
  {
    /// <summary> The object that owns the player. </summary>
    GameObject Owner { get; }

    /// <summary> The layer on which projectiles should be created. </summary>
    Layer ProjectileLayer { get; }

    /// <summary> The places items can be placed. </summary>
    AttachmentToPositionLookup AttachmentPoints { get; }

    /// <summary> Indicates where the player is currently aiming. </summary>
    IPlayerCursor Cursor { get; }
  }
}