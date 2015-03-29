﻿using System;
using System.Collections.Generic;
using System.Linq;
using NineByteGames.Common.Extensions;
using NineByteGames.TowerDefense.Behaviors;
using NineByteGames.TowerDefense.Equipment;
using NineByteGames.TowerDefense.Objects;
using NineByteGames.TowerDefense.Services;
using NineByteGames.TowerDefense.Signals;
using NineByteGames.TowerDefense.Utils;
using NineByteGames.TowerDefense.World.Grid;
using UnityEngine;

namespace NineByteGames.TowerDefense.Player
{
  /// <summary>
  ///  Contains all functionality related to the player and its current inventory.
  /// </summary>
  internal class InventoryBehavior : AttachedBehavior
  {
    #region Unity Properties

    [Tooltip("List of items currently in inventory")]
    public PlaceableObject[] inventoryList;

    [Tooltip("List of weapons currently in inventory")]
    public FirableWeapon[] weaponList;

    [Tooltip("The layer on which projectiles should be created")]
    public Layer projectileLayer;

    [Tooltip("How fast weapons can be switched")]
    public RateLimiter weaponSwapRate;

    [Tooltip("The rate at which buildings can be placed")]
    public RateLimiter buildingRate;

    #endregion

    private DataCollection<PlaceableObject> _placeables;
    private DataCollection<FirableWeapon> _weapons;
    private Vector3 _cursorLocation;
    private GameObject _placeablePreviewItem;
    private FireableWeaponInstance _currentWeapon;
    private AttachmentToPositionLookup _lookup;

    public void Start()
    {
      _placeables = new DataCollection<PlaceableObject>(inventoryList);
      _weapons = new DataCollection<FirableWeapon>(weaponList);

      _lookup = AttachmentPointsBehavior.RetrieveFor(Owner);

      _placeablePreviewItem = _placeables.Selected.PreviewItem.Clone();
      _currentWeapon = _weapons.Selected.CreateObjectInstance(Owner, _lookup[AttachmentPoint.Weapon]);
    }

    /// <summary> Updates the current location of the cursor. </summary>
    /// <param name="location"> The newest location of the cursor. </param>
    public void UpdateCursor(Vector3 location)
    {
      _cursorLocation = location;
      var lowerLeft = GridCoordinate.FromVector3(_cursorLocation);

      _placeablePreviewItem.GetComponent<Transform>().position =
        _placeables.Selected.Strategy.ConvertToGameObjectPosition(lowerLeft);
    }

    /// <summary> Activate the primary item, for example, firing a weapon. </summary>
    public void TryTrigger1()
    {
      _currentWeapon.Weapon.AttemptFire(Owner.GetComponent<Transform>(), projectileLayer);
    }

    /// <summary> Activate the secondary item, for example, placing an object. </summary>
    public void TryTrigger2()
    {
      if (!buildingRate.CanTrigger)
        return;

      buildingRate.Restart();

      var lowerLeft = GridCoordinate.FromVector3(_cursorLocation);

      if (Managers.Placer.CanCreate(lowerLeft, _placeables.Selected))
      {
        Managers.Placer.PlaceAt(lowerLeft, _placeables.Selected);
      }
    }

    /// <summary> Try to switch to the given inventory item. </summary>
    /// <param name="inventoryId"> The inventory item to switch to. </param>
    public void TrySwitchTo(int inventoryId)
    {
      if (!weaponSwapRate.CanTrigger)
        return;

      SwitchPlaceable(inventoryId);
      SwitchWeapon(inventoryId);

      weaponSwapRate.Restart();
    }

    private void SwitchPlaceable(int inventoryId)
    {
      if (!_placeables.SetSelectedIndex(inventoryId))
        return;

      // TODO do we want to cache this somehow
      _placeablePreviewItem.DestroySelf();
      _placeablePreviewItem = _placeables.Selected.PreviewItem.Clone();
    }

    private void SwitchWeapon(int inventoryId)
    {
      if (!_weapons.SetSelectedIndex(inventoryId))
        return;

      // TODO implement switching weapons animation
      _currentWeapon.Owner.DestroySelf();
      _currentWeapon = _weapons.Selected.CreateObjectInstance(Owner, _lookup[AttachmentPoint.Weapon]);
    }
  }
}