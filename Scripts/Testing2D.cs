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
	Vector2 lineIntersectionPoint;

	MyLine secondLine;
 
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
        if (Primitives2DTests.LineCircle(myLine, myCircle) || Primitives2DTests.LineOrientedRecrangle(myLine, orientedRectangle) 
			|| Primitives2DTests.LineLine(myLine, secondLine, ref lineIntersectionPoint))
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
		DrawLine(secondLine.start, secondLine.end, Colors.Green);

        //Draw oriented rect
        var t = new Transform2D();
        t.x.x = t.y.y = Mathf.Cos(orientedRectangle.rotation);
        t.x.y = t.y.x = Mathf.Sin(orientedRectangle.rotation);
        t.y.x *= -1;

		Vector2 tempPos = orientedRectangle.position;
		//translate first to origin
		orientedRectangle.position = Vector2.Zero;
		Vector2 topLeft = orientedRectangle.position - orientedRectangle.halfExtents;
        Vector2 bottomRight = orientedRectangle.position + orientedRectangle.halfExtents;
		Vector2 topRight = new Vector2(bottomRight.x, topLeft.y);
		Vector2 bottomLeft = new Vector2(topLeft.x, bottomRight.y);
		//rotate
		topLeft *= t;
		bottomLeft *= t;
		topRight *= t;
		bottomRight *= t;
		//translate back
		orientedRectangle.position = tempPos;
		topLeft += orientedRectangle.position;
		bottomRight += orientedRectangle.position;
		bottomLeft += orientedRectangle.position;
		topRight += orientedRectangle.position;
  


        DrawLine(topLeft, topRight, Colors.Green);
        DrawLine(topLeft, bottomLeft, Colors.Green);
        DrawLine(bottomLeft, bottomRight, Colors.Green);
        DrawLine(bottomRight, topRight, Colors.Green);


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
		orientedRectangle.rotation = rectangle.Rotation;
    }

	private void InitLine()
	{
		myLine = new MyLine(Vector2.Zero, Vector2.Zero);
		secondLine = new MyLine(new Vector2(100f, 200f), new Vector2(150f, 300f));
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
