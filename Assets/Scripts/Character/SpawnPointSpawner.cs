﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointSpawner : MonoBehaviour
{
	public Transform[] spawnTargets;
	public Transform[] spawnPoints;
	public Transform targetInstance;
	public OrbitCamera followCam;
	public bool spawnNewPeople;
	public float spawnTimer = 60;
	public int spawnCount = 1;

	[HideInInspector]
	public bool useHeroPreset;
	public AppearancePreset heroPreset;
	[HideInInspector]
	public bool othersWithHero;

	List<GameObject> activePeople;
	List<float> spawnTimers;
	float nextSpawnTime;
	int defaultLayer;

	void Awake ()
	{
		activePeople = new List<GameObject> ();
		spawnTimers = new List<float> ();
		spawnPoints = GetComponentsInChildren<Transform> ( false );
		defaultLayer = LayerMask.NameToLayer ( "Default" );
		SpawnPerson ();
		nextSpawnTime = Time.time + spawnTimer;
		if ( !useHeroPreset || othersWithHero )
		{
			for ( int i = 0; i < spawnCount - 1; i++ )
			{
				SpawnPerson ( false );
			}
		}
	}

	void Update ()
	{
		if ( spawnNewPeople )
		{
			if ( Time.time > nextSpawnTime )
			{
				SpawnPerson ();
				nextSpawnTime = Time.time + spawnTimer;
			}
			for ( int i = spawnTimers.Count - 1; i > 0; i-- )
			{
				if ( Time.time > spawnTimers[i] )
				{
					SpawnPerson ( false, i );
				}
			}
		}
	}

	void SpawnPerson (bool isTarget = true, int activeIndex = -1)
	{
		Transform target = useHeroPreset ? ( spawnTargets [ heroPreset.female ? 1 : 0 ] ) : spawnTargets [ Random.Range ( 0, spawnTargets.Length ) ];
		Transform spawn = GetRandomPoint ();
		if ( isTarget )
		{
			if ( targetInstance != null )
				Destroy ( targetInstance.gameObject );
			targetInstance = Instantiate ( target );
			targetInstance.position = spawn.position;
			if ( useHeroPreset )
				targetInstance.GetComponent<CharacterCustomization> ().SetAppearance ( heroPreset );
			targetInstance.gameObject.SetActive ( true );
			followCam.target = targetInstance;
			targetInstance.name = "Hero";
			
		} else
		{
			
			Transform person = Instantiate ( target );
			person.position = spawn.position;
//			person.GetComponent<CharacterCustomization> ().SetAppearance ( presets [ 0 ] );
			person.gameObject.SetActive ( true );
			if ( activeIndex != -1 )
			{
				Destroy ( activePeople [ activeIndex ] );
				activePeople [ activeIndex ] = person.gameObject;
				spawnTimers [ activeIndex ] = Time.time + Random.Range ( 55f, 150f );
				
			} else
			{
				activePeople.Add ( person.gameObject );
				spawnTimers.Add ( Time.time + Random.Range ( 55f, 150f ) );
			}
			SetLayerRecursively ( person, defaultLayer );
		}
	}

	Transform GetRandomPoint ()
	{
		return spawnPoints [ Random.Range ( 0, spawnPoints.Length ) ];
	}

	void SetLayerRecursively (Transform t, int layer)
	{
		t.gameObject.layer = layer;
		if ( t.childCount > 0 )
		{
			foreach ( Transform c in t )
			{
				SetLayerRecursively ( c, layer );
			}
		}
	}

	public void UseHero (bool hero, bool others)
	{
		useHeroPreset = hero;
		othersWithHero = others;
	}
}