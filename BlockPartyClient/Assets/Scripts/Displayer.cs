using UnityEngine;
using System.Collections;

public class Displayer : MonoBehaviour
{
    public BlockManager BlockManager;
    public Slider Slider;
    public Creep Creep;
    public SignManager SignManager;
    public ParticleManager ParticleManager;
    Color[] blockColors = new Color[Block.TypeCount];
    Color[] creepColors = new Color[Block.TypeCount];
    Color flashColor = new Color(1.0f, 1.0f, 1.0f);
    float playOffsetY;
    const float gridElementLength = 1.0f;
    const float blockDyingFlashDuration = 0.2f;
    const float blockDyingSpeed = 1000;

    void Start()
    {
        blockColors[0] = new Color(0.73f, 0.0f, 0.73f);
        blockColors[1] = new Color(0.2f, 0.2f, 0.8f);
        blockColors[2] = new Color(0.0f, 0.6f, 0.05f);
        blockColors[3] = new Color(0.85f, 0.85f, 0.0f);
        blockColors[4] = new Color(1.0f, 0.4f, 0.0f);

        creepColors[0] = new Color(0.25f * 0.73f, 0.25f * 0.0f, 0.25f * 0.73f);
        creepColors[1] = new Color(0.25f * 0.2f, 0.25f * 0.2f, 0.25f * 0.8f);
        creepColors[2] = new Color(0.25f * 0.0f, 0.25f * 0.6f, 0.25f * 0.05f);
        creepColors[3] = new Color(0.25f * 0.85f, 0.25f * 0.85f, 0.25f * 0.0f);
        creepColors[4] = new Color(0.25f * 1.0f, 0.25f * 0.4f, 0.25f * 0.0f);
    }

    public void CalculatePlayOffsetY()
    {
        playOffsetY = gridElementLength * Creep.CreepElapsed / Creep.CreepDuration;
    }

    void Update()
    {
        CalculatePlayOffsetY();

        DrawBlocks();
        DrawSigns();
        DrawParticles();
    }

    void DrawBlocks()
    {
        foreach (Block block in BlockManager.Blocks)
        {
            if (block.Y > Grid.SafeHeight)
                continue;

            DrawBlock(block);
        }
    }

    void DrawBlock(Block block)
    {
        float x, y;
        float dX = 0.0f;

        x = block.X * gridElementLength;
        y = block.Y * gridElementLength + playOffsetY;

        switch (block.State)
        {
            case Block.BlockState.Idle:
                if (block.Y != 0)
                    block.transform.Find("Cube").renderer.material.color = blockColors[block.Type];
                else
                    block.transform.Find("Cube").renderer.material.color = creepColors[block.Type];
			
                block.transform.position = new Vector3(x, y, 0.0f);
                block.transform.rotation = Quaternion.identity;
                break;

            case Block.BlockState.Sliding:
                if (block.Direction == Slider.SlideDirection.Left)
                {
                    dX = -gridElementLength / 2.0f;
                }

                if (block.Direction == Slider.SlideDirection.Right)
                {
                    dX = gridElementLength / 2.0f;
                }

                block.transform.Find("Cube").renderer.material.color = blockColors[block.Type];

                Vector3 center = new Vector3(x + dX, y, 0.0f);
                if (block.SlideFront)
                {
                    center -= new Vector3(0, 0, -0.01f);
                }
                else
                {
                    center -= new Vector3(0, 0, 0.01f);
                }

                Vector3 fromRelCenter = new Vector3(x, y, 0.0f) - center;
                Vector3 toRelCenter = new Vector3(x + dX * 2.0f, y, 0.0f) - center;
                float time = Slider.SlideElapsed / Slider.SlideDuration;
                block.transform.position = Vector3.Slerp(fromRelCenter, toRelCenter, time);
                block.transform.position += center;
                break;

            case Block.BlockState.Falling:
                y += gridElementLength - gridElementLength * block.FallElapsed / Block.FallDuration;

                block.transform.position = new Vector3(x, y, 0.0f);
                break;

            case Block.BlockState.Dying:
			// when dying, first we flash
                if (block.DieElapsed < blockDyingFlashDuration)
                {
                    float flash = block.DieElapsed * 4.0f / blockDyingFlashDuration;
                    if (flash > 2.0f)
                        flash = 4.0f - flash;
                    if (flash > 1.0f)
                        flash = 2.0f - flash;

                    block.transform.Find("Cube").renderer.material.color = new Color(
                        blockColors[block.Type].r + flash * (flashColor.r - blockColors[block.Type].r),
                        blockColors[block.Type].g + flash * (flashColor.g - blockColors[block.Type].g),
                        blockColors[block.Type].b + flash * (flashColor.b - blockColors[block.Type].b));
                }
                else
                {
                    block.transform.Find("Cube").renderer.material.color = blockColors[block.Type];

                    block.transform.Find("Cube").transform.Rotate(new Vector3(block.DyingAxis.x, block.DyingAxis.y, 0.0f), block.DieElapsed * block.DieElapsed * Time.deltaTime * blockDyingSpeed);

                    float scale = 1.0f - block.DieElapsed / Block.DieDuration;

                    block.transform.localScale = new Vector3(scale, scale, scale);
                }
                break;
        }
    }

    void DrawSigns()
    {
        foreach (Sign sign in SignManager.Signs)
        {
            DrawSign(sign);
        }
    }

    void DrawSign(Sign sign)
    {
        float x, y;
        float alpha;

        x = sign.X * gridElementLength;
        y = sign.Y * gridElementLength;
        alpha = 1.0f - sign.Elapsed / Sign.Duration;

        sign.transform.position = new Vector3(x, y + sign.Elapsed / Sign.Duration, 0.0f);
        sign.transform.localScale = new Vector3(1.0f + sign.Elapsed / Sign.Duration, 1.0f + sign.Elapsed / Sign.Duration, 1.0f + sign.Elapsed / Sign.Duration);
        TextMesh mesh = sign.transform.Find("Text").GetComponent<TextMesh>() as TextMesh;
        mesh.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }

    void DrawParticles()
    {
        foreach (ParticleEffect effect in ParticleManager.ParticleEffects)
        {
            DrawParticleEffect(effect);
        }
    }

    void DrawParticleEffect(ParticleEffect effect)
    {
        float x, y;
        x = effect.X * gridElementLength;
        y = (effect.Y + 0.5f) * gridElementLength;

        effect.transform.position = new Vector3(x, y, -0.5f);
        effect.transform.rotation = Quaternion.AngleAxis(-90.0f, new Vector3(1.0f, 0.0f, 0.0f));
    }
}
