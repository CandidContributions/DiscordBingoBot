using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using DiscordBingoBot.Extenstions;

namespace DiscordBingoBot.Models
{
    public class Grid
    {
        private static int size = 5;

        public Row[] Rows { get; } = Row.Create(size);
        public Guid GridId { get; }
        private ControlRow[] _controlRows = ControlRow.Create(size);

        public Grid(Guid gridId)
        {
            GridId = gridId;
        }

        public class Row
        {
            public string[] Items { get; } = new string[size];

            public static Row[] Create(int size)
            {
                var retval = new Row[size];
                for (int i = 0; i < size; i++)
                {
                    retval[i] = new Row();
                }

                return retval;
            }
        }

        public class ControlRow
        {
            public bool[] Items = new bool[size];
            public static ControlRow[] Create(int size)
            {
                var retval = new ControlRow[size];
                for (int i = 0; i < size; i++)
                {
                    retval[i] = new ControlRow();
                }

                return retval;
            }
        }

        public bool Populate(IEnumerable<string> items)
        {
            var random = RandomFactory.FromGuid(GridId);
            var shuffledList = items.ToList();
            shuffledList.Shuffle(random);

            if (items.Count() < 25)
            {
                return false;
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Rows[i].Items[j] = shuffledList[i * 5 + j];
                }
            }

            return true;
        }

        public void Mark(string item)
        {
            var isMatch = false;
            for (int i = 0; i < 5; i++)
            {
                if(isMatch) break;
                for (int j = 0; j < 5; j++)
                {
                    if (Rows[i].Items[j] == item)
                    {
                        _controlRows[i].Items[j] = true;
                        isMatch = true;
                        break;
                    }
                }
            }
        }

        public bool IsMarked(int row, int column)
        {
            return _controlRows[row].Items[column];
        }
    }
}
