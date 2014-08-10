using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour
{
    public List<ParticleEffect> ParticleEffects = new List<ParticleEffect>();
    public ParticleEffect ParticleEffectPrefab;
    List<Color> particleColors = new List<Color>(Block.TypeCount);

    // Use this for initialization
    void Start()
    {
        particleColors.Add(new Color(0.73f, 0.0f, 0.73f));
        particleColors.Add(new Color(0.2f, 0.2f, 0.8f));
        particleColors.Add(new Color(0.0f, 0.6f, 0.05f));
        particleColors.Add(new Color(0.85f, 0.85f, 0.0f));
        particleColors.Add(new Color(1.0f, 0.4f, 0.0f));
    }

    public void CreateParticles(int x, int y, int count, int type)
    {
        ParticleEffect effect = Instantiate(ParticleEffectPrefab, Vector3.zero, Quaternion.identity) as ParticleEffect;
        ParticleEffects.Add(effect);
        effect.transform.parent = transform;
        
        effect.X = x;
        effect.Y = y;
        effect.GetComponent<ParticleSystem>().startColor = particleColors[type];
        effect.GetComponent<ParticleSystem>().Emit(count);
    }

    // Update is called once per frame
    void Update()
    {
        List<ParticleEffect> effectsToRemove = new List<ParticleEffect>();

        foreach (ParticleEffect effect in ParticleEffects)
        {
            if (effect.GetComponent<ParticleSystem>().particleCount == 0)
            {
                effectsToRemove.Add(effect);
            }
        }

        foreach (ParticleEffect effect in effectsToRemove)
        {
            ParticleEffects.Remove(effect);
            Destroy(effect.gameObject);
        }
    }
}
