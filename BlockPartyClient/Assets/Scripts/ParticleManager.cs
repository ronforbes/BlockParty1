﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {
    public List<ParticleEffect> ParticleEffects = new List<ParticleEffect>();
    public ParticleEffect ParticleEffectPrefab;

	// Use this for initialization
	void Start () {
	    
	}
	
    public void CreateParticles(int x, int y, int count)
    {
        ParticleEffect effect = Instantiate(ParticleEffectPrefab, Vector3.zero, Quaternion.identity) as ParticleEffect;
        ParticleEffects.Add(effect);
        effect.transform.parent = transform;
        
        effect.X = x;
        effect.Y = y;
        effect.GetComponent<ParticleSystem>().Emit(count);
    }

	// Update is called once per frame
	void Update () {
        List<ParticleEffect> effectsToRemove = new List<ParticleEffect>();

        foreach(ParticleEffect effect in ParticleEffects)
        {
            if(effect.GetComponent<ParticleSystem>().particleCount == 0)
            {
                effectsToRemove.Add(effect);
            }
        }

        foreach(ParticleEffect effect in effectsToRemove)
        {
            ParticleEffects.Remove(effect);
            Destroy(effect.gameObject);
        }
	}
}