using OrangeNBT.NBT;
using System;
using System.Collections.Generic;

namespace OrangeNBT.Data.AnvilImproved
{
	public class AnvilImprovedBlock : IBlock
	{
		public int Luminance => 0;

		public int Opacity => 0;

		public bool HasTileEntity => false;

		public int Id => _defaultState.SystemId;

		private readonly string _name;
		public string Name => _name;

		public BlockSet DefaultBlockSet => new BlockSet(this, _defaultState.Properties, _defaultState.SystemId);

		private List<State> _states = new List<State>();
		private State _defaultState;

		public AnvilImprovedBlock(string name)
		{
			_name = name;
		}

		public void SetState(int id, bool @default, IDictionary<string, string> properties)
		{
			State state = new State() { SystemId = id, Default = @default, Properties = properties };
			if (@default)
				_defaultState = state;
			_states.Add(state);
		}

		public TagCompound BuildTileEntity()
		{
			throw new NotSupportedException();
		}

		public BlockSet GetBlock(IDictionary<string, string> properties)
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (BlockSet.EqualsProperties(_states[i].Properties, properties))
				{
					return new BlockSet(this, properties, _states[i].SystemId);
				}
			}
			return DefaultBlockSet;
		}

		public string GetName(int metadata)
		{
			return _name;
		}

		public IDictionary<string, string> GetProperties(int metadata)
		{
			if (_states == null || _states.Count == 0)
				return DefaultBlockSet.Properties;

			int index = _states.FindIndex(e => e.SystemId == metadata);
			if (index >= 0)
				return _states[index].Properties;
			return DefaultBlockSet.Properties;
		}

		public int GetRuntimeId(IDictionary<string, string> properties)
		{
			for (int i = 0; i < _states.Count; i++)
			{
				if (BlockSet.EqualsProperties(_states[i].Properties, properties))
				{
					return _states[i].SystemId;
				}
			}
			return Id;
		}

		private class State
		{
			public bool Default { get; set; }
			public IDictionary<string, string> Properties { get; set; }	
			public int SystemId { get; set; }
		}
	}
}
