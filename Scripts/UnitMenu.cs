using System;
using Godot;

public class UnitMenu : Sprite3D {

	Control RadialMenu;
	Viewport Viewport;

	public override void _Ready() {

		Viewport = GetNode<Viewport>("Viewport");
		RadialMenu = GetNode<Control>("Viewport/RadialMenu");

		Texture = Viewport.GetTexture();

		//RadialMenu.Show();


	}

	public void Show() {

		RadialMenu.Show();
	}

	public void Hide() {

		RadialMenu.Hide();
	}

	public void CursorInteract() {

		RadialMenu.Show();

	}
}
