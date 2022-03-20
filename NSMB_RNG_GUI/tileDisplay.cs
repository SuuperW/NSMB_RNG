using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                updateBoxes();
            }
        }

        public tileDisplay()
        {
            InitializeComponent();
            pbxArray = new PictureBox[0];
        }

        private void updateBoxes()
        {
            foreach (PictureBox box in pbxArray)
            {
                Controls.Remove(box);
                box.Dispose();
            }

            pbxArray = new PictureBox[TileCount + 1];
            for (int i = 0; i < pbxArray.Length; i++)
            {
                PictureBox box = new PictureBox();
                box.Visible = false;
                box.Location = new Point(i * 32, 0);
                box.Size = new Size(32, 32);
                box.BackgroundImageLayout = ImageLayout.Zoom;
                box.BackColor = Color.Red;

                pbxArray[i] = box;
                Controls.Add(box);
            }
        }

        public List<int> update(string tilePattern)
        {
            string upper = tilePattern.ToUpper();
            bool inputIsValid = true;
            int maxLength = pbxArray.Length - 1;
            List<int> userPattern = new List<int>(maxLength);
            int pi = 0;
            for (int i = 0; i < upper.Length; i++)
            {
                if (upper[i] == ' ')
                    continue;

                int index = Array.IndexOf(TilesFor12.tileLetters, upper[i]);
                if (index == -1 || pi >= maxLength)
                {
                    if (pi >= maxLength)
                        pbxArray[pi].BackgroundImage = tooLong;
                    else
                        pbxArray[pi].BackgroundImage = null;
                    pbxArray[pi].Visible = true;
                    pi++;
                    inputIsValid = false;
                    break;
                }
                else
                {
                    pbxArray[pi].BackgroundImage = tiles[index];
                    pbxArray[pi].Visible = true;
                    userPattern.Add(index);
                    pi++;
                }
            }

            for (int i = pi; i < pbxArray.Length; i++)
                pbxArray[i].Visible = false;

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

            return true;
        }
    }
}
