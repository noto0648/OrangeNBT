﻿using System;
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

        public void Save()
        {
            foreach (AnvilDimension d in _dimensions.Values)
            {
                d.Save();
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
            for(int i = 0; i < directory.Length;i++)
            {
                string dir = Path.GetFileName(directories[i]);
                if(dir.StartsWith("DIM"))
                {
                    int no;
                    string id = dir.Replace("DIM", "");
                    if(int.TryParse(id, out no))
                    {
                        world._dimensions.Add(no, new AnvilDimension(directories[i]));
                    }
                }
            }
            return world;
        }


    }
}