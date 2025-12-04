using System;
using System.IO;
using System.Text.Json;

namespace TilesetEditor.IO
{
    public class MapStorage
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public int[] CellsIndices { get; set; } = Array.Empty<int>();
        public bool[] CellsFlipX { get; set; } = Array.Empty<bool>();
        public byte[] CellsRotation { get; set; } = Array.Empty<byte>();

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public MapStorage() { }

        public MapStorage(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            int size = columns * rows;
            CellsIndices = new int[size];
            CellsFlipX = new bool[size];
            CellsRotation = new byte[size];
            Array.Fill(CellsIndices, -1);
        }

        public void Save(string path)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, opts);
            File.WriteAllText(path, json);
        }

        public static MapStorage Load(string path)
        {
            var json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<MapStorage>(json) ?? throw new Exception("Failed to load map");
            int expected = loaded.Columns * loaded.Rows;
            loaded.CellsIndices = EnsureArray(loaded.CellsIndices, expected, -1);
            loaded.CellsFlipX = EnsureArray(loaded.CellsFlipX, expected, false);
            loaded.CellsRotation = EnsureArray(loaded.CellsRotation, expected, (byte)0);
            return loaded;
        }

        private static T[] EnsureArray<T>(T[]? array, int expectedLength, T defaultValue)
        {
            if (array == null || array.Length != expectedLength)
            {
                var newArray = new T[expectedLength];
                if (array != null) Array.Copy(array, newArray, Math.Min(array.Length, expectedLength));
                for (int i = array?.Length ?? 0; i < expectedLength; i++) newArray[i] = defaultValue;
                return newArray;
            }
            return array;
        }
    }
}
