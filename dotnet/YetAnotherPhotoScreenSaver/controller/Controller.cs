using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Org.Kuhn.Yapss {
    class Controller : IController {
        public Controller(int w, int h, int xPixels, int yPixels, IImageSource imageSource) {
            this.layout = new Layout(w, h);
            this.xPixels = xPixels;
            this.yPixels = yPixels;
            this.imageSource = imageSource;
            round = 0;
            rounds = new int[w, h];
        }
        public Instruction GetInstruction() {
            // first look for any empty squares that need to be filled with a 1x1 image
            for (int j = 0; j < layout.Height; ++j) {
                for (int i = 0; i < layout.Width; ++i) {
                    Cell cell = layout.Get(i, j);
                    if (cell.x == 0 && cell.y == 0) {
                        return instruction(i, j, 1, 1, false);
                    }
                }
            }

            // next "round"
            ++round;

            // no empty squares, so use a large image
            int w = 0, h = 0;
            while (w == 0 || h == 0 || (float)w / (float)h > 2.0 || (float)w / (float)h < 0.5) {
                w = random.Next(2, layout.Width / 2 + 1);
                h = random.Next(2, layout.Width / 2 + 1);
            }

            // find locations overwriting oldest cells
            int minAge = int.MaxValue;
            List<Cell> minAgeList = new List<Cell>(); // misusing Cell class.. oh well
            for (int i = 0; i < layout.Width - w + 1; ++i) {
                for (int j = 0; j < layout.Height - h + 1; ++j) {
                    int age = 0;
                    for (int ii = i; ii < i + w; ++ii) {
                        for (int jj = j; jj < j + h; ++jj) {
                            Cell cell = layout.Get(ii, jj);
                            if (cell.x != 0 && cell.y != 0 && cell.x != 1 && cell.y != 1) {
                                int x = ii, y = jj;
                                if (cell.x < 0 || cell.y < 0) {
                                    x += cell.x;
                                    y += cell.y;
                                }
                                age = Math.Max(age, rounds[x, y]);
                            }
                        }
                    }
                    if (age < minAge) {
                        minAge = age;
                        minAgeList.Clear();
                    }
                    if (age == minAge)
                        minAgeList.Add(new Cell(i, j));
                }
            }
           
            // of those, choose a location causing the least "damage"
            int minDamage = int.MaxValue;
            List<Cell> minDamageList = new List<Cell>(); // misusing Cell class.. oh well
            foreach (Cell c in minAgeList) {
                Layout tempLayout = new Layout(layout);
                tempLayout.Set(c.x, c.y, new Cell(w, h));
                int damage = tempLayout.EmptyCount;
                if (damage < minDamage) {
                    minDamage = damage;
                    minDamageList.Clear();
                }
                if (damage == minDamage) {
                    minDamageList.Add(c);
                }
            }

            Cell coord = minDamageList[random.Next(minDamageList.Count)];
            return instruction(coord.x, coord.y, w, h, true);
        }

        private Instruction instruction(int x, int y, int w, int h, bool longPause) {
            layout.Set(x, y, new Cell(w, h));
            rounds[x, y] = round;
            Instruction i = new Instruction();
            i.x = x;
            i.y = y;
            i.w = w;
            i.h = h;
            i.image = imageSource.GetImage(w * xPixels, h * yPixels);
            i.longPause = longPause;
            return i;
        }

        private int xPixels;
        private int yPixels;
        private Layout layout;
        private int round;
        private int[,] rounds;
        private IImageSource imageSource;
        private Random random = new Random();
    }
}