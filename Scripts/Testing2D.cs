using Godot;
using System;
using System.Diagnostics;
using static Primitives2D;

public class Testing2D : Node2D
{
	Position2D point;
	Sprite rectangle;

	OrientedRectangle orientedRectangle;
	MyLine myLine;
	MyCircle myCircle;
	Color lineHitColor = Colors.Green;

	float moveSpeed = 150f;
	Vector2 moveDir;

	Vector2[] orientedAABBVertices;
 
	public override void _Ready()
	{
		point = GetNode<Position2D>("Point");
		rectangle = GetNode<Sprite>("OrientedRect");

		
		InitOrientedRectangle();
		InitLine();
		InitCircle();

		


	}

	public override void _Process(float delta)
	{
		UpdateControls(delta);
		UpdateLine();

		Update();
	}

    public override void _PhysicsProcess(float delta)
    {
        if (Primitives2DTests.LineCircle(myLine, myCircle) || Primitives2DTests.LineOrientedRecrangle(myLine, orientedRectangle))
        {
            lineHitColor = Colors.Red;
        }
        else
        {
            lineHitColor = Colors.Green;
        }
    }

    bool isPressed = false;
	public override void _Input(InputEvent @event)
	{

		isPressed = false;
		if (Input.IsKeyPressed((int)KeyList.A))
		{
			moveDir = Vector2.Left;
			isPressed = true;
		}
		if (Input.IsKeyPressed((int)KeyList.D))
		{
			moveDir = Vector2.Right;
			isPressed = true;
		}

		if (Input.IsKeyPressed((int)KeyList.W))
		{
			moveDir = Vector2.Up;
			isPressed = true;
		}

		if (Input.IsKeyPressed((int)KeyList.S))
		{
			moveDir = Vector2.Down;
			isPressed = true;
		}



	}



	public override void _Draw()
	{
		DrawCircle(myCircle.position, myCircle.radius, Colors.Green);
		DrawLine(myLine.start, myLine.end, lineHitColor);

    }

    public void UpdateControls(float dt)
	{
		if(isPressed)
		point.GlobalPosition += moveDir * moveSpeed * dt;
	}

	private void InitOrientedRectangle()
	{
		orientedRectangle = new OrientedRectangle();
		orientedRectangle.position = rectangle.Position;
		orientedRectangle.halfExtents = new Vector2(rectangle.Texture.GetWidth() / 2,
										rectangle.Texture.GetHeight() / 2);
		orientedRectangle.rotation = rectangle.RotationDegrees;
    }

	private void InitLine()
	{
		myLine = new MyLine(Vector2.Zero, Vector2.Zero);
	}
	private void UpdateLine()
	{

		myLine.start = point.GlobalPosition;
		myLine.end = GetGlobalMousePosition();

		
	}
	private void InitCircle()
	{
		myCircle = new MyCircle(GetNode<Position2D>("Circle").GlobalPosition, 50f);
	}


	

}
