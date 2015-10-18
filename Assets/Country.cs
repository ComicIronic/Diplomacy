using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country {

	public Faction faction;

	public string countryName = "";

	public bool locked = false;

	public bool centre = false;

	public List<CountryObject> territories = new List<CountryObject>();

	public Country () {
		Camera.main.GetComponent<CanvasCreator> ().factions [0].AddCountry (this);
	}

	public void ColorTerritories() {
		foreach (CountryObject territory in territories) {
			territory.ColorToFaction ();
		}
	}

	public void AddTerritory(CountryObject territory) {
		territories.Add (territory);
		territory.parentCountry = this;
		territory.ColorToFaction ();
	}

	public void RemoveTerritory(CountryObject territory) {
		territories.Remove (territory);
		territory.parentCountry = new Country ();
		territory.ColorToFaction ();
	}

	void MergeInto(Country other) {
		foreach (CountryObject territory in territories) {
			other.AddTerritory (territory);
		}

		faction = null;
		territories = null;
	}
}
