using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Kuhn.Yapss {
    public class Layout {
        public int Width {
            get { return width; }
        }
        public int Height {
            get { return height; }
        }
        public Layout(int width, int height) {
            this.width = width;
            this.height = height;
            this.cells = new Cell[width, height];
        }
        public Layout(Layout layout) {
            this.width = layout.width;
            this.height = layout.height;
            this.cells = (Cell[,])layout.cells.Clone();
        }
        public Cell Get(int x, int y) {
            return cells[x, y];
        }
        public void Set(int x, int y, Cell cell) {
            for (int i = x; i < x + cell.x; ++i) {
                for (int j = y; j < y + cell.y; ++j) {
                    Cell current = cells[i, j];
                    if (current.x < 0 || current.y < 0) {
                        Clear(i + current.x, j + current.y);
                    }
                    else if (current.x > 0 && current.y > 0) {
                        Clear(i, j);
                    }
                }
            }
            for (int i = x; i < x + cell.x; ++i) {
                for (int j = y; j < y + cell.y; ++j) {
                    if (i == x && j == y) {
                        cells[i, j] = cell;
                    }
                    else {
                        cells[i, j].x = x - i;
                        cells[i, j].y = y - j;
                    }
                }
            }
        }
        public int EmptyCount {
            get {
                int count = 0;
                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                        if (cells[i, j].x == 0 && cells[i, j].y == 0)
                            ++count;
                return count;
            }
        }
        private void Clear(int x, int y) {
            Cell cell = cells[x, y];
            for (int i = x; i < x + cell.x; ++i) {
                for (int j = y; j < y + cell.y; ++j) {
                    cells[i, j].x = 0;
                    cells[i, j].y = 0;
                }
            }
        }


        private int width;
        private int height;
        private Cell[,] cells;
    }

    public struct Cell {
        public int x;
        public int y;

        public Cell(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}
