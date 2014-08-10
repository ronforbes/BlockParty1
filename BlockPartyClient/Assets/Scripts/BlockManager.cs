using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    public Block BlockPrefab;
    public List<Block> Blocks = new List<Block>(BlockCapacity);
    public List<int> LastRowCreepTypes = new List<int>(Grid.PlayWidth);
    public List<int> SecondToLastRowCreepTypes = new List<int>(Grid.PlayWidth);

    public const int BlockCapacity = Grid.GridSize;

    int lastCreepType, secondToLastCreepType;

    public void StartRound()
    {
        Blocks = new List<Block>(BlockCapacity);

        lastCreepType = secondToLastCreepType = 0;

        LastRowCreepTypes = new List<int>(Grid.PlayWidth);
        SecondToLastRowCreepTypes = new List<int>(Grid.PlayWidth);

        for (int x = 0; x < Grid.PlayWidth; x++)
        {
            LastRowCreepTypes.Add(0);
            SecondToLastRowCreepTypes.Add(0);
        }
    }

    public void CreateIdleBlock(int x, int y, int type)
    {
        if (Blocks.Count == BlockCapacity)
            return;

        Block block = InstantiateBlock();

        block.InitializeIdle(x, y, type);
    }

    Block InstantiateBlock()
    {
        Block block = Instantiate(BlockPrefab, Vector3.zero, Quaternion.identity) as Block;
        block.transform.parent = transform;
        Blocks.Add(block);

        return block;
    }

    public void CreateCreepRow()
    {
        for (int x = 0; x < Grid.PlayWidth; x++)
        {
            CreateCreepBlock(x);
        }
    }

    public void CreateCreepBlock(int x)
    {
        int type = 0;

        if (LastRowCreepTypes.Count == 0)
            LastRowCreepTypes = new List<int>(Grid.PlayWidth);
        if (SecondToLastRowCreepTypes.Count == 0)
            SecondToLastRowCreepTypes = new List<int>(Grid.PlayWidth);

        do
        {
            type = Random.Range(0, Block.TypeCount);
        } while((type == lastCreepType && lastCreepType == secondToLastCreepType) ||
          (type == LastRowCreepTypes[x] && LastRowCreepTypes[x] == SecondToLastRowCreepTypes[x]));

        SecondToLastRowCreepTypes[x] = LastRowCreepTypes[x];
        LastRowCreepTypes[x] = type;

        secondToLastCreepType = lastCreepType;
        lastCreepType = type;

        CreateIdleBlock(x, 0, type);
    }

    public void DeleteBlock(Block block)
    {
        Blocks.Remove(block);
        Destroy(block.gameObject);
    }

    public void ShiftUp()
    {
        foreach (Block block in Blocks)
        {
            block.Y++;
        }
    }

    public bool Match(Block block1, Block block2)
    {
        return block1.Type == block2.Type;
    }
}
