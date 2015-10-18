using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Faction {
	public string factionName = "";
	public Color factionColor = new Color();
	public string colorString = "";

	public List<Country> countries = new List<Country>();

	public Faction () {
		colorString = MeshMaker.ColorToHex (factionColor);
	}

	public void AddCountry(Country newCountry) {
		if (newCountry.faction != null) {
			newCountry.faction.countries.Remove (newCountry);
		}

		newCountry.faction = this;
		countries.Add (newCountry);
		newCountry.ColorTerritories ();
	}

	public void ColorCountries(){
		foreach (Country country in countries) {
			country.ColorTerritories ();
		}
	}

}
