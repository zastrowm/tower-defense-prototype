﻿using System;
using System.Collections.Generic;
using System.Linq;
using NineByteGames.TowerDefense.Signals;
using NineByteGames.TowerDefense.Utils;
using UnityEngine;

namespace NineByteGames.TowerDefense.Behaviors
{
  public class PlayerInput : AttachedBehavior, IUpdatable
  {
    /// <summary> The layer on which projectiles should be created. </summary>
    public Layer ProjectileLayer;

    /// <summary> How fast the player moves </summary>
    public float PlayerSpeed = 2.25f;

    /// <summary> How fast the player turns. </summary>
    public int TurnSpeed = 4;

    /// <summary> All of the layers that should be displayed. </summary>
    public LayerMask InfoLayerMask;

    private const KeyCode KeyUp = KeyCode.W;
    private const KeyCode KeyDown = KeyCode.S;
    private const KeyCode KeyLeft = KeyCode.A;
    private const KeyCode KeyRight = KeyCode.D;

    private Collider2D _physics;
    private Transform _transform;

    private RateLimiter _weaponRecharge;

    public GameObject CurrentObject { get; set; }

    public void Update()
    {
      TrackMouse();
      CheckMovement();
      DetectCurrentObject();

      if (Input.GetMouseButton(0) && _weaponRecharge.Trigger())
      {
        GameObject bullet = GameObject.Find("Bullet");
        var behavior = bullet.GetComponent<ProjectileBehavior>();
        behavior.CreateAndInitializeFrom(Owner.transform, ProjectileLayer);
      }
      else if (Input.GetMouseButton(1) && _weaponRecharge.Trigger())
      {
        GameObject bullet = GameObject.Find("HealPill");
        var behavior = bullet.GetComponent<ProjectileBehavior>();
        behavior.CreateAndInitializeFrom(Owner.transform, ProjectileLayer);
      }
    }

    public override void Start()
    {
      base.Start();

      _physics = Owner.GetComponent<Collider2D>();
      _transform = Owner.GetComponent<Transform>();

      _weaponRecharge = new RateLimiter(TimeSpan.FromSeconds(0.5f));
    }

    /// <summary> Check all input related to movement and move the player accordingly. </summary>
    private void CheckMovement()
    {
      var desiredVelocity = new Vector3();

      if (Input.GetKey(KeyUp))
      {
        desiredVelocity.y += 1;
      }

      if (Input.GetKey(KeyDown))
      {
        desiredVelocity.y -= 1;
      }

      if (Input.GetKey(KeyLeft))
      {
        desiredVelocity.x -= 1;
      }

      if (Input.GetKey(KeyRight))
      {
        desiredVelocity.x += 1;
      }

      if (desiredVelocity.sqrMagnitude > 0.001f)
      {
        desiredVelocity = desiredVelocity.normalized;
        _transform.position += desiredVelocity * Time.deltaTime * PlayerSpeed;
      }
    }

    private void TrackMouse()
    {
      Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

      Quaternion desired = Quaternion.Lerp(transform.rotation,
                                           Quaternion.LookRotation(Vector3.forward, mousePos - transform.position),
                                           Time.deltaTime * TurnSpeed);

      transform.rotation = desired;
    }

    private void DetectCurrentObject()
    {
      // Fire a ray from the camera into the world
      Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      position.z -= 1;

      var ray = new Ray(position, new Vector3(0, 0, 1));

      // Test it out to 1000 units. If it hits something, continue.
      RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                           Vector2.zero,
                                           100.0f,
                                           InfoLayerMask.value);

      CurrentObject = hit.collider != null
        ? hit.collider.gameObject
        : null;
    }
  }
}