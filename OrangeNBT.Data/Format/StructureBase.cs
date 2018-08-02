using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OrangeNBT.NBT;

namespace OrangeNBT.Data.Format
{
    public abstract class StructureBase
    {
        protected readonly int _width;
        protected readonly int _height;
        protected readonly int _length;

        public int Width => _width;
        public int Height => _height;
        public int Length => _length;

        public StructureBase(int width, int height, int length)
        {
            _width = width;
            _height = height;
            _length = length;
        }

        public StructureBase(Cuboid cuboid)
            : this(cuboid.Width, cuboid.Height, cuboid.Length)
        { }

		public class EntityCollection : IEntityCollection, ITagProvider<TagList>
		{
			private TagList _entities;

			public EntityCollection(TagList tagList)
			{
				_entities = tagList;
			}

			public EntityCollection()
			{
				_entities = new TagList("Entities");
			}

			public void Add(TagCompound tag)
			{
				_entities.Add(tag);
			}

			public void Add(TagCompound tag, bool safe)
			{
				if ((safe && Entity.IsEntityTag(tag)) || !safe)
					Add(tag);
			}

			public TagList BuildTag()
			{
				return _entities;
			}

			public IEnumerator<TagCompound> GetEnumerator()
			{
				foreach(TagBase t in _entities)
				{
					TagCompound c = t as TagCompound;
					if (c != null)
						yield return t as TagCompound;
				}
			}

			public IEnumerable<TagCompound> GetWithin(Cuboid area)
			{
				foreach (TagCompound e in _entities)
				{
					Position pos = Entity.GetPosition(e);
					if (area.Contains(pos.X, pos.Y, pos.Z))
					{
						yield return e;
					}
				}
			}

			public void Remove(TagCompound tag)
			{
				_entities.Remove(tag);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _entities.GetEnumerator();
			}
		}

	}
}
