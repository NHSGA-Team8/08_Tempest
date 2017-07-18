using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An interface for all ships, friendly and enemy.
	public interface IShipBase {

	// Called to fire a projectile.
	void Fire();

	// Called when a projectile damages the ship. Should call OnDeath() if it kills;
	void TakeDamage(int dmg);

	// Called when the ship dies. Add points, do game state detection, etc.
	void OnDeath();
}
