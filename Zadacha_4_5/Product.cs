using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zadacha_4_5 
{
    public class Product
    {
        public Vector2 pos;
        public Action<Product> ReachEnd;
        public PictureBox View;

        public Product(Vector2 startPos)
        {
            pos = new Vector2(startPos.x, startPos.y);
        }

        public void Move()
        {
            pos.y--;
            if (pos.y <= 200)
            {
                ReachEnd?.Invoke(this);
            }
        }
    }
}
