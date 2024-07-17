namespace Rose;

public sealed class GameController : Component
{
	[Property] private GameObject _spawnPoint;
	[Property] private PrefabFile _playerPrefab;

	protected override void OnStart()
	{
		var player = GameObject.Clone( _playerPrefab );
		player.Transform.Position = _spawnPoint.Transform.Position;
	}
}
