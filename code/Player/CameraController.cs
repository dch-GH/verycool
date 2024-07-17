namespace Rose;

public sealed class CameraController : Component
{
	[Property] private CameraComponent _camera { get; set; }
	[Property] private Vector3 _offset { get; set; }

	protected override void OnAwake()
	{
		_camera = Scene.GetAllComponents<CameraComponent>().First();
	}

	protected override void OnPreRender()
	{
		_camera.Transform.Position = Transform.Position + _offset;
	}
}
