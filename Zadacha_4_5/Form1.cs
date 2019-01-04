using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zadacha_4_5
{
    public partial class Form1 : Form
    {
        Graphics graphicsContainer;
        public Form1()
        {
            InitializeComponent();
            graphicsContainer = CreateGraphics();
        }

        TransporterManager transporterManager;
        Image image;

        private void StartButton_Click(object sender, EventArgs e)
        {
            image = Image.FromFile(System.IO.Path.GetFullPath(@"..\..\..\Zadacha_4_5\Graphics\product.jpg"));             //("C:/Users/1/source/repos/Zadacha_4_5/Zadacha_4_5/Graphics/product.jpg");

            transporterManager = new TransporterManager();
            transporterManager.Progress += Progress;
            transporterManager.Draw += Draw;
            transporterManager.DrawNewProduct += DrawNewProduct;
            transporterManager.AddToStoreProduct += AddToStoreProduct;

            transporterManager.InitTransporters(4);
            transporterManager.InitLoaders(2);
            transporterManager.InitRepairs(2);

            transporterManager.StartManager();
        }
        private void AddToStoreProduct(Product product)
        {
            Action action = () =>
            {
                product.View.Dispose();
            };

            Invoke(action);
        }

        private void DrawNewProduct(Product product)
        {
            Action action = () =>
            {
                var bitmap = new Bitmap(image, new Size(10, 10));
                var point = new Point(product.pos.x, product.pos.y);

                var pictureBox = new PictureBox();
                pictureBox.Size = new Size(10, 10);
                pictureBox.Image = bitmap;
                product.View = pictureBox;
                product.View.Location = point;

                Controls.Add(product.View);
            };

            Invoke(action);
        }

        private void Draw(Transporter transporter)
        {
            graphicsContainer.DrawRectangle(new Pen(Color.DarkBlue), new Rectangle(transporter.Pos.x - 10, transporter.Pos.y - 100, 25, 110));
        }

        private void Progress(Transporter transporter)
        {
            Action action = () =>
            {
                for (int i = 0; i < transporter.Products.Count; i++)
                {
                    var product = transporter.Products[i];

                    var pb = Controls.OfType<PictureBox>().First(p => p == product.View);

                    pb.Location = new Point(product.pos.x, product.pos.y);
                }
            };

           Invoke(action);
        }
    }
}
