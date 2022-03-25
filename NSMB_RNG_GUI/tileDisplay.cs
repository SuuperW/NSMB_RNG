using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using NSMB_RNG;

namespace NSMB_RNG_GUI
{
    public partial class tileDisplay : UserControl
    {
        static Bitmap[] tiles = new Bitmap[] { Properties.Resources.tileB, Properties.Resources.tileE, Properties.Resources.tileI,
            Properties.Resources.tileC, Properties.Resources.tileP, Properties.Resources.tileS };
        static Bitmap tooLong = Properties.Resources.tooLong;

        PictureBox[] pbxArray;

        private int _tileCount = -1;
        public int TileCount
        {
            get => _tileCount;
            set
            {
                if (value < 0 || value > 11)
                    _tileCount = -1;
                else
                    _tileCount = value;

                initializeBoxes();
            }
        }

        public bool HasVisibleTiles
        {
            get => pbxArray.Length > 0 && pbxArray[0].Visible;
        }

        public tileDisplay()
        {
            InitializeComponent();
            pbxArray = new PictureBox[0];
        }

        private int _lastHeight = 0;
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Created && Height != _lastHeight)
            {
                _lastHeight = Height;
                // scale the picture boxes
                int size = Height;
                for (int i = 0; i < pbxArray.Length; i++)
                {
                    pbxArray[i].Location = new Point(i * size, 0);
                    pbxArray[i].Size = new Size(size, size);
                }
            }
        }

        private void initializeBoxes()
        {
            foreach (PictureBox box in pbxArray)
            {
                Controls.Remove(box);
                box.Dispose();
            }

            // Using the height of the control allows it to be scaled (include for high DPI)
            int size = Height;
            pbxArray = new PictureBox[TileCount + 1];
            for (int i = 0; i < pbxArray.Length; i++)
            {
                PictureBox box = new PictureBox();
                box.Visible = false;
                box.Location = new Point(i * size, 0);
                box.Size = new Size(size, size);
                box.BackgroundImageLayout = ImageLayout.Zoom;
                box.BackColor = Color.Red;

                pbxArray[i] = box;
                Controls.Add(box);
            }
        }

        public List<int> update(string tilePattern)
        {
            // case-insensitive
            string upper = tilePattern.ToUpper();

            bool inputIsValid = true;
            int maxLength = pbxArray.Length - 1;
            List<int> userPattern = new List<int>(maxLength);
            int pi = 0; // index in the picture box array
            for (int i = 0; i < upper.Length; i++)
            {
                // ignore whitespace
                if (upper[i] == ' ')
                    continue;

                int index = Array.IndexOf(TilesFor12.tileLetters, upper[i]);
                if (index == -1 || pi >= maxLength)
                {
                    // input has too many tiles
                    if (pi >= maxLength)
                        pbxArray[pi].BackgroundImage = tooLong;
                    else // input has invalid character; no image will show red background
                        pbxArray[pi].BackgroundImage = null;
                    pbxArray[pi].Visible = true; 
                    pi++;
                    inputIsValid = false;
                    break;
                }
                else
                {
                    // show tile image
                    pbxArray[pi].BackgroundImage = tiles[index];
                    pbxArray[pi].Visible = true;
                    userPattern.Add(index);
                    pi++;
                }
            }

            // make remaining boxes invisible and adjust width
            for (int i = pi; i < pbxArray.Length; i++)
                pbxArray[i].Visible = false;
            Width = Height * pi;

            if (inputIsValid) return userPattern;
            else return new List<int>();
        }

        public bool update(byte[] pattern)
        {
            if (pattern.Length != TileCount) return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                pbxArray[i].BackgroundImage = tiles[pattern[i]];
                pbxArray[i].Visible = true;
            }
            pbxArray[pbxArray.Length - 1].Visible = false;
            Width = Height * pattern.Length;

            return true;
        }
    }
}
