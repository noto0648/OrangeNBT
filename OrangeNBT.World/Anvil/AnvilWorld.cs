using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace OrangeNBT.World.Anvil
{
    public class AnvilWorld : IDisposable
    {
        private string _baseDirectory;

        private Dictionary<int, AnvilDimension> _dimensions;
        public Dictionary<int, AnvilDimension> Dimensions => _dimensions;

        private AnvilWorld(string directory)
        {
            _dimensions = new Dictionary<int, AnvilDimension>();
            _dimensions.Add(Dimension.Overworld, new AnvilDimension(directory));
            _baseDirectory = directory;
        }

        public Dimension AddDimension(int id)
        {
            if (_dimensions.ContainsKey(id))
                return _dimensions[id];
            AnvilDimension dim = new AnvilDimension(_baseDirectory + Path.DirectorySeparatorChar + "DIM" + id);
            _dimensions.Add(id, dim);
            return dim;
        }

		public void Save(int version = 1549)
		{
			foreach (AnvilDimension d in _dimensions.Values)
			{
				d.Save(version);
			}
		}

        public void Dispose()
        {
            foreach(IDisposable d in _dimensions.Values)
            {
                d.Dispose();
            }
        }

        public static AnvilWorld Create(string directory)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return new AnvilWorld(directory);
        }

        public static AnvilWorld Load(string directory)
        {
            AnvilWorld world = new AnvilWorld(directory);
            string[] directories = Directory.GetDirectories(directory);
            for(int i = 0; i < directories.Length;i++)
            {
                string dir = Path.GetFileName(directories[i]);
                if(dir.StartsWith("DIM"))
                {
                    string id = dir.Replace("DIM", "");
                    if(int.TryParse(id, out int no))
                    {
                        world._dimensions.Add(no, new AnvilDimension(directories[i]));
                    }
                }
            }
            return world;
        }


    }
}
