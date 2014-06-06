using UnityEngine;
using System.Collections;

public class Slider : MonoBehaviour 
{
	public enum SlideDirection
	{
		None,
		Left,
		Right
	}
	
	public float SlideElapsed;
	public float SlideDuration = 0.1f;
	public Grid Grid;
	
	bool sliding;
	Block selectedBlock;
	Block leftBlock;
	Block rightBlock;
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				if(hit.collider.name == "Cube")
				{
					Block block = hit.collider.gameObject.transform.parent.gameObject.GetComponent<Block>();
					if(Grid.StateAt(block.X, block.Y) == GridElement.ElementState.Block &&
					   block.Y > 0)
					{
						selectedBlock = block;
					}
				}
			}
		}
		
		if(selectedBlock != null && Grid.StateAt(selectedBlock.X, selectedBlock.Y) == GridElement.ElementState.Block)
		{
			float leftEdge = selectedBlock.transform.position.x - selectedBlock.transform.localScale.x / 2;
			float rightEdge = selectedBlock.transform.position.x + selectedBlock.transform.localScale.x / 2;
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = Camera.main.transform.position.z * -1;

			bool slideDisallowed = false;

			if(Camera.main.ScreenToWorldPoint(mousePosition).x < leftEdge)
			{
				if(selectedBlock.X - 1 >= 0)
				{
					if(Grid.StateAt(selectedBlock.X, selectedBlock.Y) == GridElement.ElementState.Block)
					{
						if(Grid.BlockAt(selectedBlock.X, selectedBlock.Y) != null)
						{
							rightBlock = Grid.BlockAt(selectedBlock.X, selectedBlock.Y);
						}
					}
					else
					{
						if(Grid.StateAt(selectedBlock.X, selectedBlock.Y) != GridElement.ElementState.Empty ||
						   Grid.StateAt(selectedBlock.X, selectedBlock.Y - 1) == GridElement.ElementState.Falling)
						{
							// TODO: Once you implement hanging state, check for that too
							slideDisallowed = true;
						}
					}

					if(Grid.StateAt(selectedBlock.X - 1, selectedBlock.Y) == GridElement.ElementState.Block)
					{
						if(Grid.BlockAt(selectedBlock.X - 1, selectedBlock.Y) != null)
						{
							leftBlock = Grid.BlockAt(selectedBlock.X - 1, selectedBlock.Y);
						}
					}
					else
					{
						if(Grid.StateAt(selectedBlock.X - 1, selectedBlock.Y) != GridElement.ElementState.Empty ||
						   Grid.StateAt(selectedBlock.X - 1, selectedBlock.Y - 1) == GridElement.ElementState.Falling)
						{
							// TODO: Once you implement hanging state, check for that too
							slideDisallowed = true;
						}
					}
				}
			}
			
			if(Camera.main.ScreenToWorldPoint(mousePosition).x > rightEdge)
			{
				if(selectedBlock.X + 1 < Grid.PlayWidth)
				{
					if(Grid.StateAt(selectedBlock.X, selectedBlock.Y) == GridElement.ElementState.Block)
					{
						if(Grid.BlockAt(selectedBlock.X, selectedBlock.Y) != null)
						{
							leftBlock = Grid.BlockAt(selectedBlock.X, selectedBlock.Y);
						}
					}
					else
					{
						if(Grid.StateAt(selectedBlock.X, selectedBlock.Y) != GridElement.ElementState.Empty ||
						   Grid.StateAt(selectedBlock.X, selectedBlock.Y - 1) == GridElement.ElementState.Falling)
						{
							// TODO: Once you implement hanging state, check for that too
							slideDisallowed = true;
						}
					}

					if(Grid.StateAt(selectedBlock.X + 1, selectedBlock.Y) == GridElement.ElementState.Block)
					{
						if(Grid.BlockAt(selectedBlock.X + 1, selectedBlock.Y) != null)
						{
							rightBlock = Grid.BlockAt(selectedBlock.X + 1, selectedBlock.Y);
						}
					}
					else
					{
						if(Grid.StateAt(selectedBlock.X + 1, selectedBlock.Y) != GridElement.ElementState.Empty ||
						   Grid.StateAt(selectedBlock.X + 1, selectedBlock.Y - 1) == GridElement.ElementState.Falling)
						{
							// TODO: Once you implement hanging state, check for that too
							slideDisallowed = true;
						}
					}
				}
			}

			if(!slideDisallowed)
			{
				if(leftBlock)
					leftBlock.StartSliding(SlideDirection.Right, leftBlock == Grid.BlockAt(selectedBlock.X, selectedBlock.Y));
				if(rightBlock)
					rightBlock.StartSliding(SlideDirection.Left, rightBlock == Grid.BlockAt(selectedBlock.X, selectedBlock.Y));
				
				if(leftBlock || rightBlock)
				{
					sliding = true;
					SlideElapsed = 0.0f;
				}
			}
			else
			{
				leftBlock = null;
				rightBlock = null;
			}
		}
		
		if(Input.GetMouseButtonUp(0))
		{
			selectedBlock = null;
		}

		if(sliding)
		{
			SlideElapsed += Time.deltaTime;

			if(SlideElapsed >= SlideDuration)
			{
				sliding = false;

				if(leftBlock)
					Grid.Remove(leftBlock.X, leftBlock.Y, leftBlock);
				if(rightBlock)
					Grid.Remove(rightBlock.X, rightBlock.Y, rightBlock);

				if(leftBlock)
					leftBlock.FinishSliding(leftBlock.X + 1);
				if(rightBlock)
					rightBlock.FinishSliding(rightBlock.X - 1);

				if(leftBlock)
				{
					Grid.RequestMatchCheck(leftBlock);
				}
				if(rightBlock)
				{
					Grid.RequestMatchCheck(rightBlock);
				}

				leftBlock = null;
				rightBlock = null;
			}
		}
	}
}
