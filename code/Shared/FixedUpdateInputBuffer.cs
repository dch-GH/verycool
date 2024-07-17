namespace Sandbox;

public struct FixedUpdateInputBuffer
{
	private class State
	{
		public bool Held;
		public bool Pressed;
	}

	private Dictionary<InputAction, State> _bindStates;

	public FixedUpdateInputBuffer()
	{
		_bindStates = new Dictionary<InputAction, State>();

		foreach ( var b in Input.GetActions() )
		{
			_bindStates[b] = new State();
		}
	}

	public void Update()
	{
		foreach ( var binding in _bindStates.Keys )
		{
			if ( Input.Down( binding.Name ) )
				_bindStates[binding].Held = true;

			if ( Input.Pressed( binding.Name ) )
				_bindStates[binding].Pressed = true;
		}
	}

	public bool FixedHeld( InputAction bind )
	{
		return _bindStates[bind].Held;
	}

	public bool FixedPressed( InputAction bind )
	{
		return _bindStates[bind].Pressed;
	}

	public void FixedClear()
	{
		foreach ( var b in _bindStates.Keys )
		{
			_bindStates[b].Held = false;
			_bindStates[b].Pressed = false;
		}
	}
}
